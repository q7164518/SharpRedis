using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using SharpRedis.Network;
using SharpRedis.Provider.Standard;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines.Middlewares.ClientCaching;

internal sealed class ClientCachingDefaultMiddleware : IMiddleware
{
    public const string NAME = "ClientCachingDefault";

    private readonly RedisContext _context;
    private ClientCachingBase _clientCaching;
    private CacheKeyInvoker _cacheKeyInvoker;
    private Encoding _encoding;
    private bool _disposedValue;

    public string Name => ClientCachingDefaultMiddleware.NAME;

    internal ClientCachingDefaultMiddleware(RedisContext context, ClientCachingBase clientCaching)
    {
        this._context = context;
        this._clientCaching = clientCaching;
        this._encoding = context.RedisOptions.Encoding;
        string keyPrefix = string.Empty, keySuffix = string.Empty;
        if (!string.IsNullOrEmpty(context.RedisOptions.Prefix))
        {
            keyPrefix = context.RedisOptions.Prefix;
        }
        if (!string.IsNullOrEmpty(context.RedisOptions.Suffix))
        {
            keySuffix = context.RedisOptions.Suffix;
        }
        this._cacheKeyInvoker = new(this._encoding, keyPrefix, keySuffix);
    }

    RedisResult IMiddleware.Invoke(ICommand command, BaseConnection connection, RedisDelegate next, CancellationToken cancellationToken)
    {
        long redirectId = connection.RedirectConnectionId;
        if (!this._context.ConnectionPool.IsCacheReady(connection, redirectId)) return next.Invoke(command, connection, cancellationToken);
        if (command.IsBatch)
        {
            var (runCommand, cacheResult) = this.PipeliningRun(command, connection, redirectId);
            using var runCommandOwner = runCommand;
            if (runCommand.ResultCount > 0)
            {
                var result = next.Invoke(runCommand, connection, cancellationToken);
                return this.MatchResult(runCommand, ref result, cacheResult, connection, redirectId);
            }

            return new RedisResult(RespType.Array, cacheResult.Select(static f => f.cache).ToArray(), this._encoding);
        }
        else
        {
            if ((command.Mode & Config.WITH_CLIENT_CACHING_MODE) == Config.WITH_CLIENT_CACHING_MODE
                && (command.Mode & CommandMode.Transaction) != CommandMode.Transaction // Transactions do not use client-side caching.
                && this.MatchKey(command))
            {
                string cacheKey = this._cacheKeyInvoker.GetCacheKey(command);
                //try get cache
                if (this.TryGetCache(cacheKey, connection, redirectId, out var cache)) return cache.Value;

                using var commandBatch = command.EnableCachingYes(); // insert [CLIENT CACHING YES] command
                var result = next.Invoke(commandBatch, connection, cancellationToken);
                return this.SetCache(command, cacheKey, ref result, false, connection, redirectId);
            }
            else
            {
                return next.Invoke(command, connection, cancellationToken);
            }
        }
    }

    async Task<RedisResult> IMiddleware.InvokeAsync(ICommand command, BaseConnection connection, AsyncRedisDelegate next, CancellationToken cancellationToken)
    {
        long redirectId = connection.RedirectConnectionId;
        if (!this._context.ConnectionPool.IsCacheReady(connection, redirectId)) return await next.Invoke(command, connection, cancellationToken).ConfigureAwait(false);
        if (command.IsBatch)
        {
            var (runCommand, cacheResult) = this.PipeliningRun(command, connection, redirectId);
            using var runCommandOwner = runCommand;
            if (runCommand.ResultCount > 0)
            {
                var result = await next.Invoke(runCommand, connection, cancellationToken).ConfigureAwait(false);
                return this.MatchResult(runCommand, ref result, cacheResult, connection, redirectId);
            }

            return new RedisResult(RespType.Array, cacheResult.Select(static f => f.cache).ToArray(), this._encoding);
        }
        else
        {
            if ((command.Mode & Config.WITH_CLIENT_CACHING_MODE) == Config.WITH_CLIENT_CACHING_MODE
                && (command.Mode & CommandMode.Transaction) != CommandMode.Transaction // Transactions do not use client-side caching.
                && this.MatchKey(command))
            {
                string cacheKey = this._cacheKeyInvoker.GetCacheKey(command);
                //try get cache
                if (this.TryGetCache(cacheKey, connection, redirectId, out var cache)) return cache.Value;

                using var commandBatch = command.EnableCachingYes(); // insert [CLIENT CACHING YES] command
                var result = await next.Invoke(commandBatch, connection, cancellationToken).ConfigureAwait(false);
                return this.SetCache(command, cacheKey, ref result, false, connection, redirectId);
            }
            else
            {
                return await next.Invoke(command, connection, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private void StoreCache(ICommand command, string key, in RedisResult value, BaseConnection connection, long redirectId)
    {
        // Do not repopulate caches from a request started before subscriber replacement.
        if (!this._context.ConnectionPool.IsCacheReady(connection, redirectId)) return;
        this._clientCaching.SetCache(command, key, in value, this._cacheKeyInvoker);
        if (!this._context.ConnectionPool.IsCacheReady(connection, redirectId)) this._clientCaching.ClearCache();
    }

    private bool MatchKey(ICommand command)
    {
        foreach (var item in command.Items)
        {
            if (item.Category != CommandItemCategory.Key) continue;
            if (!this._clientCaching.KeyMatching(this._cacheKeyInvoker.FullKey(in item))) return false;
        }
        return true;
    }

    private RedisResult SetCache(ICommand command, string cacheKey, ref readonly RedisResult result, bool firsthand, BaseConnection connection, long redirectId)
    {
        if (result.Type is RespType.Exception or RespType.SimpleErrors or RespType.BulkErrors) return result;

        if (firsthand)
        {
            if (result.Type is not RespType.BulkErrors and not RespType.SimpleErrors and not RespType.Exception)
            {
                this.StoreCache(command, cacheKey, in result, connection, redirectId);
            }
            return result;
        }
        if (result.RawValue is RedisResult[] { Length: 2 } arr)
        {
            var cachingResult = arr[0];
            var commandResult = arr[1];
            if (cachingResult.HasValue && cachingResult)
            {
                if (commandResult.Type is not RespType.BulkErrors and not RespType.SimpleErrors and not RespType.Exception)
                {
                    this.StoreCache(command, cacheKey, in commandResult, connection, redirectId);
                }
            }
            return commandResult;
        }
        return new RedisResult(RespType.Exception, new FormatException("The return value cannot be parsed."), Encoding.UTF8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    private bool TryGetCache(string cacheKey, BaseConnection connection, long redirectId, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out RedisResult? cache)
#else
    private bool TryGetCache(string cacheKey, BaseConnection connection, long redirectId, out RedisResult? cache)
#endif
    {
        if (this._clientCaching.TryGet(cacheKey, out var cacheValue) && cacheValue.HasValue
            && this._context.ConnectionPool.IsCacheReady(connection, redirectId))
        {
#if DEBUG
            SharpConsole.WriteSucceed($"[Default mode] --> Hit the client cache, Command: {cacheKey}");
#endif
            cache = cacheValue.Value;
            return true;
        }
        cache = null;
        return false;
    }

    private (CommandBatch runCommand, (string cacheKey, bool isMatch, bool hasCache, RedisResult cache)[] cacheResult) PipeliningRun(ICommand command, BaseConnection connection, long redirectId)
    {
        var runCommand = new CommandBatch();
        try
        {
            var cacheResult = new (string cacheKey, bool isMatch, bool hasCache, RedisResult cache)[command.ResultCount];

            int i = -1;
            foreach (CommandPacket itemCommand in command.BatchCommands)
            {
                i++;
                if ((itemCommand.Mode & Config.WITH_CLIENT_CACHING_MODE) == Config.WITH_CLIENT_CACHING_MODE && this.MatchKey(itemCommand))
                {
                    cacheResult[i].isMatch = true;
                    var cacheKey = this._cacheKeyInvoker.GetCacheKey(itemCommand);
                    cacheResult[i].cacheKey = cacheKey;
                    if (this.TryGetCache(cacheKey, connection, redirectId, out var cache))
                    {
                        cacheResult[i].cache = cache.Value;
                        cacheResult[i].hasCache = true;
                        continue;
                    }
                }

                if (cacheResult[i].isMatch)
                {
                    runCommand.AppendCommandWithCachingYes(itemCommand);
                }
                else
                {
                    runCommand.BorrowCommand(itemCommand);
                }
            }

            return (runCommand, cacheResult);
        }
        catch
        {
            runCommand.Dispose();
            throw;
        }
    }

    private RedisResult MatchResult(
        CommandBatch runCommand,
        ref readonly RedisResult result,
        (string cacheKey, bool isMatch, bool hasCache, RedisResult cache)[] cacheResult, BaseConnection connection, long redirectId)
    {
        if (result.Type is RespType.Exception or RespType.SimpleErrors or RespType.BulkErrors) return result;

        if (result.Type != RespType.Array || result.RawValue is not RedisResult[] resultArray)
        {
            return new(RespType.Exception, new InvalidCastException("The pipeline executes commands in batches, but the returned value is not an array."), this._encoding);
        }
        var commands = runCommand.BatchCommands;
        for (int cacheIndex = 0, commandIndex = 0; cacheIndex < cacheResult.Length; cacheIndex++)
        {
            if (cacheResult[cacheIndex].hasCache) continue;
            var currentCommand = commands[commandIndex];
            var currentItems = currentCommand.Items;
            if (currentItems.Length == 3
                && currentItems[0].ToString(this._encoding) == "CLIENT"
                && currentItems[1].ToString(this._encoding) == "CACHING"
                && currentItems[2].ToString(this._encoding) == "YES")
            {
                if (cacheResult[cacheIndex].isMatch)
                {
                    var cachingResult = resultArray[commandIndex];
                    if (!cachingResult.HasValue || !cachingResult)
                    {
                        cacheResult[cacheIndex].isMatch = false;
                    }
                }
                cacheResult[cacheIndex].cache = resultArray[++commandIndex];
            }
            else
            {
                cacheResult[cacheIndex].cache = resultArray[commandIndex];
            }
            if (cacheResult[cacheIndex].isMatch && !cacheResult[cacheIndex].hasCache)
            {
                this.SetCache(commands[commandIndex], cacheResult[cacheIndex].cacheKey, ref cacheResult[cacheIndex].cache, true, connection, redirectId);
            }
            commandIndex++;
        }

        return new RedisResult(RespType.Array, cacheResult.Select(static f => f.cache).ToArray(), this._encoding);
    }

    #region Dispose
    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
                this._clientCaching?.Dispose();
                this._cacheKeyInvoker?.Dispose();
            }

            this._clientCaching = null!;
            this._encoding = null!;
            this._cacheKeyInvoker = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
