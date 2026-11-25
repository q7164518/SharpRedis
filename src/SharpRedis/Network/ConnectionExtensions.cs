using SharpRedis.Commands;
using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Exceptions;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;
using System.Threading.Tasks;

namespace SharpRedis.Network;

internal enum HeartbeatResult : byte
{
    Succeeded,
    Deferred,
    Failed,
}

internal static class ConnectionExtensions
{
    internal static void CommandEnqueue(this BaseConnection connection, ICommand command,
        TaskCompletionSource<RedisResult> resultTask, int millisecondsTimeout)
    {
        command.TimeoutAt = millisecondsTimeout == System.Threading.Timeout.Infinite
            ? DateTime.MaxValue : DateTime.UtcNow.AddMilliseconds(millisecondsTimeout);
        connection.CommandEnqueue(command, resultTask);
    }

    internal static bool WaitForCompletion(this TaskCompletionSource<RedisResult> resultTask, int millisecondsTimeout)
        => resultTask.Task.Wait(millisecondsTimeout)
            && resultTask.Task.GetAwaiter().GetResult().RawValue is not TimeoutException;

#if MODERN_NET
    internal static bool Authentication(this BaseConnection connection, string? username, string? password, Resp respVer, int dbIndex, int millisecondsTimeout)
#else
    internal static bool Authentication(this BaseConnection connection, string username, string password, Resp respVer, int dbIndex, int millisecondsTimeout)
#endif
    {
        if (respVer is Resp.RESP2)
        {
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                using var command = ConnectionCommands.Auth(username, password);
                TaskCompletionSource<RedisResult> resultTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
                connection.CommandEnqueue(command, resultTask, millisecondsTimeout);

                var ok = resultTask.WaitForCompletion(millisecondsTimeout);
                if (!ok) throw new RedisInitializationException("Redis authentication timeout.");
                var result = resultTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
                if (result.RawValue is TimeoutException or OperationCanceledException)
                {
                    throw new RedisInitializationException("Redis authentication timeout.");
                }
                else if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
                var succeed = result.ToBoolean();
                if (!succeed) return false;
            }

            //set client name
            using var nameCommand = ConnectionCommands.ClientSetName(connection.ConnectionName);
            TaskCompletionSource<RedisResult> setNameTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
            connection.CommandEnqueue(nameCommand, setNameTask, millisecondsTimeout);

            var setNameOk = setNameTask.WaitForCompletion(millisecondsTimeout);
            if (!setNameOk) throw new RedisInitializationException("Set the timeout for the Redis connection name.");
        }
        else
        {
            using var helloCommand = ConnectionCommands.Hello(Resp.RESP3, username, password, connection.ConnectionName);
            TaskCompletionSource<RedisResult> helloTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
            connection.CommandEnqueue(helloCommand, helloTask, millisecondsTimeout);
            var ok = helloTask.WaitForCompletion(millisecondsTimeout);
            if (!ok) throw new RedisInitializationException("Redis authentication exception.");
            var result = helloTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
            if (result.RawValue is TimeoutException or OperationCanceledException)
            {
                throw new RedisInitializationException("Redis authentication timeout.");
            }
            else if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
            if (result.Type is RespType.SimpleErrors or RespType.BulkErrors)
                throw new RedisInitializationException("HELLO failed: " + result.RawValue);
        }

        connection.ClientId = connection.ClientId(millisecondsTimeout);
        connection.SetLibName(millisecondsTimeout);
        connection.SetLibVersion(millisecondsTimeout);
        // Fresh data connections start in DB 0. Sentinel does not implement SELECT.
        return dbIndex == 0 || connection.SelectDatabase(dbIndex, millisecondsTimeout);
    }

    internal static bool SelectDatabase(this BaseConnection connection, int dbIndex, int millisecondsTimeout)
    {
        using var command = ConnectionCommands.Select(dbIndex);
        TaskCompletionSource<RedisResult> indexTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, indexTask, millisecondsTimeout);
        var ok = indexTask.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("Select database exception.");
        var result = indexTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is TimeoutException or OperationCanceledException)
        {
            throw new RedisInitializationException("Select database timeout.");
        }
        else if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        return result;
    }

    internal static HeartbeatResult Ping(this BaseConnection connection, int millisecondsTimeout)
    {
        var pingValue = Guid.NewGuid().ToString();
        using var command = ConnectionCommands.Ping(pingValue);
        HeartbeatExecution pingTask = new();
        connection.CommandEnqueue(command, pingTask, millisecondsTimeout);
        // Give the server its own reply window once the dispatcher has written the batch.
        if (!Task.WhenAny(pingTask.Task, pingTask.Sent.Task).Wait(millisecondsTimeout))
        {
            return HeartbeatResult.Deferred;
        }
        if (!pingTask.Task.Wait(millisecondsTimeout)) return HeartbeatResult.Failed;

        var result = pingTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is TimeoutException) return HeartbeatResult.Deferred;
        return result.ToString() == pingValue ? HeartbeatResult.Succeeded : HeartbeatResult.Failed;
    }

    internal static long ClientId(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = ConnectionCommands.ClientId();
        TaskCompletionSource<RedisResult> clientIdTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, clientIdTask, millisecondsTimeout);
        var ok = clientIdTask.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("Get client ID timeout.");
        var result = clientIdTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is Exception) return -1;
        return result;
    }

    internal static void SetLibName(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = ConnectionCommands.ClientSetInfo(RedisConnection.SetInfoCategory.LibName, Config.LIB_NAME);
        TaskCompletionSource<RedisResult> setLibNameTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, setLibNameTask, millisecondsTimeout);
        var ok = setLibNameTask.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("Set client LIB-NAME timeout.");
        _ = setLibNameTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    internal static void SetLibVersion(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = ConnectionCommands.ClientSetInfo(RedisConnection.SetInfoCategory.LibVersion, Utilities.GetVersion());
        TaskCompletionSource<RedisResult> setLibVersionTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, setLibVersionTask, millisecondsTimeout);
        var ok = setLibVersionTask.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("Set client LIB-VER timeout.");
        _ = setLibVersionTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
    }

    internal static void Unwatch(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = TransactionCommands.Unwatch();
        TaskCompletionSource<RedisResult> task = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, task, millisecondsTimeout);
        var ok = task.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("UNWATCH timeout.");
        var result = task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is TimeoutException or OperationCanceledException)
        {
            throw new RedisInitializationException("UNWATCH timeout.");
        }
        else if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        if (result.Type is RespType.SimpleErrors or RespType.BulkErrors or RespType.Exception)
            throw new RedisInitializationException("UNWATCH failed: " + result.RawValue);
    }

    internal static void Multi(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = TransactionCommands.Multi();
        TaskCompletionSource<RedisResult> task = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, task, millisecondsTimeout);
        var ok = task.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("MULTI timeout.");
        var result = task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is TimeoutException or OperationCanceledException)
        {
            throw new RedisInitializationException("MULTI timeout.");
        }
        else if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        if (result.Type is RespType.SimpleErrors or RespType.BulkErrors or RespType.Exception)
            throw new RedisInitializationException("MULTI failed: " + result.RawValue);
    }

    internal static RedisCluster.SlotInfo[] ClusterSlots(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = ClusterCommands.Slots();
        TaskCompletionSource<RedisResult> task = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, task, millisecondsTimeout);
        if (!task.WaitForCompletion(millisecondsTimeout)) throw new RedisInitializationException("CLUSTER SLOTS timeout.");
        var result = task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        if (result.Type is RespType.SimpleErrors or RespType.BulkErrors or RespType.Exception)
            throw new RedisInitializationException("CLUSTER SLOTS failed: " + result.RawValue);
        return result;
    }

    internal static void Asking(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = ClusterCommands.Asking();
        TaskCompletionSource<RedisResult> task = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, task, millisecondsTimeout);
        if (!task.WaitForCompletion(millisecondsTimeout)) throw new RedisInitializationException("ASKING timeout.");
        var result = task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        if (result.Type is RespType.SimpleErrors or RespType.BulkErrors or RespType.Exception)
            throw new RedisInitializationException("ASKING failed: " + result.RawValue);
    }

    internal static void Readonly(this BaseConnection connection, int millisecondsTimeout)
    {
        using var command = ClusterCommands.Readonly();
        TaskCompletionSource<RedisResult> task = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, task, millisecondsTimeout);
        if (!task.WaitForCompletion(millisecondsTimeout)) throw new RedisInitializationException("READONLY timeout.");
        var result = task.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is Exception ex) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
        if (result.Type is RespType.SimpleErrors or RespType.BulkErrors or RespType.Exception)
            throw new RedisInitializationException("READONLY failed: " + result.RawValue);
    }

    internal static void SubscribeClientCacheInvalidate(this BaseConnection connection, ClientCachingBase clientCaching, int millisecondsTimeout)
    {
        using var command = new CommandSubscribe(PubSubCommands.Subscribe(Config.CLIENT_CACHING_SUB_CHANNEL), clientCaching.ClientCacheInvalidate);
        TaskCompletionSource<RedisResult> subscribeTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, subscribeTask, millisecondsTimeout);
        var ok = subscribeTask.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("Subscription client cache expiration channel timeout.");
        var result = subscribeTask.Task.GetAwaiter().GetResult();
        if (result.Type != RespType.Empty)
        {
            throw new RedisInitializationException($"Failed to subscribe to the client cache invalidation channel: {result.RawValue}");
        }
    }

    internal static void UseClientCaching(this BaseConnection connection, ClientCachingBase clientCaching, int millisecondsTimeout)
    {
        switch (clientCaching.Mode)
        {
            case ClientCachingBase.ClientCachingMode.Broadcast:
                connection.UseClientCachingByBroadcasting(clientCaching, millisecondsTimeout);
                break;
            case ClientCachingBase.ClientCachingMode.Default:
                connection.UseClientCachingByDefault(clientCaching, millisecondsTimeout);
                break;
            default:
                break;
        }
    }

    internal static void ResetClientCaching(this BaseConnection connection, ClientCachingBase clientCaching, int millisecondsTimeout)
    {
        // Remove existing BCAST prefixes before registering the replacement redirect.
        connection.DisableClientCaching(millisecondsTimeout);
        connection.UseClientCaching(clientCaching, millisecondsTimeout);
    }

    internal static void DisableClientCaching(this BaseConnection connection, int millisecondsTimeout)
    {
        connection.RedirectConnectionId = 0;
        using var command = ConnectionCommands.ClientTracking(false, null, null, false, false, false, false);
        TaskCompletionSource<RedisResult> resultTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, resultTask, millisecondsTimeout);
        if (!resultTask.WaitForCompletion(millisecondsTimeout))
            throw new RedisInitializationException("Disable client cache timeout");
        var result = resultTask.Task.GetAwaiter().GetResult();
        if (!result) throw new RedisInitializationException($"Failed to disable client cache, Msg: {result.RawValue}");
    }

    internal static void UseClientCachingByBroadcasting(this BaseConnection connection, ClientCachingBase clientCaching, int millisecondsTimeout)
    {
        using var command = ConnectionCommands.ClientTracking(
            onOff: true,
            redirect: clientCaching.RedirectConnectionId,
            keyPrefixes: clientCaching.KeyPrefixes,
            bcast: true,
            optin: false,
            optout: false,
            noloop: false);
        TaskCompletionSource<RedisResult> trackingTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, trackingTask, millisecondsTimeout);
        var ok = trackingTask.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("Enable client cache Broadcasting timeout");
        var result = trackingTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is Exception ex)
        {
            throw new RedisInitializationException($"Failed to start Broadcasting, Msg: {ex.Message}");
        }
        if (!result)
        {
            throw new RedisInitializationException($"Failed to enable client cache, Msg: {result.RawValue}");
        }
        connection.RedirectConnectionId = clientCaching.RedirectConnectionId;
    }

    private static void UseClientCachingByDefault(this BaseConnection connection, ClientCachingBase clientCaching, int millisecondsTimeout)
    {
        using var command = ConnectionCommands.ClientTracking(
            onOff: true,
            redirect: clientCaching.RedirectConnectionId,
            keyPrefixes: null,
            bcast: false,
            optin: true,
            optout: false,
            noloop: false);
        TaskCompletionSource<RedisResult> tracingTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.CommandEnqueue(command, tracingTask, millisecondsTimeout);
        var ok = tracingTask.WaitForCompletion(millisecondsTimeout);
        if (!ok) throw new RedisInitializationException("Enable client cache tracing timeout");
        var result = tracingTask.Task.ConfigureAwait(false).GetAwaiter().GetResult();
        if (result.RawValue is Exception ex)
        {
            throw new RedisInitializationException($"Failed to start tracing, Msg: {ex.Message}");
        }
        if (!result)
        {
            throw new RedisInitializationException($"Failed to enable client cache, Msg: {result.RawValue}");
        }
        connection.RedirectConnectionId = clientCaching.RedirectConnectionId;
    }
}
