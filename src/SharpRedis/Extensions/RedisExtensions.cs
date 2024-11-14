#pragma warning disable IDE0130
#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
#if !LOW_NET
using System.Threading;
using System.Threading.Tasks;
#endif
using SharpRedis.Provider.Calls;
using SharpRedis.Provider.Standard;

namespace SharpRedis
{
    public static class RedisExtensions
    {
        #region Switch database index
        /// <summary>
        /// Switches to the specified database based on the subscript. Note that switching databases does not use client-side caching, even if you have client-side caching enabled
        /// <para>Remember to call the Disposable method to release after using it, using the using syntax</para>
        /// <para>If not released in time, the connection may run out</para>
        /// <para>根据下标切换到指定的数据库. 请注意, 切换数据库不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// <para>使用完记得调用Disposable方法进行释放, 使用using语法即可</para>
        /// <para>如果不及时释放, 可能会导致连接用尽</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <param name="index">Database index
        /// <para>数据库下标</para>
        /// </param>
        /// <returns></returns>
#if NET30
        public static RedisSwitchDatabase SwitchDatabase(Redis redis, ushort index)
#else
        public static RedisSwitchDatabase SwitchDatabase(this Redis redis, ushort index)
#endif
            => new RedisSwitchDatabase(new SwitchDatabaseCall(index, redis.ConnectionPool));

        /// <summary>
        /// Switches to the specified database based on the subscript. Note that switching databases does not use client-side caching, even if you have client-side caching enabled
        /// <para>Remember to call the Disposable method to release after using it, using the using syntax</para>
        /// <para>If not released in time, the connection may run out</para>
        /// <para>根据下标切换到指定的数据库. 请注意, 切换数据库不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// <para>使用完记得调用Disposable方法进行释放, 使用using语法即可</para>
        /// <para>如果不及时释放, 可能会导致连接用尽</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <param name="index">Database index
        /// <para>数据库下标</para>
        /// </param>
        /// <returns></returns>
#if NET30
        public static RedisSwitchDatabase SelectDatabase(Redis redis, ushort index)
#else
        public static RedisSwitchDatabase SelectDatabase(this Redis redis, ushort index)
#endif
            => new RedisSwitchDatabase(new SwitchDatabaseCall(index, redis.ConnectionPool));

        /// <summary>
        /// Switches to the specified database based on the subscript. Note that switching databases does not use client-side caching, even if you have client-side caching enabled
        /// <para>Remember to call the Disposable method to release after using it, using the using syntax</para>
        /// <para>If not released in time, the connection may run out</para>
        /// <para>根据下标切换到指定的数据库. 请注意, 切换数据库不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// <para>使用完记得调用Disposable方法进行释放, 使用using语法即可</para>
        /// <para>如果不及时释放, 可能会导致连接用尽</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <param name="index">Database index
        /// <para>数据库下标</para>
        /// </param>
        /// <returns></returns>
#if NET30
        public static RedisSwitchDatabase Select(Redis redis, ushort index)
#else
        public static RedisSwitchDatabase Select(this Redis redis, ushort index)
#endif
            => new RedisSwitchDatabase(new SwitchDatabaseCall(index, redis.ConnectionPool));
        #endregion

        #region Default redis create pipelining
        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining StartPipelining(Redis redis)
#else
        public static RedisPipelining StartPipelining(this Redis redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining BeginPipelining(Redis redis)
#else
        public static RedisPipelining BeginPipelining(this Redis redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining CreatePipelining(Redis redis)
#else
        public static RedisPipelining CreatePipelining(this Redis redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining NewPipelining(Redis redis)
#else
        public static RedisPipelining NewPipelining(this Redis redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining UsePipelining(Redis redis)
#else
        public static RedisPipelining UsePipelining(this Redis redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }
        #endregion

        #region Switch database redis create pipelining
        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>Please be aware that the pipeline does not use client-side caching</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 管道不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining StartPipelining(RedisSwitchDatabase redis)
#else
        public static RedisPipelining StartPipelining(this RedisSwitchDatabase redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>Please be aware that the pipeline does not use client-side caching</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 管道不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining BeginPipelining(RedisSwitchDatabase redis)
#else
        public static RedisPipelining BeginPipelining(this RedisSwitchDatabase redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>Please be aware that the pipeline does not use client-side caching</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 管道不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining CreatePipelining(RedisSwitchDatabase redis)
#else
        public static RedisPipelining CreatePipelining(this RedisSwitchDatabase redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>Please be aware that the pipeline does not use client-side caching</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 管道不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining NewPipelining(RedisSwitchDatabase redis)
#else
        public static RedisPipelining NewPipelining(this RedisSwitchDatabase redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }

        /// <summary>
        /// Begin a command pipeline. Call the EndPipelining method to execute the pipeline and get the return value.
        /// <para>You are advised to release the command pipe immediately after using it.</para>
        /// <para>Please be aware that the pipeline does not use client-side caching</para>
        /// <para>开启一个命令管道. 调用EndPipelining方法来执行管道并获取返回值</para>
        /// <para>建议使用完命令管道后, 立即进行释放. 使用using语法即可</para>
        /// <para>请注意, 管道不会使用客户端缓存, 哪怕你开启了客户端缓存功能</para>
        /// </summary>
        /// <param name="redis">redis</param>
        /// <returns>Command pipeline
        /// <para>命令管道对象</para>
        /// </returns>
#if NET30
        public static RedisPipelining UsePipelining(RedisSwitchDatabase redis)
#else
        public static RedisPipelining UsePipelining(this RedisSwitchDatabase redis)
#endif
        {
            return RedisExtensions.CreatePipeliningRedis(redis);
        }
        #endregion

        #region Default redis use transaction
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
        /// <param name="redis">redis</param>
        /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
        /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
#if NET30
        public static RedisTransaction UseTransaction(Redis redis, bool multi = true, CancellationToken cancellationToken = default)
#else
        public static RedisTransaction UseTransaction(this Redis redis, bool multi = true, CancellationToken cancellationToken = default)
#endif
        {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            RedisTransaction? tranRedis = null;
#else
            RedisTransaction tranRedis = null;
#endif
            try
            {
                tranRedis = new RedisTransaction(new TransactionCall(redis.ConnectionPool, cancellationToken));
                if (multi)
                {
                    if (tranRedis.Multi(cancellationToken))
                    {
                        return tranRedis;
                    }
                    else
                    {
                        throw new RedisException("Failed to start transaction.");
                    }
                }
                return tranRedis;
            }
            catch
            {
                tranRedis?.Dispose();
                throw;
            }
        }

#if !NET30
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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction Multi(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction CreateTransaction(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction NewTransaction(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction StartTransaction(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction BeginTransaction(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);
#endif

#if !LOW_NET
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
        /// <param name="redis">redis</param>
        /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
        /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        async public static Task<RedisTransaction> UseTransactionAsync(this Redis redis, bool multi = true, CancellationToken cancellationToken = default)
        {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            RedisTransaction? tranRedis = null;
#else
            RedisTransaction tranRedis = null;
#endif
            try
            {
                tranRedis = new RedisTransaction(new TransactionCall(redis.ConnectionPool, cancellationToken));
                if (multi)
                {
                    if (await tranRedis.MultiAsync(cancellationToken).ConfigureAwait(false))
                    {
                        return tranRedis;
                    }
                    else
                    {
                        throw new RedisException("Failed to start transaction.");
                    }
                }
                return tranRedis;
            }
            catch
            {
                tranRedis?.Dispose();
                throw;
            }
        }

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> MultiAsync(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> CreateTransactionAsync(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> BeginTransactionAsync(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> NewTransactionAsync(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> StartTransactionAsync(this Redis redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);
#endif
        #endregion

        #region Switch database redis use transaction
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
        /// <param name="redis">redis</param>
        /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
        /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
#if NET30
        public static RedisTransaction UseTransaction(RedisSwitchDatabase redis, bool multi = true, CancellationToken cancellationToken = default)
#else
        public static RedisTransaction UseTransaction(this RedisSwitchDatabase redis, bool multi = true, CancellationToken cancellationToken = default)
#endif
        {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            RedisTransaction? tranRedis = null;
#else
            RedisTransaction tranRedis = null;
#endif
            try
            {
                tranRedis = new RedisTransaction(new TransactionCall(redis.ConnectionPool, redis.SwitchCall, cancellationToken));
                if (multi)
                {
                    if (tranRedis.Multi(cancellationToken))
                    {
                        return tranRedis;
                    }
                    else
                    {
                        throw new RedisException("Failed to start transaction.");
                    }
                }
                return tranRedis;
            }
            catch
            {
                tranRedis?.Dispose();
                throw;
            }
        }

#if !NET30
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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction Multi(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction CreateTransaction(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction NewTransaction(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction StartTransaction(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static RedisTransaction BeginTransaction(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransaction(true, cancellationToken);
#endif

#if !LOW_NET
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
        /// <param name="redis">redis</param>
        /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
        /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public async static Task<RedisTransaction> UseTransactionAsync(this RedisSwitchDatabase redis, bool multi = true, CancellationToken cancellationToken = default)
        {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            RedisTransaction? tranRedis = null;
#else
            RedisTransaction tranRedis = null;
#endif
            try
            {
                tranRedis = new RedisTransaction(new TransactionCall(redis.ConnectionPool, redis.SwitchCall, cancellationToken));
                if (multi)
                {
                    if (await tranRedis.MultiAsync(cancellationToken).ConfigureAwait(false))
                    {
                        return tranRedis;
                    }
                    else
                    {
                        throw new RedisException("Failed to start transaction.");
                    }
                }
                return tranRedis;
            }
            catch
            {
                tranRedis?.Dispose();
                throw;
            }
        }

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> MultiAsync(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> StartTransactionAsync(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> CreateTransactionAsync(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> NewTransactionAsync(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);

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
        /// <param name="redis">redis</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Redis transaction object
        /// <para>Redis事务操作对象</para>
        /// </returns>
        public static Task<RedisTransaction> BeginTransactionAsync(this RedisSwitchDatabase redis, CancellationToken cancellationToken = default)
            => redis.UseTransactionAsync(true, cancellationToken);
#endif
        #endregion

        #region Private methods
        private static RedisPipelining CreatePipeliningRedis<TRedis>(TRedis redis)
            where TRedis : BaseRedis
        {
            return new RedisPipelining(new PipeliningCall(redis.ConnectionPool, redis.TCall));
        }
        #endregion
    }
}
