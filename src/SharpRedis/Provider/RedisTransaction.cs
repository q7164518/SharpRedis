#pragma warning disable IDE0130
#if !LOW_NET
using System.Threading;
using System.Threading.Tasks;
#endif
using SharpRedis.Provider.Calls;
using System;
using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Network.Standard;

namespace SharpRedis
{
    /// <summary>
    /// Redis transaction
    /// <para>Available since: 1.2.0</para>
    /// <para>Redis事务操作</para>
    /// <para>支持的Redis版本: 1.2.0+</para>
    /// </summary>
    public sealed class RedisTransaction : BaseRedis
    {
        private const string _disposedException = "The transaction has been released and cannot continue";
        private const string _noOpenExceptionMsg = "The transaction is not started, you need to run the [MULTI] command to start the transaction.";
        private bool _isOpen = false;
        private TransactionCall _tranCall;

        internal override sealed IConnectionPool ConnectionPool => base._call.ConnectionPool;

        /// <summary>
        /// Gets an object of type Redis connection
        /// <para>获得操作Redis连接的对象</para>
        /// </summary>
        public sealed override RedisConnection Connection
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return this._isOpen ? base._connection : throw new NotSupportedException(RedisTransaction._noOpenExceptionMsg);
            }
        }

        /// <summary>
        /// Get the subscribe publish action object
        /// <para>获得Redis订阅发布操作对象</para>
        /// </summary>
        public sealed override RedisPubSub PubSub
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return this._isOpen ? base._pubsub : throw new NotSupportedException(RedisTransaction._noOpenExceptionMsg);
            }
        }

        /// <summary>
        /// Gets an object of type Redis String
        /// <para>获得操作Redis String类型的对象</para>
        /// </summary>
        public sealed override RedisString String
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return this._isOpen ? base._string : throw new NotSupportedException(RedisTransaction._noOpenExceptionMsg);
            }
        }

        /// <summary>
        /// Gets an object of type Redis Hash
        /// <para>获得Redis Hash类型操作对象</para>
        /// </summary>
        public sealed override RedisHash Hash
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._hash;
            }
        }

        /// <summary>
        /// Gets an object of type Redis List
        /// <para>获得Redis List类型操作对象</para>
        /// </summary>
        public sealed override RedisList List
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._list;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Bitmap
        /// <para>获得Redis Bitmap类型操作对象</para>
        /// </summary>
        public sealed override RedisBitmap Bitmap
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._bitmap;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Set
        /// <para>获得Redis Set类型操作对象</para>
        /// </summary>
        public sealed override RedisSet Set
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._set;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Sorted Set
        /// <para>获得Redis Sorted Set类型操作对象</para>
        /// </summary>
        public sealed override RedisSortedSet SortedSet
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._sortedSet;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Stream
        /// <para>获得Redis Stream类型操作对象</para>
        /// </summary>
        public sealed override RedisStream Stream
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._stream;
            }
        }

        /// <summary>
        /// Gets an object of type Redis HyperLogLog
        /// <para>获得Redis HyperLogLog类型操作对象</para>
        /// </summary>
        public sealed override RedisHyperLogLog HyperLogLog
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._hyperLogLog;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Geospatial Indices
        /// <para>获得Redis Geospatial Indices类型操作对象</para>
        /// </summary>
        public sealed override RedisGeospatialIndices Geospatial
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._geospatial;
            }
        }

        /// <summary>
        /// Gets an object of type Redis lua script
        /// <para>获得Redis Lua脚本操作对象</para>
        /// </summary>
        public sealed override RedisScript Script
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._script;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Key
        /// <para>获得Redis Key操作对象</para>
        /// </summary>
        public sealed override RedisKey Key
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._key;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Server
        /// <para>获得Redis Server端操作对象</para>
        /// </summary>
        public sealed override RedisServer Server
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
                return base._server;
            }
        }

        internal RedisTransaction(TransactionCall call) : base(call)
        {
            this._tranCall = call;
        }

        #region Sync
        /// <summary>
        /// Executes all previously queued commands in a transaction and restores the connection state to normal.
        /// <para>To use the transaction again, you can execute the [MULTI] command method again</para>
        /// <para></para>
        /// <para>Available since: 1.2.0</para>
        /// <para>执行事务中的命令, 并获取返回值, 并将连接状态恢复为正常. 如需继续使用事务, 可以再次执行[MULTI]命令方法</para>
        /// <para>支持的Redis版本: 1.2.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Command return values
        /// <para>返回值数组</para>
        /// </returns>
        public TransactionValue Exec(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            if (!this._isOpen) throw new RedisException("ERR: [EXEC] without [MULTI]");
            var data = this._tranCall.Call(TransactionCommands.Exec(), ResultType.Object, cancellationToken);
            return this._tranCall.ToResult(data);
        }

        /// <summary>
        /// Marks the start of a transaction block. Subsequent commands will be queued for atomic execution using [EXEC].
        /// <para>Available since: 1.2.0</para>
        /// <para>开启一个事务, 后续使用[EXEC]命令执行事务并获取返回值</para>
        /// <para>支持的Redis版本: 1.2.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Multi(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            if (this._isOpen) return true;
            if (this._tranCall.CallCondition(TransactionCommands.Multi(), "OK", cancellationToken))
            {
                this._tranCall.Reset();
                this._isOpen = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Marks the given keys to be watched for conditional execution of a transaction.
        /// <para>Available since: 2.2.0</para>
        /// <para>监视指定的Key, 可以监视多个</para>
        /// <para>支持的Redis版本: 2.2.0+</para>
        /// </summary>
        /// <param name="keys">Keys to watch
        /// <para>要监视的Key, 可以多个</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Watch(string[] keys, CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            return this._tranCall.CallCondition(TransactionCommands.Watch(keys), "OK", cancellationToken);
        }

        /// <summary>
        /// Marks the given keys to be watched for conditional execution of a transaction.
        /// <para>Available since: 2.2.0</para>
        /// <para>监视指定的Key, 可以监视多个</para>
        /// <para>支持的Redis版本: 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key to watch
        /// <para>要监视的Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Watch(string key, CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            return this._tranCall.CallCondition(TransactionCommands.Watch(key), "OK", cancellationToken);
        }

        /// <summary>
        /// Flushes all the previously watched keys for a transaction.
        /// <para>If you call [EXEC] or [DISCARD], there's no need to manually call [UNWATCH].</para>
        /// <para>Available since: 2.2.0</para>
        /// <para>取消当前事务中之前所有监视的Key. 如你已经执行了[EXEC]或[DISCARD]命令, 则无需手动调用此方法取消监视</para>
        /// <para>支持的Redis版本: 2.2.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Unwatch(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            return this._tranCall.CallCondition(TransactionCommands.Unwatch(), "OK", cancellationToken);
        }

        /// <summary>
        /// Flushes all previously queued commands in a transaction and restores the connection state to normal.
        /// <para>If WATCH was used, DISCARD unwatches all keys watched by the connection.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>清除并取消执行当前事务中所有的命令. 如果你之前调用了[WATCH], 则会取消所有之前所有监视的Key</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Discard(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            if (!this._isOpen) return true;
            if (this._tranCall.CallCondition(TransactionCommands.Discard(), "OK", cancellationToken))
            {
                this._tranCall.Reset();
                this._isOpen = false;
                return true;
            }
            return false;
        }
        #endregion

        #region Async
#if !LOW_NET
        /// <summary>
        /// Executes all previously queued commands in a transaction and restores the connection state to normal.
        /// <para>To use the transaction again, you can execute the [MULTI] command method again</para>
        /// <para></para>
        /// <para>Available since: 1.2.0</para>
        /// <para>执行事务中的命令, 并获取返回值, 并将连接状态恢复为正常. 如需继续使用事务, 可以再次执行[MULTI]命令方法</para>
        /// <para>支持的Redis版本: 1.2.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Command return values
        /// <para>返回值数组</para>
        /// </returns>
        public async Task<TransactionValue> ExecAsync(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            if (!this._isOpen) throw new RedisException("ERR: [EXEC] without [MULTI]");
            var data = await this._tranCall.CallAsync(TransactionCommands.Exec(), ResultType.Object, cancellationToken).ConfigureAwait(false);
            return this._tranCall.ToResult(data);
        }

        /// <summary>
        /// Marks the start of a transaction block. Subsequent commands will be queued for atomic execution using [EXEC].
        /// <para>Available since: 1.2.0</para>
        /// <para>开启一个事务, 后续使用[EXEC]命令执行事务并获取返回值</para>
        /// <para>支持的Redis版本: 1.2.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public async Task<bool> MultiAsync(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            if (this._isOpen) return true;
            if (await this._tranCall.CallConditionAsync(TransactionCommands.Multi(), "OK", cancellationToken).ConfigureAwait(false))
            {
                this._tranCall.Reset();
                this._isOpen = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Marks the given key to be watched for conditional execution of a transaction.
        /// <para>Available since: 2.2.0</para>
        /// <para>监视指定的Key</para>
        /// <para>支持的Redis版本: 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key to watch
        /// <para>要监视的Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> WatchAsync(string key, CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            return this._tranCall.CallConditionAsync(TransactionCommands.Watch(key), "OK", cancellationToken);
        }

        /// <summary>
        /// Marks the given keys to be watched for conditional execution of a transaction.
        /// <para>Available since: 2.2.0</para>
        /// <para>监视指定的Key, 可以监视多个</para>
        /// <para>支持的Redis版本: 2.2.0+</para>
        /// </summary>
        /// <param name="keys">Keys to watch
        /// <para>要监视的Key, 可以多个</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> WatchAsync(string[] keys, CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            return this._tranCall.CallConditionAsync(TransactionCommands.Watch(keys), "OK", cancellationToken);
        }

        /// <summary>
        /// Flushes all the previously watched keys for a transaction.
        /// <para>If you call [EXEC] or [DISCARD], there's no need to manually call [UNWATCH].</para>
        /// <para>Available since: 2.2.0</para>
        /// <para>取消当前事务中之前所有监视的Key. 如你已经执行了[EXEC]或[DISCARD]命令, 则无需手动调用此方法取消监视</para>
        /// <para>支持的Redis版本: 2.2.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> UnwatchAsync(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            return this._tranCall.CallConditionAsync(TransactionCommands.Unwatch(), "OK", cancellationToken);
        }

        /// <summary>
        /// Flushes all previously queued commands in a transaction and restores the connection state to normal.
        /// <para>If WATCH was used, DISCARD unwatches all keys watched by the connection.</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>清除并取消执行当前事务中所有的命令. 如果你之前调用了[WATCH], 则会取消所有之前所有监视的Key</para>
        /// <para>支持的Redis版本: 2.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public async Task<bool> DiscardAsync(CancellationToken cancellationToken = default)
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisTransaction), RedisTransaction._disposedException);
            if (!this._isOpen) return true;
            if (await this._tranCall.CallConditionAsync(TransactionCommands.Discard(), "OK", cancellationToken).ConfigureAwait(false))
            {
                this._tranCall.Reset();
                this._isOpen = false;
                return true;
            }
            return false;
        }
#endif
        #endregion

        protected sealed override void Dispose(bool disposing)
        {
            this._isOpen = false;
            base.Dispose(disposing);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8625
#endif
            this._tranCall = null;
        }

        ~RedisTransaction()
        {
            this.Dispose(true);
        }
    }
}
