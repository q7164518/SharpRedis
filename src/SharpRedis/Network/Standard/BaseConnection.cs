#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using AliasTaskCompletionSource = System.Threading.Tasks.TaskCompletionSource<object?>;
#else
#if !LOW_NET
using AliasTaskCompletionSource = System.Threading.Tasks.TaskCompletionSource<object>;
#endif
#endif
#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif
#if !LOW_NET
using System.Threading.Tasks;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using SharpRedis.Models;
using System.Threading;
using SharpRedis.Extensions;

namespace SharpRedis.Network.Standard
{
    internal abstract class BaseConnection : IConnection
    {
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private protected AliasTaskCompletionSource? _asyncTcs;
#else
        private protected AliasTaskCompletionSource _asyncTcs;
#endif
#endif
        private protected bool _currentIsSync = true;
        private protected bool _disposedValue;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private protected object? _syncResult;
        private protected byte[]? _prefix;
        private protected CommandPacket? _currentCommand;
        private protected CommandPacket[]? _currentCommands;
#else
        private protected object _syncResult;
        private protected byte[] _prefix;
        private protected CommandPacket _currentCommand;
        private protected CommandPacket[] _currentCommands;
#endif
        private protected AutoResetEvent _autoResetEvent;
        private protected bool _connected = false;
        private protected Socket _socketClient;
        private protected MemoryStream _dataPacket;
        private protected Encoding _encoding;
        private protected ConnectionOptions _connectionOptions;
        private protected long _connectionId;
        private protected DateTime _lastActiveTime;
        private protected ushort _currentDataBaseIndex = 0;
        private protected byte[] _receiveBuffer;
        private protected WaitHandle[] _waitHandleBuffer;

        #region Properties
        public bool Connected => this._socketClient?.Connected == true && this._connected;

        public long ConnectionId => this._connectionId;

        public ConnectionOptions ConnectionOptions => this._connectionOptions;

        public ConnectionType Type { get; }

        public string ConnectionName { get; }

        public DateTime LastActiveTime => this._lastActiveTime;

        public ushort CurrentDataBaseIndex => this._currentDataBaseIndex;

        public bool Tracking { get; set; }

        public long RedirectConnectionId { get; set; }

        public Encoding Encoding => this._encoding;
        #endregion

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private protected BaseConnection(Encoding encoding, ConnectionOptions connectionOptions, ConnectionType connectionType, byte[]? _prefix)
#else
        private protected BaseConnection(Encoding encoding, ConnectionOptions connectionOptions, ConnectionType connectionType, byte[] _prefix)
#endif
        {
            this._socketClient = new Socket(connectionOptions.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this._autoResetEvent = new AutoResetEvent(false);
            this._dataPacket = new MemoryStream(connectionOptions.Buffer);
            this._encoding = encoding;
            this._connectionOptions = connectionOptions;
            this._prefix = _prefix;
            this.Type = connectionType;
            this.ConnectionName = $"{connectionOptions.ConnectName?.Trim()}_SharpRedis_{connectionType}_{Guid.NewGuid():N}".TrimStart('_');
            this._receiveBuffer = new byte[connectionOptions.Buffer];
            this._waitHandleBuffer = new WaitHandle[2];
            this._waitHandleBuffer[0] = this._autoResetEvent;
        }

        public void ResetBuffer()
        {
            this._dataPacket.SetLength(0);
            this._dataPacket.Capacity = this._connectionOptions.Buffer;
        }

        public bool Connect()
        {
            try
            {
                this._socketClient.Connect(this._connectionOptions.Host, this._connectionOptions.Port);
                if (!this._socketClient.Connected)
                {
                    this.Dispose();
                    return false;
                }
                this._connected = true;
                this.ConnectedEvent();
                if (this._connectionOptions.RespVersion is 3)
                {
                    if (!ConnectionExtensions.Hello(this, (RespVersion)this._connectionOptions.RespVersion, this._connectionOptions.User, this._connectionOptions.Password))
                    {
                        this.Dispose();
                        return false;
                    }
                }
                else
                {
                    if (!ConnectionExtensions.Auth(this, this._connectionOptions.User, this._connectionOptions.Password))
                    {
                        this.Dispose();
                        return false;
                    }
                    if (!ConnectionExtensions.SetClientName(this))
                    {
                        this.Dispose();
                        return false;
                    }
                }
                this._connectionId = ConnectionExtensions.ClientId(this);
                if (this._currentDataBaseIndex != this._connectionOptions.DefaultDatabase)
                {
                    if (!ConnectionExtensions.Select(this, this._connectionOptions.DefaultDatabase))
                    {
                        this.Dispose();
                        return false;
                    }
                    this._currentDataBaseIndex = this._connectionOptions.DefaultDatabase;
                }
                return true;
            }
            catch (Exception ex)
            {
                SharpConsole.WriteError($"Connect Exception, message: {ex.Message}");
                return false;
            }
        }

        public bool SwitchDatabaseIndex(ushort databaseIndex)
        {
            if (this._currentDataBaseIndex == databaseIndex) return true;
            if (!ConnectionExtensions.Select(this, databaseIndex)) return false;
            this._currentDataBaseIndex = databaseIndex;
            return true;
        }

        #region Sync
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public object? ExecuteCommand(CommandPacket command, CancellationToken cancellationToken)
#else
        public object ExecuteCommand(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            try
            {
                this.SendCommand(command, cancellationToken);
                var timespan = TimeSpan.FromMilliseconds(this._connectionOptions.CommandTimeout);
                this._waitHandleBuffer[1] = cancellationToken.WaitHandle;
                int index = WaitHandle.WaitAny(this._waitHandleBuffer, timespan);
                this._waitHandleBuffer[1] = null
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                    !
#endif
                    ;
                if (index != 0) throw new OperationCanceledException();
                this._currentCommand = null;
                var result = this._syncResult;
                this._syncResult = null;
                return result;
            }
            catch (OperationCanceledException)
            {
                this._connected = false;
                this._currentCommand = null;
                this._syncResult = null;
                return new TimeoutException($"Command: [{command}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._currentCommand = null;
                this._syncResult = null;
                return ex;
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public object? ExecuteCommands(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        public object ExecuteCommands(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            try
            {
                this.SendCommands(commands, cancellationToken);
                var timespan = TimeSpan.FromMilliseconds(this._connectionOptions.CommandTimeout);
                this._waitHandleBuffer[1] = cancellationToken.WaitHandle;
                int index = WaitHandle.WaitAny(this._waitHandleBuffer, timespan);
                this._waitHandleBuffer[1] = null
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                    !
#endif
                    ;
                if (index != 0) throw new OperationCanceledException();
                this._currentCommands = null;
                var result = this._syncResult;
                this._syncResult = null;
                return result;
            }
            catch (OperationCanceledException)
            {
                var sb = new StringBuilder();
                foreach (var c in commands)
                {
                    sb.Append(c.ToString()).Append(" && ");
                }
                sb.Remove(sb.Length - 5, 4);
                this._connected = false;
                this._currentCommands = null;
                this._syncResult = null;
                return new TimeoutException($"Pipeline command: [{sb}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._currentCommands = null;
                this._syncResult = null;
                return ex;
            }
        }

        #region Send command methods
        private void SendCommand(CommandPacket command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this._currentIsSync = true;
            try
            {
                this.DiscardAvailable();
                var commandBytes = command.ToResp(this._encoding, this._prefix);
                if ((command.Mode & CommandMode.WithoutActiveTime) != CommandMode.WithoutActiveTime) this._lastActiveTime = DateTime.UtcNow;
                this._currentCommand = command;
                if (this._socketClient.Send(commandBytes, 0, commandBytes.Length, SocketFlags.None) != commandBytes.Length)
                {
                    throw new RedisException("Incomplete data transmission");
                }
                if ((command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult)
                {
                    this._syncResult = DBNull.Value;
                    _ = this._autoResetEvent.Set();
                    return;
                }
                this.BeginReceive();
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._syncResult = ex;
                _ = this._autoResetEvent.Set();
                return;
            }
        }

        private void SendCommands(CommandPacket[] commands, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this._currentIsSync = true;
            try
            {
                this.DiscardAvailable();
                var commandArrayBytes = new List<byte>();
                for (uint i = 0; i < commands.Length; i++)
                {
                    var commandBytes = commands[i].ToResp(this._encoding, this._prefix);
                    commandArrayBytes.AddRange(commandBytes);
                }
                var commandsBytes = commandArrayBytes.ToArray();
                commandArrayBytes.Clear();
                commandArrayBytes.Capacity = 0;
                commandArrayBytes = null;
                this._currentCommands = commands;
                this._lastActiveTime = DateTime.UtcNow;
                if (this._socketClient.Send(commandsBytes, 0, commandsBytes.Length, SocketFlags.None) != commandsBytes.Length)
                {
                    throw new RedisException("Incomplete data transmission");
                }
                this.BeginReceive();
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._syncResult = ex;
                _ = this._autoResetEvent.Set();
                return;
            }
        }
        #endregion
        #endregion

        #region Async
#if !LOW_NET
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public async Task<object?> ExecuteCommandAsync(CommandPacket command, CancellationToken cancellationToken)
#else
        public async Task<object> ExecuteCommandAsync(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            try
            {
                var tcs = await this.SendCommandAsync(command, cancellationToken).ConfigureAwait(false);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                object? result;
#else
                object result;
#endif
                if (cancellationToken == default)
                {
                    var timespan = TimeSpan.FromMilliseconds(this._connectionOptions.CommandTimeout);
#if NET6_0_OR_GREATER
                    result = await tcs.Task.WaitAsync(timespan, cancellationToken).ConfigureAwait(false);
#else
                    var task = await Task.WhenAny(tcs.Task, Task.Delay(timespan, cancellationToken)).ConfigureAwait(false);
                    if (object.ReferenceEquals(task, tcs.Task))
                    {
                        result = await tcs.Task.ConfigureAwait(false);
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
#endif
                }
                else
                {
#if NET6_0_OR_GREATER
                    result = await tcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
#else
                    result = await tcs.Task.ContinueWith(res => res.Result, cancellationToken).ConfigureAwait(false);
#endif
                }
                this._currentCommand = null;
                this._asyncTcs = null;
                return result;
            }
            catch (OperationCanceledException)
            {
                this._connected = false;
                this._currentCommand = null;
                this._asyncTcs = null;
                return new TimeoutException($"Command: [{command}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._currentCommand = null;
                this._asyncTcs = null;
                return ex;
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public async Task<object?> ExecuteCommandsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        public async Task<object> ExecuteCommandsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            try
            {
                var tcs = await this.SendCommandsAsync(commands, cancellationToken).ConfigureAwait(false);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                object? result;
#else
                object result;
#endif
                if (cancellationToken == default)
                {
                    var timespan = TimeSpan.FromMilliseconds(this._connectionOptions.CommandTimeout);
#if NET6_0_OR_GREATER
                    result = await tcs.Task.WaitAsync(timespan, cancellationToken).ConfigureAwait(false);
#else
                    var task = await Task.WhenAny(tcs.Task, Task.Delay(timespan, cancellationToken)).ConfigureAwait(false);
                    if (object.ReferenceEquals(task, tcs.Task))
                    {
                        result = await tcs.Task.ConfigureAwait(false);
                    }
                    else
                    {
                        throw new OperationCanceledException();
                    }
#endif
                }
                else
                {
#if NET6_0_OR_GREATER
                    result = await tcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
#else
                    result = await tcs.Task.ContinueWith(res => res.Result, cancellationToken).ConfigureAwait(false);
#endif
                }
                this._currentCommands = null;
                this._asyncTcs = null;
                return result;
            }
            catch (OperationCanceledException)
            {
                var sb = new StringBuilder();
                foreach (var c in commands)
                {
                    sb.Append(c.ToString()).Append(" && ");
                }
                sb.Remove(sb.Length - 5, 4);
                this._connected = false;
                this._currentCommands = null;
                this._asyncTcs = null;
                return new TimeoutException($"Pipeline command: [{sb}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._currentCommands = null;
                this._asyncTcs = null;
                return ex;
            }
        }

        #region Send command methods
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
        async
#endif
        private Task<AliasTaskCompletionSource> SendCommandAsync(CommandPacket command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            var tcs = new AliasTaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
#else
            var tcs = new AliasTaskCompletionSource();
#endif
            this._currentIsSync = false;
            try
            {
                this.DiscardAvailable();
                var commandBytes = command.ToResp(this._encoding, this._prefix);
                this._asyncTcs = tcs;
                if ((command.Mode & CommandMode.WithoutActiveTime) != CommandMode.WithoutActiveTime) this._lastActiveTime = DateTime.UtcNow;
                this._currentCommand = command;
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
                if (await this._socketClient.SendAsync(new ArraySegment<byte>(commandBytes, 0, commandBytes.Length), SocketFlags.None
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                    , cancellationToken
#endif
                    ).ConfigureAwait(false) != commandBytes.Length)
                {
                    throw new RedisException("Incomplete data transmission");
                }
#else
                if (this._socketClient.Send(commandBytes, 0, commandBytes.Length, SocketFlags.None) != commandBytes.Length)
                {
                    throw new RedisException("Incomplete data transmission");
                }
#endif
                if ((command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult)
                {
                    tcs.SetResult(DBNull.Value);
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
                    return tcs;
#else
                    return Task.FromResult(tcs);
#endif
                }
                this.BeginReceive();
            }
            catch (Exception ex)
            {
                this._connected = false;
                tcs.SetResult(ex);
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
                return tcs;
#else
                return Task.FromResult(tcs);
#endif
            }
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            return tcs;
#else
            return Task.FromResult(tcs);
#endif
        }

#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
        async
#endif
        private Task<AliasTaskCompletionSource> SendCommandsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            var tcs = new AliasTaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
#else
            var tcs = new AliasTaskCompletionSource();
#endif
            this._currentIsSync = false;
            try
            {
                this.DiscardAvailable();
                var commandArrayBytes = new List<byte>();
                for (uint i = 0; i < commands.Length; i++)
                {
                    var commandBytes = commands[i].ToResp(this._encoding, this._prefix);
                    commandArrayBytes.AddRange(commandBytes);
                }
                var commandsBytes = commandArrayBytes.ToArray();
                commandArrayBytes.Clear();
                commandArrayBytes.Capacity = 0;
                commandArrayBytes = null;
                this._asyncTcs = tcs;
                this._lastActiveTime = DateTime.UtcNow;
                this._currentCommands = commands;
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
                if (await this._socketClient.SendAsync(new ArraySegment<byte>(commandsBytes, 0, commandsBytes.Length), SocketFlags.None
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                    , cancellationToken
#endif
                    ).ConfigureAwait(false) != commandsBytes.Length)
                {
                    throw new RedisException("Incomplete data transmission");
                }
#else
                if (this._socketClient.Send(commandsBytes, 0, commandsBytes.Length, SocketFlags.None) != commandsBytes.Length)
                {
                    throw new RedisException("Incomplete data transmission");
                }
#endif
                this.BeginReceive();
            }
            catch (Exception ex)
            {
                this._connected = false;
                tcs.SetResult(ex);
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
                return tcs;
#else
                return Task.FromResult(tcs);
#endif
            }
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            return tcs;
#else
            return Task.FromResult(tcs);
#endif
        }
        #endregion
#endif
        #endregion

        #region ReceiveCompleted
        private protected virtual void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesReceived = this._socketClient.EndReceive(ar);
                if (bytesReceived <= 0) //closure
                {
                    this._connected = false;
                    this.SetResult(new RedisConnectionException("Unexpected connection closure"));
                    return;
                }

                this._dataPacket.Write(this._receiveBuffer, 0, bytesReceived);
                if (this._socketClient.Available > 0) goto Continue;
                _ = this._dataPacket.Seek(0, SeekOrigin.Begin);

                #region Single command
                if (this._currentCommand != null)
                {
                    if (DataPacketExtensions.GetNextValue(this._dataPacket, this._encoding, this._currentCommand.ResultDataType, out var result))
                    {
                        this.SetResult(result);
                        return;
                    }
                    else
                    {
                        if (this._socketClient.Available <= 0)
                        {
                            throw new RedisException("The data cannot be parsed, possibly due to packet loss");
                        }
                        _ = this._dataPacket.Seek(this._dataPacket.Length, SeekOrigin.Begin);
                        goto Continue;
                    }
                }
                #endregion

                #region Command pipeline
                if (this._currentCommands != null)
                {
                    int pipeCommandCount = this._currentCommands.Length;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    var result = new object?[pipeCommandCount];
#else
                    var result = new object[pipeCommandCount];
#endif
                    for (uint i = 0; i < pipeCommandCount; i++)
                    {
                        var dataType = this._currentCommands[i].ResultDataType;
                        if (DataPacketExtensions.GetNextValue(this._dataPacket, this._encoding, dataType, out var data))
                        {
                            result[i] = data;
                        }
                        else
                        {
                            if (this._socketClient.Available <= 0)
                            {
                                throw new RedisException("The data cannot be parsed, possibly due to packet loss");
                            }
                            _ = this._dataPacket.Seek(this._dataPacket.Length, SeekOrigin.Begin);
                            goto Continue;
                        }
                    }
                    this.SetResult(result);
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                this._connected = false;
                this.SetResult(ex);
                return;
            }

            Continue:
            this.BeginReceive();
        }

        private protected void SetResult(object result, bool clearDataPacket = true)
        {
            if (clearDataPacket) this._dataPacket.SetLength(0);
#if LOW_NET
            this._syncResult = result;
            _ = this._autoResetEvent.Set();
#else
            if (this._currentIsSync)
            {
                this._syncResult = result;
                _ = this._autoResetEvent.Set();
            }
            else
            {
                if (this._asyncTcs != null)
                {
                    lock (this._asyncTcs)
                    {
                        if (this._asyncTcs.Task.Status == TaskStatus.WaitingForActivation)
                            this._asyncTcs.SetResult(result);
                    }
                }
            }
#endif
        }
        #endregion

        private void DiscardAvailable()
        {
            if (this._socketClient.Available > 0)
            {
                var discard = new byte[this._socketClient.Available];
                this._socketClient.Receive(discard);
            }
        }

        private protected virtual void BeginReceive()
        {
            _ = this._socketClient.BeginReceive(this._receiveBuffer, 0, this._receiveBuffer.Length, SocketFlags.None, this.ReceiveCallback, null);
        }

        private protected virtual void ConnectedEvent()
        {
        }

        #region Dispose
        protected private virtual void Dispose(bool disposing)
        {
            if (this._disposedValue) return;
            this._disposedValue = true;
            this._connected = false;

            if (disposing)
            {
                if (this._socketClient?.Connected == true)
                {
                    this._socketClient?.Shutdown(SocketShutdown.Both);
                    this._socketClient?.Close();
                    (this._socketClient as IDisposable)?.Dispose();
                }
                this._dataPacket?.Dispose();
                (this._autoResetEvent as IDisposable)?.Dispose();
            }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            this._socketClient = null;
            this._connectionOptions = null;
            this._encoding = null;
            this._dataPacket = null;
            this._encoding = null;
            this._prefix = null;
            this._receiveBuffer = null;
            this._waitHandleBuffer = null;
            this._autoResetEvent = null;
            this._currentCommand = null;
            this._currentCommands = null;
            this._syncResult = null;
#if !LOW_NET
            this._asyncTcs = null;
#endif
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~BaseConnection()
        {
            this.Dispose(disposing: true);
        }
        #endregion
    }
}
