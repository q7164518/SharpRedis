#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Commands;
using SharpRedis.Extensions;
using SharpRedis.Models;
using SharpRedis.Provider.Standard;
using SharpRedis.Network.Standard;

namespace SharpRedis.Provider.Calls
{
    internal sealed class ClientSideCachingCall : BaseCall
    {
        /// <summary>
        /// CLIENT CACHING YES
        /// </summary>
        private CommandPacket _cachingYesCommand = ConnectionCommands.ClientCaching(true);

        /// <summary>
        /// CLIENT CACHING NO
        /// </summary>
        private CommandPacket _cachingNoCommand = ConnectionCommands.ClientCaching(false);

        private BaseCall _tCall;
        private ClientSideCachingStandard _clientSideCache;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private string? _keyPrefix;

        internal string? KeyPrefix => this._keyPrefix;
#else
        private string _keyPrefix;

        internal string KeyPrefix => this._keyPrefix;
#endif

        sealed internal override bool SubUsable => this._tCall.SubUsable;

        sealed internal override string CallMode => this._tCall.CallMode;

        internal ClientSideCachingStandard ClientSideCache => this._clientSideCache;

        internal ClientSideCachingCall(IConnectionPool connectionPool, BaseCall tCall, ClientSideCachingStandard clientSideCache)
            : base(connectionPool)
        {
            this._tCall = tCall;
            this._clientSideCache = clientSideCache;
            if (connectionPool.KeyPrefix?.Length > 0)
            {
                this._keyPrefix = connectionPool.Encoding.GetString(connectionPool.KeyPrefix);
            }
        }

        #region Sync
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        internal sealed override object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            if (command.Mode == ClientSideCachingExtensions._cacheCommandMode)
            {
                if (this._clientSideCache.Mode is ClientSideCachingMode.Default)
                {
                    return this.DefaultMode(command, resultType, cancellationToken);
                }
                else if (this._clientSideCache.Mode is ClientSideCachingMode.Broadcasting)
                {
                    return this.BroadcastingMode(command, resultType, cancellationToken);
                }
            }
            return this._tCall.Call(command, resultType, cancellationToken);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override object?[]? Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override object[] Calls(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            return this._tCall.Calls(commands, cancellationToken);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private object? DefaultMode(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        private object DefaultMode(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            var cacheKey = command.ToClientSideCacheKey(this._keyPrefix);
            var matchType = ClientSideCachingExtensions.MatcheDefault(this._clientSideCache, command, this._keyPrefix);
            switch (matchType)
            {
                case ClientSideCachingDefaultMatchType.Include:
                    {
                        if (this._clientSideCache.TryGet(in cacheKey, out var cacheValue))
                        {
#if DEBUG
                            SharpConsole.WriteInfo($"Default mode, KeyPatterns: [{(this._clientSideCache.KeyPatterns is null ? "NULL" : string.Join(",", this._clientSideCache.KeyPatterns))}], cache");
#endif
                            return cacheValue;
                        }
                        var commands = new CommandPacket[] { this._cachingYesCommand, command };
                        var redisResult = this.Calls(commands, cancellationToken);
                        if (redisResult?.Length is 2)
                        {
                            var result = ConvertExtensions.To<object>(redisResult[1], resultType, base.Encoding);
                            if (ConvertExtensions.To<string>(redisResult[0], ResultType.String, base.Encoding) == "OK" && result != null)
                            {
                                this._clientSideCache.WholeSet(in cacheKey, result);
                            }
                            return result;
                        }
                        throw new RedisException("Client cache tracing failed");
                    }
                case ClientSideCachingDefaultMatchType.Exclude:
                    {
                        var commands = new CommandPacket[] { this._cachingNoCommand, command };
                        var redisResult = this.Calls(commands, cancellationToken);
                        if (redisResult?.Length is 2)
                        {
                            return redisResult[1];
                        }
                        throw new RedisException("Client cache tracing failed");
                    }
                case ClientSideCachingDefaultMatchType.Include | ClientSideCachingDefaultMatchType.Unmatch:
                    {
                        if (this._clientSideCache.TryGet(in cacheKey, out var cacheValue))
                        {
#if DEBUG
                            SharpConsole.WriteInfo($"Default mode, WithoutKeyPatterns: [{(this._clientSideCache.WithoutKeyPatterns is null ? "NULL" : string.Join(",", this._clientSideCache.WithoutKeyPatterns))}], cache");
#endif
                            return cacheValue;
                        }
                        var redisResult = this._tCall.Call(command, resultType, cancellationToken);
                        var result = ConvertExtensions.To<object>(redisResult, resultType, base.Encoding);
                        if (result != null)
                        {
                            this._clientSideCache.WholeSet(in cacheKey, result);
                        }
                        return result;
                    }
                case ClientSideCachingDefaultMatchType.Unmatch:
                default:
                    return this._tCall.Call(command, resultType, cancellationToken);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private object? BroadcastingMode(CommandPacket command, in ResultType resultType, CancellationToken cancellationToken)
#else
        private object BroadcastingMode(CommandPacket command, in ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            var cacheKey = command.ToClientSideCacheKey(this._keyPrefix);
            var isMatche = ClientSideCachingExtensions.MatcheBroadcasting(this._clientSideCache, command, this._keyPrefix);
            if (isMatche)
            {
                if (this._clientSideCache.TryGet(in cacheKey, out var cacheValue))
                {
#if DEBUG
                    SharpConsole.WriteInfo($"Broadcasting mode, KeyPrefixes: [{(this._clientSideCache.KeyPrefixes is null ? "NULL" : string.Join(",", this._clientSideCache.KeyPrefixes))}], cache");
#endif
                    return cacheValue;
                }
                var redisResult = this._tCall.Call(command, resultType, cancellationToken);
                var result = ConvertExtensions.To<object>(redisResult, resultType, base.Encoding);
                if (result != null)
                {
                    this._clientSideCache.WholeSet(in cacheKey, result);
                }
                return result;
            }
            return this._tCall.Call(command, resultType, cancellationToken);
        }
        #endregion

        #region Async
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        internal sealed override Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            if (command.Mode == ClientSideCachingExtensions._cacheCommandMode)
            {
                if (this._clientSideCache.Mode is ClientSideCachingMode.Default)
                {
                    return this.DefaultModeAsync(command, resultType, cancellationToken);
                }
                else if (this._clientSideCache.Mode is ClientSideCachingMode.Broadcasting)
                {
                    return this.BroadcastingModeAsync(command, resultType, cancellationToken);
                }
            }
            return this._tCall.CallAsync(command, resultType, cancellationToken);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal sealed override Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#else
        internal sealed override Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken)
#endif
        {
            return this._tCall.CallsAsync(commands, cancellationToken);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private async Task<object?> DefaultModeAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        private async Task<object> DefaultModeAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            var cacheKey = command.ToClientSideCacheKey(this._keyPrefix);
            var matchType = ClientSideCachingExtensions.MatcheDefault(this._clientSideCache, command, this._keyPrefix);
            switch (matchType)
            {
                case ClientSideCachingDefaultMatchType.Include:
                    {
                        if (this._clientSideCache.TryGet(in cacheKey, out var cacheValue))
                        {
#if DEBUG
                            SharpConsole.WriteInfo($"Default mode, KeyPatterns: [{(this._clientSideCache.KeyPatterns is null ? "NULL" : string.Join(",", this._clientSideCache.KeyPatterns))}], cache");
#endif
                            return cacheValue;
                        }
                        var commands = new CommandPacket[] { this._cachingYesCommand, command };
                        var redisResult = await this.CallsAsync(commands, cancellationToken).ConfigureAwait(false);
                        if (redisResult?.Length is 2)
                        {
                            var result = ConvertExtensions.To<object>(redisResult[1], resultType, base.Encoding);
                            if (ConvertExtensions.To<string>(redisResult[0], ResultType.String, base.Encoding) == "OK" && result != null)
                            {
                                this._clientSideCache.WholeSet(in cacheKey, result);
                            }
                            return result;
                        }
                        throw new RedisException("Client cache tracing failed");
                    }
                case ClientSideCachingDefaultMatchType.Exclude:
                    {
                        var commands = new CommandPacket[] { this._cachingNoCommand, command };
                        var redisResult = await this.CallsAsync(commands, cancellationToken).ConfigureAwait(false);
                        if (redisResult?.Length is 2)
                        {
                            return redisResult[1];
                        }
                        throw new RedisException("Client cache tracing failed");
                    }
                case ClientSideCachingDefaultMatchType.Include | ClientSideCachingDefaultMatchType.Unmatch:
                    {
                        if (this._clientSideCache.TryGet(in cacheKey, out var cacheValue))
                        {
#if DEBUG
                            SharpConsole.WriteInfo($"Default mode, WithoutKeyPatterns: [{(this._clientSideCache.WithoutKeyPatterns is null ? "NULL" : string.Join(",", this._clientSideCache.WithoutKeyPatterns))}], cache");
#endif
                            return cacheValue;
                        }
                        var redisResult = await this._tCall.CallAsync(command, resultType, cancellationToken).ConfigureAwait(false);
                        var result = ConvertExtensions.To<object>(redisResult, resultType, base.Encoding);
                        if (result != null)
                        {
                            this._clientSideCache.WholeSet(in cacheKey, result);
                        }
                        return result;
                    }
                case ClientSideCachingDefaultMatchType.Unmatch:
                default:
                    return await this._tCall.CallAsync(command, resultType, cancellationToken).ConfigureAwait(false);
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private async Task<object?> BroadcastingModeAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#else
        private async Task<object> BroadcastingModeAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken)
#endif
        {
            var cacheKey = command.ToClientSideCacheKey(this._keyPrefix);
            var isMatche = ClientSideCachingExtensions.MatcheBroadcasting(this._clientSideCache, command, this._keyPrefix);
            if (isMatche)
            {
                if (this._clientSideCache.TryGet(in cacheKey, out var cacheValue))
                {
#if DEBUG
                    SharpConsole.WriteInfo($"Broadcasting mode, KeyPrefixes: [{(this._clientSideCache.KeyPrefixes is null ? "NULL" : string.Join(",", this._clientSideCache.KeyPrefixes))}], cache");
#endif
                    return cacheValue;
                }
                var redisResult = await this._tCall.CallAsync(command, resultType, cancellationToken).ConfigureAwait(false);
                var result = ConvertExtensions.To<object>(redisResult, resultType, base.Encoding);
                if (result != null)
                {
                    this._clientSideCache.WholeSet(in cacheKey, result);
                }
                return result;
            }
            return await this._tCall.CallAsync(command, resultType, cancellationToken).ConfigureAwait(false);
        }
#endif
        #endregion

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
                    this._tCall.Dispose();
                    this._clientSideCache?.Dispose();
                }
            }

            this._tCall = null;
            this._clientSideCache = null;
            this._keyPrefix = null;
            this._cachingYesCommand = null;
            this._cachingNoCommand = null;
            base.Dispose(disposing);
        }

        ~ClientSideCachingCall()
        {
            this.Dispose(true);
        }
    }
}
