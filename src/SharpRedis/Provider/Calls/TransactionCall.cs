#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using System;
using SharpRedis.Models;
using SharpRedis.Network.Standard;
using SharpRedis.Provider.Standard;
using System.Collections.Generic;
using SharpRedis.Extensions;
using SharpRedis.Commands;
using SharpRedis.Network;
using static SharpRedis.Provider.Calls.SwitchDatabaseCall;

namespace SharpRedis.Provider.Calls
{
    internal sealed class TransactionCall : BaseCall
    {
        private const string _notCallsErrorMsg = "Transactions do not support pipelines";

        private readonly bool _needReturnToPool = true;
        private DefaultConnection _connection;
        private List<ResultType> _commandResultTypes;
        private DatabaseConnection? _switchConnection;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private List<string?> _conditions;
        private List<object?> _queued;
        private SwitchDatabaseCall? _switchCall;
        private List<Type?> _enumTypes;
#else
        private List<string> _conditions;
        private List<object> _queued;
        private SwitchDatabaseCall _switchCall;
        private List<Type> _enumTypes;
#endif

        sealed internal override bool SubUsable => false;

        sealed internal override string CallMode => "Transaction mode";

        internal TransactionCall(IConnectionPool connectionPool, CancellationToken cancellationToken)
            : base(connectionPool)
        {
            this._connection = base.ConnectionPool.GetMasterConnection(cancellationToken);
            this._commandResultTypes = new List<ResultType>(8);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            this._conditions = new List<string?>(4);
            this._queued = new List<object?>(8);
            this._enumTypes = new List<Type?>(2);
#else
            this._conditions = new List<string>(4);
            this._queued = new List<object>(8);
            this._enumTypes = new List<Type>(2);
#endif
        }

        internal TransactionCall(IConnectionPool connectionPool, SwitchDatabaseCall switchCall, CancellationToken cancellationToken)
            : base(connectionPool)
        {
            this._needReturnToPool = false;
#if LOW_NET
            var switchConnection = switchCall.GetMasterConnection(cancellationToken);
#else
            var switchConnection = switchCall.GetMasterConnectionAsync(cancellationToken).GetAwaiter().GetResult();
#endif
            this._switchConnection = switchConnection;
            this._connection = switchConnection.Connection;
            this._switchCall = switchCall;
            this._commandResultTypes = new List<ResultType>(8);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            this._conditions = new List<string?>(8);
            this._queued = new List<object?>(8);
            this._enumTypes = new List<Type?>(2);
#else
            this._conditions = new List<string>(8);
            this._queued = new List<object>(8);
            this._enumTypes = new List<Type>(2);
#endif
        }

        #region Sync
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        sealed internal override object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        sealed internal override object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            if ((command.Mode & CommandMode.Transaction) is CommandMode.Transaction)
            {
                return this._connection.ExecuteCommand(command, cancellationToken);
            }
            this._commandResultTypes.Add(resultType);
            this._conditions.Add(null);
            this._queued.Add(this._connection.ExecuteCommand(command, cancellationToken));
            this._enumTypes.Add(null);
            return null;
        }

        internal sealed override bool CallCondition(CommandPacket command, string condition, CancellationToken cancellationToken)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            if ((command.Mode & CommandMode.Transaction) is CommandMode.Transaction)
            {
                var data = this._connection.ExecuteCommand(command, cancellationToken);
                if (data is null || data is DBNull) return false;
                var dataString = ConvertExtensions.To<string>(data, ResultType.String, base.Encoding);
                return dataString == condition;
            }

            this._commandResultTypes.Add(ResultType.String);
            this._conditions.Add(condition);
            this._queued.Add(this._connection.ExecuteCommand(command, cancellationToken));
            this._enumTypes.Add(null);
            return false;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override bool[]? CallConditionArray(CommandPacket command, string condition, CancellationToken cancellationToken)
#else
        internal sealed override bool[] CallConditionArray(CommandPacket command, string condition, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            this._commandResultTypes.Add(ResultType.String | ResultType.Array);
            this._conditions.Add(condition);
            this._queued.Add(this._connection.ExecuteCommand(command, cancellationToken));
            this._enumTypes.Add(null);
            return null;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override object?[]? Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override object[] Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            throw new NotSupportedException(TransactionCall._notCallsErrorMsg);
        }

        internal sealed override TEnum CallEnum<TEnum>(CommandPacket command, CancellationToken cancellationToken)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            this._commandResultTypes.Add(ResultType.Enum);
            this._conditions.Add(null);
            this._queued.Add(this._connection.ExecuteCommand(command, cancellationToken));
            this._enumTypes.Add(typeof(TEnum));
            return default;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override TEnum[]? CallEnumArray<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#else
        internal sealed override TEnum[] CallEnumArray<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            this._commandResultTypes.Add(ResultType.Array | ResultType.Enum);
            this._conditions.Add(null);
            this._queued.Add(this._connection.ExecuteCommand(command, cancellationToken));
            this._enumTypes.Add(typeof(TEnum));
            return default;
        }
        #endregion

        #region Async
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        sealed async internal override Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        sealed async internal override Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            if ((command.Mode & CommandMode.Transaction) is CommandMode.Transaction)
            {
                return await this._connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
            this._commandResultTypes.Add(resultType);
            this._conditions.Add(null);
            this._queued.Add(await this._connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false));
            this._enumTypes.Add(null);
            return null;
        }

        async internal sealed override Task<bool> CallConditionAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            if ((command.Mode & CommandMode.Transaction) is CommandMode.Transaction)
            {
                var data = await this._connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
                if (data is null || data is DBNull) return false;
                var dataString = ConvertExtensions.To<string>(data, ResultType.String, base.Encoding);
                return dataString == condition;
            }

            this._commandResultTypes.Add(ResultType.String);
            this._conditions.Add(condition);
            this._queued.Add(await this._connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false));
            this._enumTypes.Add(null);
            return false;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal sealed override Task<bool[]?> CallConditionArrayAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
#else
        async internal sealed override Task<bool[]> CallConditionArrayAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            this._commandResultTypes.Add(ResultType.String | ResultType.Array);
            this._conditions.Add(condition);
            this._queued.Add(await this._connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false));
            this._enumTypes.Add(null);
            return null;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            throw new NotSupportedException(TransactionCall._notCallsErrorMsg);
        }

        async internal sealed override Task<TEnum> CallEnumAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            this._commandResultTypes.Add(ResultType.Enum);
            this._conditions.Add(null);
            this._queued.Add(await this._connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false));
            this._enumTypes.Add(typeof(TEnum));
            return default;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal sealed override Task<TEnum[]?> CallEnumArrayAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#else
        async internal sealed override Task<TEnum[]> CallEnumArrayAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), "The current connection has been released and cannot continue");
            TransactionCall.CommandNonsupportException(command);
            this._commandResultTypes.Add(ResultType.Array | ResultType.Enum);
            this._conditions.Add(null);
            this._queued.Add(await this._connection.ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false));
            this._enumTypes.Add(typeof(TEnum));
            return default;
        }
#endif
        #endregion

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal TransactionValue ToResult(object? data)
#else
        internal TransactionValue ToResult(object data)
#endif
        {
            var queued = this._queued.ToArray();
            if (data is null)
            {
                this.Reset();
                return new TransactionValue(null, queued, false, null);
            }
            if (data is object[] dataArray)
            {
                if (dataArray.Length == 0)
                {
                    this.Reset();
                    return new TransactionValue(null, queued, false, null);
                }
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                var result = new object?[dataArray.Length];
#else
                var result = new object[dataArray.Length];
#endif

                for (int i = 0; i < dataArray.Length; i++)
                {
                    try
                    {
                        var resultType = this._commandResultTypes[i];
                        if (resultType == ResultType.String)
                        {
#if NET5_0_OR_GREATER
                            if (dataArray[i] is not string stringValue)
#else
                            if (!(dataArray[i] is string stringValue))
#endif
                            {
                                stringValue = ConvertExtensions.To<string>(dataArray[i], resultType, base.Encoding)
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                                    !
#endif
                                    ;
                            }
                            if (this._conditions[i] != null)
                            {
                                result[i] = stringValue == this._conditions[i];
                            }
                            else
                            {
                                result[i] = stringValue;
                            }
                        }
                        else if ((resultType & ResultType.Array) == ResultType.Array)
                        {
                            if (dataArray[i] is object[] objArray && objArray.Length > 0)
                            {
                                var valueType = resultType & ~ResultType.Array;
                                if (valueType == ResultType.String)
                                {
                                    if (this._conditions[i] != null)
                                    {
                                        var boolArray = new bool[objArray.Length];
                                        var condition = this._conditions[i];
                                        for (int z = 0; z < objArray.Length; z++)
                                        {
                                            boolArray[z] = ConvertExtensions.To<string>(objArray[z], ResultType.String, base.Encoding) == condition;
                                        }
                                        result[i] = boolArray;
                                    }
                                    else
                                    {
                                        result[i] = ConvertExtensions.To<string[]>(dataArray[i], ResultType.String | ResultType.Array, base.Encoding);
                                    }
                                }
                                else if (valueType == ResultType.Enum && this._enumTypes[i] != null)
                                {
                                    var enumArray = new object[objArray.Length];
                                    for (int z = 0; z < objArray.Length; z++)
                                    {
                                        var intValue = ConvertExtensions.To<int>(objArray[z], ResultType.Enum, base.Encoding);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                        enumArray[z] = Enum.ToObject(this._enumTypes[i]!, intValue);
#else
                                        enumArray[z] = Enum.ToObject(this._enumTypes[i], intValue);
#endif
                                    }
                                    result[i] = enumArray;
                                }
                                else
                                {
                                    result[i] = ConvertExtensions.To<object>(dataArray[i], resultType, base.Encoding);
                                }
                            }
                            else
                            {
                                result[i] = null;
                                continue;
                            }
                        }
                        else if (resultType == ResultType.Enum && this._enumTypes[i] != null)
                        {
                            var intValue = ConvertExtensions.To<int>(dataArray[i], ResultType.Enum, base.Encoding);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            result[i] = Enum.ToObject(this._enumTypes[i]!, intValue);
#else
                            result[i] = Enum.ToObject(this._enumTypes[i], intValue);
#endif
                        }
                        else
                        {
                            result[i] = ConvertExtensions.To<object>(dataArray[i], resultType, base.Encoding);
                        }
                    }
                    catch (Exception ex)
                    {
                        result[i] = ex;
                        continue;
                    }
                }
                this.Reset();
                return new TransactionValue(result, queued, false, null);
            }
            else if (data is Exception ex)
            {
                this.Reset();
                return new TransactionValue(null, queued, true, ex);
            }
            else
            {
                this.Reset();
                return new TransactionValue(null, queued, true, new RedisException("Transaction return value that cannot be resolved"));
            }
        }

        internal void Reset()
        {
            this._commandResultTypes.Clear();
            this._conditions.Clear();
            this._queued.Clear();
            this._enumTypes.Clear();
        }

        #region Dispose
        protected sealed override void Dispose(bool disposing)
        {
            if (this._connection != null)
            {
                this._connection.ExecuteCommand(TransactionCommands.Discard(), CancellationToken.None);
                if (this._needReturnToPool) base.ConnectionPool.ReturnMasterConnection(this._connection);
            }
            if (!this._needReturnToPool && this._switchCall != null && this._switchConnection.HasValue)
            {
                this._switchCall.ReturnDatabaseConnection(this._switchConnection.Value);
            }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            if (!base._disposedValue)
            {
                base._disposedValue = true;
                if (disposing)
                {
                    this._commandResultTypes?.Clear();
                    this._conditions?.Clear();
                    this._queued?.Clear();
                    this._enumTypes?.Clear();
                }
            }

            this._switchCall = null;
            this._switchConnection = null;
            this._connection = null;
            this._commandResultTypes = null;
            this._conditions = null;
            this._queued = null;
            this._enumTypes = null;
            base.Dispose(disposing);
        }

        ~TransactionCall()
        {
            this.Dispose(true);
        }
        #endregion

        private static void CommandNonsupportException(CommandPacket command)
        {
            if ((command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult
                || (command.Mode & CommandMode.Sub) == CommandMode.Sub
                || (command.Mode & CommandMode.WithBlock) == CommandMode.WithBlock)
            {
                throw new NotSupportedException($"The transaction does not support the command [{command}]");
            }
        }
    }
}
