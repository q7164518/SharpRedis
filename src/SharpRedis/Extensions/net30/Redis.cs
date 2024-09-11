#if NET30
namespace SharpRedis
{
    public partial class Redis
    {
        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// </summary>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
        public RedisPipelining BeginPipelining() => RedisExtensions.BeginPipelining(this);

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// </summary>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
        public RedisPipelining StartPipelining() => RedisExtensions.BeginPipelining(this);

        /// <summary>
        /// Switches to the specified database based on the subscript. Note that switching databases does not use client-side caching, even if you have client-side caching enabled
        /// <para>Remember to call the Disposable method to release after using it, using the using syntax</para>
        /// <para>If not released in time, the connection may run out</para>
        /// <para>根据下标切换到指定的数据库. 请注意, 切换数据库不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// <para>使用完记得调用Disposable方法进行释放, 使用using语法即可</para>
        /// <para>如果不及时释放, 可能会导致连接用尽</para>
        /// </summary>
        /// <param name="index">Database index
        /// <para>数据库下标</para>
        /// </param>
        /// <returns></returns>
        public RedisSwitchDatabase SwitchDatabase(ushort index) => RedisExtensions.SwitchDatabase(this, index);

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
    }
}
#endif
