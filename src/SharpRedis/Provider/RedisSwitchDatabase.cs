#pragma warning disable IDE0130
using SharpRedis.Provider.Calls;
using SharpRedis.Provider.Standard;

namespace SharpRedis
{
    /// <summary>
    /// Switches to the specified database client
    /// <para>切换到指定的数据库</para>
    /// </summary>
    public sealed class RedisSwitchDatabase : FeatureRedis<RedisSwitchDatabase>
    {
        private SwitchDatabaseCall _switchCall;

        internal SwitchDatabaseCall SwitchCall => this._switchCall;

        private protected sealed override string DisposedException => "The currently selected database connection has been released";

        internal RedisSwitchDatabase(SwitchDatabaseCall call) : base(call)
        {
            this._switchCall = call;
        }

        #region NET30 Extensions
#if NET30
        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>Please be aware that the pipeline does not use client-side caching</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 管道不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// </summary>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
        public RedisPipelining BeginPipelining() => RedisExtensions.BeginPipelining(this);

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>Please be aware that the pipeline does not use client-side caching</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 管道不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// </summary>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
        public RedisPipelining StartPipelining() => RedisExtensions.BeginPipelining(this);

        /// <summary>
        /// Marks the start of a transaction block. Subsequent commands will be queued for atomic execution using [EXEC].
        /// <para>It is recommended that you use using to release a transaction immediately after using it</para>
        /// <para>Note that transactions do not use client-side caching, even if you have client-side caching enabled</para>
        /// <para>Available since: 1.2.0</para>
        /// <para>开启一个事务, 后续使用[EXEC]命令执行事务并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 事务不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// <para>支持的Redis版本: 1.2.0+</para>
        /// </summary>
        /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
        /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public RedisTransaction UseTransaction(bool multi = true, CancellationToken cancellationToken = default)
            => RedisExtensions.UseTransaction(this, multi, cancellationToken);

        /// <summary>
        /// Marks the start of a transaction block. Subsequent commands will be queued for atomic execution using [EXEC].
        /// <para>It is recommended that you use using to release a transaction immediately after using it</para>
        /// <para>Note that transactions do not use client-side caching, even if you have client-side caching enabled</para>
        /// <para>Available since: 1.2.0</para>
        /// <para>开启一个事务, 后续使用[EXEC]命令执行事务并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 事务不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// <para>支持的Redis版本: 1.2.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public RedisTransaction Multi(CancellationToken cancellationToken = default)
            => RedisExtensions.UseTransaction(this, true, cancellationToken);
#endif
        #endregion

        protected sealed override void Dispose(bool disposing)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8625
#endif
            base.Dispose(disposing);
            this._switchCall = null;
        }

        ~RedisSwitchDatabase()
        {
            this.Dispose(true);
        }
    }
}
