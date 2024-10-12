#if NET8_0_OR_GREATER
#pragma warning disable IDE0305
#endif
#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
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
        private protected static object _placeholderValue = new object();
        private protected bool _disposedValue;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private protected byte[]? _prefix;
        private protected CommandPacket? _currentCommand;
        private protected CommandPacket[]? _currentCommands;
#else
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
        private protected SocketAsyncEventArgs _receiveArgs;
        private protected long _connectionId;
        private protected DateTime _lastActiveTime;
        private protected ushort _currentDataBaseIndex = 0;
        private byte[] _receiveBuffer;
        private WaitHandle[] _waitHandleBuffer;
        
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
            this._receiveArgs = new SocketAsyncEventArgs();
            this._receiveBuffer = new byte[connectionOptions.Buffer];
            this._waitHandleBuffer = new WaitHandle[2];
            this._waitHandleBuffer[0] = this._autoResetEvent;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            this._receiveArgs.SetBuffer(new Memory<byte>(this._receiveBuffer));
#else
            this._receiveArgs.SetBuffer(this._receiveBuffer, 0, this._receiveBuffer.Length);
#endif
            this._receiveArgs.Completed += this.ReceiveCompleted;
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
                this._socketClient.ReceiveAsync(this._receiveArgs);

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
            if (this._disposedValue)
            {
                this._connected = false;
                return new RedisConnectionException("The current Redis connection has been released");
            }
            try
            {
                DateTime timeout = DateTime.UtcNow.AddMilliseconds(this._connectionOptions.CommandTimeout);
                this.SendCommand(command, timeout, cancellationToken);
                var timespan = timeout - DateTime.UtcNow;
                this._waitHandleBuffer[1] = cancellationToken.WaitHandle;
                int index = WaitHandle.WaitAny(this._waitHandleBuffer, timespan);
                this._currentCommand = null;
                if (index != 0) throw new OperationCanceledException();
                var result = this._receiveArgs.UserToken;
                this._receiveArgs.UserToken = null;
                return result;
            }
            catch (OperationCanceledException)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                this._currentCommand = null;
                return new TimeoutException($"Command: [{command}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                this._currentCommand = null;
                return ex;
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public object? ExecuteCommands(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        public object ExecuteCommands(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            if (this._disposedValue)
            {
                this._connected = false;
                return new RedisConnectionException("The current Redis connection has been released");
            }
            try
            {
                DateTime timeout = DateTime.UtcNow.AddMilliseconds(this._connectionOptions.CommandTimeout);
                this.SendCommands(commands, timeout, cancellationToken);
                var timespan = timeout - DateTime.UtcNow;
                this._waitHandleBuffer[1] = cancellationToken.WaitHandle;
                int index = WaitHandle.WaitAny(this._waitHandleBuffer, timespan);
                this._currentCommands = null;
                if (index != 0) throw new OperationCanceledException();
                var result = this._receiveArgs.UserToken;
                this._receiveArgs.UserToken = null;
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
                this._receiveArgs.UserToken = null;
                this._currentCommands = null;
                return new TimeoutException($"Pipeline command: [{sb}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                this._currentCommands = null;
                return ex;
            }
        }

        #region Send command methods
        private void SendCommand(CommandPacket command, DateTime timeout, CancellationToken cancellationToken)
        {
            BaseConnection.ThrowIfCancellationRequested(timeout, cancellationToken);
            var sendArgs = this.GetSendArgs(command.ToResp(this._encoding, this._prefix));
            this._receiveArgs.UserToken = BaseConnection._placeholderValue;
            try
            {
                if (!this._socketClient.SendAsync(sendArgs)) sendArgs.Dispose();
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = ex;
                this._autoResetEvent.Set();
                return;
            }

            if ((command.Mode & CommandMode.WithoutActiveTime) != CommandMode.WithoutActiveTime) this._lastActiveTime = DateTime.UtcNow;
            if ((command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult)
            {
                this._receiveArgs.UserToken = DBNull.Value;
                this._autoResetEvent.Set();
                return;
            }
            this._currentCommand = command;
        }

        private void SendCommands(CommandPacket[] commands, DateTime timeout, CancellationToken cancellationToken)
        {
            BaseConnection.ThrowIfCancellationRequested(timeout, cancellationToken);
            var commandArrayBytes = new List<byte>();
            for (uint i = 0; i < commands.Length; i++)
            {
                var commandBytes = commands[i].ToResp(this._encoding, this._prefix);
                commandArrayBytes.AddRange(commandBytes);
            }
            var sendArgs = this.GetSendArgs(commandArrayBytes.ToArray());
            this._receiveArgs.UserToken = commands.Length;
            try
            {
                if (!this._socketClient.SendAsync(sendArgs)) sendArgs.Dispose();
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = ex;
                this._autoResetEvent.Set();
                return;
            }
            this._lastActiveTime = DateTime.UtcNow;
            this._currentCommands = commands;
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
            if (this._disposedValue)
            {
                this._connected = false;
                return new RedisConnectionException("The current Redis connection has been released");
            }
            try
            {
                DateTime timeout = DateTime.UtcNow.AddMilliseconds(this._connectionOptions.CommandTimeout);
                var tcs = this.SendCommandAsync(command, timeout, cancellationToken);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                object? result;
#else
                object result;
#endif
                if (cancellationToken == default)
                {
                    var timespan = timeout - DateTime.UtcNow;
                    if (timespan.TotalMilliseconds <= 0) throw new OperationCanceledException();
#if NET6_0_OR_GREATER
                    result = await tcs.Task.WaitAsync(timespan, CancellationToken.None).ConfigureAwait(false);
#else
                    var task = await Task.WhenAny(tcs.Task, Task.Delay(timespan, CancellationToken.None)).ConfigureAwait(false);
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
                this._receiveArgs.UserToken = null;
                this._currentCommand = null;
                return result;
            }
            catch (OperationCanceledException)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                this._currentCommand = null;
                return new TimeoutException($"Command: [{command}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                this._currentCommand = null;
                return ex;
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public async Task<object?> ExecuteCommandsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        public async Task<object> ExecuteCommandsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            if (this._disposedValue)
            {
                this._connected = false;
                return new RedisConnectionException("The current Redis connection has been released");
            }
            try
            {
                DateTime timeout = DateTime.UtcNow.AddMilliseconds(this._connectionOptions.CommandTimeout);
                var tcs = this.SendCommandsAsync(commands, timeout, cancellationToken);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                object? result;
#else
                object result;
#endif
                if (cancellationToken == default)
                {
                    var timespan = timeout - DateTime.UtcNow;
                    if (timespan.TotalMilliseconds <= 0) throw new OperationCanceledException();
#if NET6_0_OR_GREATER
                    result = await tcs.Task.WaitAsync(timespan, CancellationToken.None).ConfigureAwait(false);
#else
                    var task = await Task.WhenAny(tcs.Task, Task.Delay(timespan, CancellationToken.None)).ConfigureAwait(false);
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
                this._receiveArgs.UserToken = null;
                this._currentCommands = null;
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
                this._receiveArgs.UserToken = null;
                this._currentCommands = null;
                return new TimeoutException($"Pipeline command: [{sb}] execution timeout");
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                this._currentCommands = null;
                return ex;
            }
        }

        #region Send command methods
        private AliasTaskCompletionSource SendCommandAsync(CommandPacket command, DateTime timeout, CancellationToken cancellationToken)
        {
            BaseConnection.ThrowIfCancellationRequested(timeout, cancellationToken);
            var sendArgs = this.GetSendArgs(command.ToResp(this._encoding, this._prefix));
#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            var tcs = new AliasTaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
#else
            var tcs = new AliasTaskCompletionSource();
#endif
            this._receiveArgs.UserToken = tcs;
            try
            {
                if (!this._socketClient.SendAsync(sendArgs)) sendArgs.Dispose();
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                tcs.SetResult(ex);
                return tcs;
            }

            if ((command.Mode & CommandMode.WithoutActiveTime) != CommandMode.WithoutActiveTime) this._lastActiveTime = DateTime.UtcNow;
            if ((command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult)
            {
                tcs.SetResult(DBNull.Value);
                return tcs;
            }
            this._currentCommand = command;
            return tcs;
        }

        private AliasTaskCompletionSource SendCommandsAsync(CommandPacket[] commands, DateTime timeout, CancellationToken cancellationToken)
        {
            BaseConnection.ThrowIfCancellationRequested(timeout, cancellationToken);
            var commandArrayBytes = new List<byte>();
            for (uint i = 0; i < commands.Length; i++)
            {
                var commandBytes = commands[i].ToResp(this._encoding, this._prefix);
                commandArrayBytes.AddRange(commandBytes);
            }
            var sendArgs = this.GetSendArgs(commandArrayBytes.ToArray());
#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            var tcs = new AliasTaskCompletionSource(commands.Length, TaskCreationOptions.RunContinuationsAsynchronously);
#else
            var tcs = new AliasTaskCompletionSource(commands.Length);
#endif
            this._receiveArgs.UserToken = tcs;
            try
            {
                if (!this._socketClient.SendAsync(sendArgs)) sendArgs.Dispose();
            }
            catch (Exception ex)
            {
                this._connected = false;
                this._receiveArgs.UserToken = null;
                tcs.SetResult(ex);
                return tcs;
            }

            this._lastActiveTime = DateTime.UtcNow;
            this._currentCommands = commands;
            return tcs;
        }
        #endregion
#endif
        #endregion

        #region ReceiveCompleted
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private protected virtual void ReceiveCompleted(object? sender, SocketAsyncEventArgs e)
#else
        private protected virtual void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
#endif
        {
            if (e.SocketError != SocketError.Success || e.BytesTransferred is 0)
            {
                this._connected = false;
                if (e.UserToken != null)
                {
                    if (object.ReferenceEquals(e.UserToken, BaseConnection._placeholderValue))
                    {
                        e.UserToken = new RedisConnectionException("Unexpected connection closure");
                        return;
                    }
#if !LOW_NET
                    if (e.UserToken is AliasTaskCompletionSource lastTcs)
                    {
                        lock (lastTcs)
                        {
                            if (lastTcs.Task.Status == TaskStatus.WaitingForActivation)
                                lastTcs.SetResult(new RedisConnectionException("Unexpected connection closure"));
                        }
                        return;
                    }
#endif
                    e.UserToken = null;
                    return;
                }
                return;
            }
            if (e.UserToken is null) goto Continue;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            this._dataPacket.Write(e.MemoryBuffer.Slice(e.Offset, e.BytesTransferred).Span);
#else
            this._dataPacket.Write(e.Buffer, e.Offset, e.BytesTransferred);
#endif
            this._dataPacket.Seek(0, SeekOrigin.Begin);

            #region Sync
            {
                if (e.UserToken is int pipeCommandCount)
                {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    var result = new object?[pipeCommandCount];
#else
                    var result = new object[pipeCommandCount];
#endif
                    for (uint i = 0; i < pipeCommandCount; i++)
                    {
                        var dataType = this._currentCommands?[i].ResultDataType;
                        if (DataPacketExtensions.GetNextValue(this._dataPacket, this._encoding, dataType ?? ResultDataType.Bytes, out var data))
                        {
                            result[i] = data;
                        }
                        else
                        {
                            this._dataPacket.Seek(this._dataPacket.Length, SeekOrigin.Begin);
                            goto Continue;
                        }
                    }
                    this._dataPacket.SetLength(0);
                    e.UserToken = result;
                    this._autoResetEvent.Set();
                    goto Continue;
                }
                else
                {
                    if (object.ReferenceEquals(BaseConnection._placeholderValue, e.UserToken))
                    {
                        if (DataPacketExtensions.GetNextValue(this._dataPacket, this._encoding, this._currentCommand?.ResultDataType ?? ResultDataType.Bytes, out var data))
                        {
                            this._dataPacket.SetLength(0);
                            e.UserToken = data;
                            this._autoResetEvent.Set();
                            goto Continue;
                        }
                        else
                        {
                            this._dataPacket.Seek(this._dataPacket.Length, SeekOrigin.Begin);
                            goto Continue;
                        }
                    }
                }
            }
            #endregion

            #region Async
#if !LOW_NET
            {
                if (e.UserToken is AliasTaskCompletionSource tcs)
                {
                    if (tcs.Task.AsyncState is int pipeCommandCount)
                    {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        var result = new object?[pipeCommandCount];
#else
                        var result = new object[pipeCommandCount];
#endif
                        for (uint i = 0; i < pipeCommandCount; i++)
                        {
                            var dataType = this._currentCommands?[i].ResultDataType;
                            if (DataPacketExtensions.GetNextValue(this._dataPacket, this._encoding, dataType ?? ResultDataType.Bytes, out var data))
                            {
                                result[i] = data;
                            }
                            else
                            {
                                this._dataPacket.Seek(this._dataPacket.Length, SeekOrigin.Begin);
                                goto Continue;
                            }
                        }
                        this._dataPacket.SetLength(0);
                        lock (tcs)
                        {
                            if (tcs.Task.Status == TaskStatus.WaitingForActivation) tcs.SetResult(result);
                            goto Continue;
                        }
                    }
                    else
                    {
                        if (DataPacketExtensions.GetNextValue(this._dataPacket, this._encoding, this._currentCommand?.ResultDataType ?? ResultDataType.Bytes, out var data))
                        {
                            this._dataPacket.SetLength(0);
                            lock (tcs)
                            {
                                if (tcs.Task.Status == TaskStatus.WaitingForActivation) tcs.SetResult(data);
                                goto Continue;
                            }
                        }
                        else
                        {
                            this._dataPacket.Seek(this._dataPacket.Length, SeekOrigin.Begin);
                            goto Continue;
                        }
                    }
                }
            }
#endif
            #endregion

            Continue:
            if (!this._socketClient.ReceiveAsync(e))
            {
                this.ReceiveCompleted(sender, e);
            }
        }
        #endregion

        #region Peivate methods
        private SocketAsyncEventArgs GetSendArgs(byte[] commandBytes)
        {
            this._dataPacket.SetLength(0);
            var sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += BaseConnection.SendArgsCompleted;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            sendArgs.SetBuffer(commandBytes);
#else
            sendArgs.SetBuffer(commandBytes, 0, commandBytes.Length);
#endif
            return sendArgs;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private static void SendArgsCompleted(object? sender, SocketAsyncEventArgs e)
#else
        private static void SendArgsCompleted(object sender, SocketAsyncEventArgs e)
#endif
        {
            e?.Dispose();
        }

        private static void ThrowIfCancellationRequested(DateTime timeout, CancellationToken cancellationToken)
        {
            if (cancellationToken != default)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            else
            {
                var exptime = timeout - DateTime.UtcNow;
                if (exptime.TotalMilliseconds <= 0)
                {
                    throw new OperationCanceledException();
                }
            }
        }
        #endregion

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
                this._receiveArgs?.Dispose();
                this._dataPacket?.Dispose();
                (this._autoResetEvent as IDisposable)?.Dispose();
            }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            this._socketClient = null;
            this._connectionOptions = null;
            this._encoding = null;
            this._receiveArgs = null;
            this._dataPacket = null;
            this._encoding = null;
            this._prefix = null;
            this._receiveBuffer = null;
            this._waitHandleBuffer = null;
            this._autoResetEvent = null;
            this._currentCommand = null;
            this._currentCommands = null;
            BaseConnection._placeholderValue = null;
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
