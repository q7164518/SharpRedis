#if NET8_0
#pragma warning disable IDE0305
#endif
#if !LOW_NET
using System.Threading;
using System.Threading.Tasks;
#endif
#if NET40
using Task = System.Threading.Tasks.TaskEx;
#endif
using SharpRedis.Models;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;
using SharpRedis.Extensions;
using SharpRedis.Network.Standard;
using SharpRedis.Commands;

namespace SharpRedis.Provider.Calls
{
    internal sealed class PipeliningCall : BaseCall
    {
        private BaseCall _tCall;
        private List<CommandPacket> _commands;
        private List<ResultType> _commandResultTypes;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private List<string?> _conditions;
        private List<Type?> _enumTypes;
        private byte[]? _clientSideCachingPipePlaceholder;
        private CommandPacket? _echoOk;
        private CommandPacket? _cachingYesCommand;
        private CommandPacket? _cachingNoCommand;
#else
        private List<string> _conditions;
        private List<Type> _enumTypes;
        private byte[] _clientSideCachingPipePlaceholder;
        private CommandPacket _echoOk;
        private CommandPacket _cachingYesCommand;
        private CommandPacket _cachingNoCommand;
#endif

        internal sealed override bool SubUsable => false;

        internal sealed override string CallMode => "Pipeline mode";

        internal PipeliningCall(IConnectionPool connectionPool, BaseCall tCall)
            : base(connectionPool)
        {
            this._tCall = tCall;
            this._commands = new List<CommandPacket>(8);
            this._commandResultTypes = new List<ResultType>(8);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            this._conditions = new List<string?>(4);
            this._enumTypes = new List<Type?>(2);
#else
            this._conditions = new List<string>(4);
            this._enumTypes = new List<Type>(2);
#endif
            if (this._tCall is ClientSideCachingCall clientSideCachingCall)
            {
                this._clientSideCachingPipePlaceholder = base.Encoding.GetBytes(ClientSideCachingExtensions._clientSideCachingPipePlaceholder);
                this._echoOk = ConnectionCommands.Echo(this._clientSideCachingPipePlaceholder);
                if (clientSideCachingCall.ClientSideCache.Mode is ClientSideCachingMode.Default)
                {
                    this._cachingYesCommand = ConnectionCommands.ClientCaching(true);
                    this._cachingNoCommand = ConnectionCommands.ClientCaching(false);
                }
            }
        }

        #region Sync
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal object?[]? ExecPipelining(CancellationToken cancellationToken)
#else
        internal object[] ExecPipelining(CancellationToken cancellationToken)
#endif
        {
            if (this._commands.Count == 0) return null;

            if (this._tCall is ClientSideCachingCall clientSideCachingCall && this._clientSideCachingPipePlaceholder != null)
            {
                if (clientSideCachingCall.ClientSideCache.Mode is ClientSideCachingMode.Broadcasting)
                {
                    var pipelining = this.BroadcastingPipelining(clientSideCachingCall);
                    var dataArray = this._tCall.Calls(pipelining.Commands, cancellationToken);
                    return this.AssembledBroadcastingPipelining(clientSideCachingCall, dataArray, pipelining.CacheDatas);
                }

                if (clientSideCachingCall.ClientSideCache.Mode is ClientSideCachingMode.Default)
                {
                    var pipelining = this.DefaultPipelining(clientSideCachingCall);
                    var dataArray = this._tCall.Calls(pipelining.Commands, cancellationToken);
                    return this.AssembledDefaultPipelining(clientSideCachingCall, dataArray, pipelining.CacheDatas);
                }
            }

            {
                var dataArray = this._tCall.Calls(this._commands.ToArray(), cancellationToken);
                if (dataArray is null || dataArray.Length <= 0) return null;
                return this.ToResult(dataArray);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        internal sealed override object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            PipeliningCall.CommandNonsupportException(command);
            this._commands.Add(command);
            this._commandResultTypes.Add(resultType);
            this._conditions.Add(null);
            this._enumTypes.Add(null);
            return null;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override object?[]? Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override object[] Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            throw new NotSupportedException($"The pipeline does not support the Calls method");
        }

        internal sealed override bool CallCondition(CommandPacket command, string condition, CancellationToken cancellationToken)
        {
            PipeliningCall.CommandNonsupportException(command);
            this._commands.Add(command);
            this._commandResultTypes.Add(ResultType.String);
            this._conditions.Add(condition);
            this._enumTypes.Add(null);
            return false;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override bool[]? CallConditionArray(CommandPacket command, string condition, CancellationToken cancellationToken)
#else
        internal sealed override bool[] CallConditionArray(CommandPacket command, string condition, CancellationToken cancellationToken)
#endif
        {
            PipeliningCall.CommandNonsupportException(command);
            this._commands.Add(command);
            this._commandResultTypes.Add(ResultType.String | ResultType.Array);
            this._conditions.Add(condition);
            this._enumTypes.Add(null);
            return null;
        }

        internal sealed override TEnum CallEnum<TEnum>(CommandPacket command, CancellationToken cancellationToken)
        {
            PipeliningCall.CommandNonsupportException(command);
            this._commands.Add(command);
            this._commandResultTypes.Add(ResultType.Enum);
            this._conditions.Add(null);
            this._enumTypes.Add(typeof(TEnum));
            return default;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override TEnum[]? CallEnumArray<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#else
        internal sealed override TEnum[] CallEnumArray<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            PipeliningCall.CommandNonsupportException(command);
            this._commands.Add(command);
            this._commandResultTypes.Add(ResultType.Array | ResultType.Enum);
            this._conditions.Add(null);
            this._enumTypes.Add(typeof(TEnum));
            return default;
        }
        #endregion

        #region Async
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal async Task<object?[]?> ExecPipeliningAsync(CancellationToken cancellationToken)
#else
        internal async Task<object[]> ExecPipeliningAsync(CancellationToken cancellationToken)
#endif
        {
            if (this._commands.Count == 0) return null;

            if (this._tCall is ClientSideCachingCall clientSideCachingCall && this._clientSideCachingPipePlaceholder != null)
            {
                if (clientSideCachingCall.ClientSideCache.Mode is ClientSideCachingMode.Broadcasting)
                {
                    var pipelining = this.BroadcastingPipelining(clientSideCachingCall);
                    var dataArray = await this._tCall.CallsAsync(pipelining.Commands, cancellationToken).ConfigureAwait(false);
                    return this.AssembledBroadcastingPipelining(clientSideCachingCall, dataArray, pipelining.CacheDatas);
                }

                if (clientSideCachingCall.ClientSideCache.Mode is ClientSideCachingMode.Default)
                {
                    var pipelining = this.DefaultPipelining(clientSideCachingCall);
                    var dataArray = await this._tCall.CallsAsync(pipelining.Commands, cancellationToken).ConfigureAwait(false);
                    return this.AssembledDefaultPipelining(clientSideCachingCall, dataArray, pipelining.CacheDatas);
                }
            }

            {
                var dataArray = await this._tCall.CallsAsync(this._commands.ToArray(), cancellationToken).ConfigureAwait(false);
                if (dataArray is null || dataArray.Length <= 0) return null;
                return this.ToResult(dataArray);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        internal sealed override Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            return Task.FromResult(this.Call(command, resultType, cancellationToken));
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            throw new NotSupportedException($"The pipeline does not support the Calls method");
        }

        internal sealed override Task<bool> CallConditionAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.CallCondition(command, condition, cancellationToken));
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override Task<bool[]?> CallConditionArrayAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
#else
        internal sealed override Task<bool[]> CallConditionArrayAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
#endif
        {
            return Task.FromResult(this.CallConditionArray(command, condition, cancellationToken));
        }

        internal sealed override Task<TEnum> CallEnumAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.CallEnum<TEnum>(command, cancellationToken));
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override Task<TEnum[]?> CallEnumArrayAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#else
        internal sealed override Task<TEnum[]> CallEnumArrayAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            return Task.FromResult(this.CallEnumArray<TEnum>(command, cancellationToken));
        }
#endif
#endregion

        private static void CommandNonsupportException(CommandPacket command)
        {
            if ((command.Mode & CommandMode.WithoutResult) == CommandMode.WithoutResult
                || (command.Mode & CommandMode.Sub) == CommandMode.Sub
                || (command.Mode & CommandMode.WithBlock) == CommandMode.WithBlock)
            {
                throw new NotSupportedException($"The pipeline does not support the command [{command}]");
            }
        }

        private PipeliningClientSideCaching BroadcastingPipelining(ClientSideCachingCall clientSideCachingCall)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            object?[] cacheDatas = new object?[this._commands.Count];
#else
            object[] cacheDatas = new object[this._commands.Count];
#endif
            var commandArray = new CommandPacket[this._commands.Count];
            for (int i = 0; i < this._commands.Count; i++)
            {
                if (this._commands[i].Mode != ClientSideCachingExtensions._cacheCommandMode)
                {
                    commandArray[i] = this._commands[i];
                    continue;
                }

                var isMatche = ClientSideCachingExtensions.MatcheBroadcasting(clientSideCachingCall.ClientSideCache, this._commands[i], clientSideCachingCall.KeyPrefix);
                if (isMatche)
                {
                    var cacheKey = this._commands[i].ToClientSideCacheKey(clientSideCachingCall.KeyPrefix);
                    if (clientSideCachingCall.ClientSideCache.TryGet(in cacheKey, out var cacheValue))
                    {
                        cacheDatas[i] = cacheValue;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        commandArray[i] = this._echoOk!;
#else
                        commandArray[i] = this._echoOk;
#endif
                        continue;
                    }
                    cacheDatas[i] = cacheKey;
                }
                commandArray[i] = this._commands[i];
            }

            return new PipeliningClientSideCaching(cacheDatas, commandArray);
        }

        private PipeliningClientSideCaching DefaultPipelining(ClientSideCachingCall clientSideCachingCall)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            object?[] cacheDatas = new object?[this._commands.Count];
#else
            object[] cacheDatas = new object[this._commands.Count];
#endif
            var commandList = new List<CommandPacket>(this._commands.Count * 2);
            for (int i = 0; i < this._commands.Count; i++)
            {
                if (this._commands[i].Mode != ClientSideCachingExtensions._cacheCommandMode)
                {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    commandList.Add(this._echoOk!);
#else
                    commandList.Add(this._echoOk);
#endif
                    commandList.Add(this._commands[i]);
                    continue;
                }

                var matchType = ClientSideCachingExtensions.MatcheDefault(clientSideCachingCall.ClientSideCache, this._commands[i], clientSideCachingCall.KeyPrefix);
                switch (matchType)
                {
                    case ClientSideCachingDefaultMatchType.Include:
                        {
                            var cacheKey = this._commands[i].ToClientSideCacheKey(clientSideCachingCall.KeyPrefix);
                            if (clientSideCachingCall.ClientSideCache.TryGet(in cacheKey, out var cacheValue))
                            {
                                cacheDatas[i] = cacheValue;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                commandList.Add(this._echoOk!);
                                commandList.Add(this._echoOk!);
#else
                                commandList.Add(this._echoOk);
                                commandList.Add(this._echoOk);
#endif
                                continue;
                            }

                            cacheDatas[i] = cacheKey;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            commandList.Add(this._cachingYesCommand!);
#else
                            commandList.Add(this._cachingYesCommand);
#endif
                            break;
                        }
                    case ClientSideCachingDefaultMatchType.Exclude:
                        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            commandList.Add(this._cachingNoCommand!);
#else
                            commandList.Add(this._cachingNoCommand);
#endif
                            break;
                        }
                    case ClientSideCachingDefaultMatchType.Include | ClientSideCachingDefaultMatchType.Unmatch:
                        {
                            var cacheKey = this._commands[i].ToClientSideCacheKey(clientSideCachingCall.KeyPrefix);
                            if (clientSideCachingCall.ClientSideCache.TryGet(in cacheKey, out var cacheValue))
                            {
                                cacheDatas[i] = cacheValue;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                commandList.Add(this._echoOk!);
                                commandList.Add(this._echoOk!);
#else
                                commandList.Add(this._echoOk);
                                commandList.Add(this._echoOk);
#endif
                                continue;
                            }

                            cacheDatas[i] = cacheKey;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            commandList.Add(this._echoOk!);
#else
                            commandList.Add(this._echoOk);
#endif
                            break;
                        }
                    case ClientSideCachingDefaultMatchType.Unmatch:
                    default:
                        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            commandList.Add(this._echoOk!);
#else
                            commandList.Add(this._echoOk);
#endif
                            break;
                        }
                }
                commandList.Add(this._commands[i]);
            }

            var result = new PipeliningClientSideCaching(cacheDatas, commandList.ToArray());
            commandList.Clear();
            commandList.Capacity = 0;
            return result;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private object?[]? AssembledBroadcastingPipelining(ClientSideCachingCall clientSideCachingCall, object?[]? dataArray, object?[] cacheDatas)
#else
        private object[] AssembledBroadcastingPipelining(ClientSideCachingCall clientSideCachingCall, object[] dataArray, object[] cacheDatas)
#endif
        {
            if (dataArray is null || dataArray.Length <= 0) return null;
            for (int i = 0; i < cacheDatas.Length; i++)
            {
                if (cacheDatas[i] != null)
                {
                    if (cacheDatas[i] is ClientSideCacheKey cacheKey)
                    {
                        var itemData = ConvertExtensions.To<object>(dataArray[i], this._commandResultTypes[i], base.Encoding);
                        if (itemData != null)
                        {
                            clientSideCachingCall.ClientSideCache.WholeSet(in cacheKey, itemData);
                        }
                        dataArray[i] = itemData;
                        continue;
                    }

                    if (dataArray[i] is byte[] bytes && System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(bytes, this._clientSideCachingPipePlaceholder))
                    {
                        dataArray[i] = cacheDatas[i];
#if DEBUG
                        SharpConsole.WriteInfo($"Broadcasting mode, Pipe index: {i}, KeyPrefixes: [{(clientSideCachingCall.ClientSideCache.KeyPrefixes is null ? "NULL" : string.Join(",", clientSideCachingCall.ClientSideCache.KeyPrefixes))}], cache");
#endif
                        continue;
                    }
                }
            }

            return this.ToResult(dataArray);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private object?[]? AssembledDefaultPipelining(ClientSideCachingCall clientSideCachingCall, object?[]? dataArray, object?[] cacheDatas)
#else
        private object[] AssembledDefaultPipelining(ClientSideCachingCall clientSideCachingCall, object[] dataArray, object[] cacheDatas)
#endif
        {
            if (dataArray is null || dataArray.Length <= 0) return null;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            var result = new object?[cacheDatas.Length];
#else
            var result = new object[cacheDatas.Length];
#endif
            for (int i = 0; i < cacheDatas.Length; i++)
            {
                var dataItem = dataArray[i * 2 + 1];
                if (cacheDatas[i] != null)
                {
                    if (cacheDatas[i] is ClientSideCacheKey cacheKey)
                    {
                        var itemData = ConvertExtensions.To<object>(dataItem, this._commandResultTypes[i], base.Encoding);
                        if (itemData != null)
                        {
                            clientSideCachingCall.ClientSideCache.WholeSet(in cacheKey, itemData);
                        }
                        result[i] = itemData;
                        continue;
                    }

                    if (dataItem is byte[] bytes && System.Collections.StructuralComparisons.StructuralEqualityComparer.Equals(bytes, this._clientSideCachingPipePlaceholder))
                    {
#if DEBUG
                        if ((clientSideCachingCall.ClientSideCache.KeyPatterns is null || clientSideCachingCall.ClientSideCache.KeyPatterns.Length is 0)
                            && (clientSideCachingCall.ClientSideCache.WithoutKeyPatterns is null || clientSideCachingCall.ClientSideCache.WithoutKeyPatterns.Length is 0))
                        {
                            SharpConsole.WriteInfo($"Default mode, Pipe index: {i}, KeyPrefixes: [NULL], cache");
                        }
                        else if (clientSideCachingCall.ClientSideCache.KeyPatterns?.Length > 0)
                        {
                            SharpConsole.WriteInfo($"Default mode, Pipe index: {i}, KeyPatterns: [{(clientSideCachingCall.ClientSideCache.KeyPatterns is null ? "NULL" : string.Join(",", clientSideCachingCall.ClientSideCache.KeyPatterns))}], cache");
                        }
                        else if (clientSideCachingCall.ClientSideCache.WithoutKeyPatterns?.Length > 0)
                        {
                            SharpConsole.WriteInfo($"Default mode, Pipe index: {i}, WithoutKeyPatterns: [{(clientSideCachingCall.ClientSideCache.WithoutKeyPatterns is null ? "NULL" : string.Join(",", clientSideCachingCall.ClientSideCache.WithoutKeyPatterns))}], cache");
                        }
#endif
                        result[i] = cacheDatas[i];
                        continue;
                    }
                }

                result[i] = dataItem;
            }

            return this.ToResult(result);
        }

        #region ToResult method
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private object?[] ToResult(object?[] dataArray)
        {
            var result = new object?[dataArray.Length];
#else
        private object[] ToResult(object[] dataArray)
        {
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
            return result;
        }
        #endregion

        #region Dispose
        protected sealed override void Dispose(bool disposing)
        {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            if (!base._disposedValue)
            {
                base._disposedValue = true;
                if (disposing)
                {
                    this._commands?.Clear();
                    this._commandResultTypes?.Clear();
                    this._conditions?.Clear();
                    this._enumTypes?.Clear();
                }
            }
            this._tCall = null;
            this._commands = null;
            this._commandResultTypes = null;
            this._conditions = null;
            this._enumTypes = null;
            this._clientSideCachingPipePlaceholder = null;
            this._echoOk = null;
            this._cachingYesCommand = null;
            this._cachingNoCommand = null;
            base.Dispose(disposing);
        }

        ~PipeliningCall()
        {
            this.Dispose(true);
        }
        #endregion

        private readonly struct PipeliningClientSideCaching
        {
            private readonly CommandPacket[] _commands;

            internal CommandPacket[] Commands => this._commands;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            private readonly object?[] _cacheDatas;

            internal object?[] CacheDatas => this._cacheDatas;

            internal PipeliningClientSideCaching(object?[] cacheDatas, CommandPacket[] commands)
#else
            private readonly object[] _cacheDatas;

            internal object[] CacheDatas => this._cacheDatas;

            internal PipeliningClientSideCaching(object[] cacheDatas, CommandPacket[] commands)
#endif
            {
                this._cacheDatas = cacheDatas;
                this._commands = commands;
            }
        }
    }
}
