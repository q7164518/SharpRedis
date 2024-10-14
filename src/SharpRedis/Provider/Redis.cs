#if !NET30
#pragma warning disable IDE0130
#endif
using SharpRedis.Extensions;
using SharpRedis.Network.Pool;
using SharpRedis.Network.Standard;
using SharpRedis.Provider.Calls;
using SharpRedis.Provider.Standard;
using System;

namespace SharpRedis
{
    /// <summary>
    /// Redis client
    /// <para>Redis操作对象</para>
    /// </summary>
    public sealed partial class Redis : BaseRedis
    {
        private IConnectionPool _connectionPool;
        private readonly RedisServerMode _redisServerMode;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private ClientSideCachingStandard? _clientSideCaching;
#else
        private ClientSideCachingStandard _clientSideCaching;
#endif

        internal override sealed IConnectionPool ConnectionPool => this._connectionPool;

        private Redis(ConnectionOptions connectionOptions)
            : this(new DefaultPool(connectionOptions), RedisServerMode.Standalone)
        {
        }

        private Redis(ConnectionOptions connectionOptions, ConnectionOptions[] slaveConnectionOptions)
            : this(new MasterSlavePool(connectionOptions, slaveConnectionOptions), RedisServerMode.MasterSlave)
        {
        }

        private Redis(IConnectionPool connectionPool, RedisServerMode serverMode)
            : base(Redis.CreateCall(connectionPool, serverMode))
        {
            this._redisServerMode = serverMode;
            this._connectionPool = connectionPool;
        }

        private static BaseCall CreateCall(IConnectionPool connectionPool, RedisServerMode serverMode)
        {
            BaseCall tcall;
            switch (serverMode)
            {
                case RedisServerMode.Standalone: tcall = new DefaultCall(connectionPool); break;
                case RedisServerMode.MasterSlave: tcall = new MasterSlaveCall(connectionPool); break;
                //case RedisServerMode.Sentinel:
                //case RedisServerMode.Cluster:
                default: tcall = new DefaultCall(connectionPool); break;
            }
            return tcall;
        }

        protected override void Dispose(bool disposing)
        {
            if (!base._disposedValue)
            {
                if (disposing)
                {
                    this._connectionPool?.Dispose();
                    this._clientSideCaching?.Dispose();
                }
                base.Dispose(disposing);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8625
#endif
                this._connectionPool = null;
                this._clientSideCaching = null;
            }
        }

        public override string ToString()
        {
            return $"SharpRedis, Mode: {this._redisServerMode}";
        }

        internal void UseClientSideCaching(ClientSideCachingStandard clientSideCaching)
        {
            lock (this)
            {
                if (this._clientSideCaching != null) return;
                this._clientSideCaching = clientSideCaching;
                this._connectionPool.SetSetClientSideCaching(clientSideCaching);
            }
            var call = new ClientSideCachingCall(this._connectionPool, base._call, clientSideCaching);
            ClientSideCachingExtensions.UseClientSideCaching(this, clientSideCaching);
            base.SetCall(call);
        }

        #region Created Standalone SharpRedis
        /// <summary>
        /// Connect to singleton Redis
        /// <para>连接单例Redis</para>
        /// </summary>
        /// <param name="connectionString">connection string
        /// <para>连接字符串</para>
        /// </param>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static Redis UseStandalone(string connectionString, Action<ConnectionOptions>? optionsAction = null)
#else
        public static Redis UseStandalone(string connectionString, Action<ConnectionOptions> optionsAction = null)
#endif
        {
            ConnectionOptions connectionOptions;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionOptions = new ConnectionOptions();
            }
            else
            {
                connectionOptions = new ConnectionOptions(connectionString);
            }
            optionsAction?.Invoke(connectionOptions);
            var redis = new Redis(connectionOptions);
            connectionOptions.UseClientSideCaching(redis);
            return redis;
        }

        /// <summary>
        /// Connect to singleton Redis
        /// <para>连接单例Redis</para>
        /// </summary>
        /// <param name="optionsAction">optionsAction</param>
        /// <returns></returns>
        public static Redis UseStandalone(Action<ConnectionOptions> optionsAction)
        {
            return Redis.UseStandalone(string.Empty, optionsAction);
        }
        #endregion

        #region Created Master-Slave SharpRedis
        /// <summary>
        /// Connect to master-slave Redis
        /// <para>连接主从Redis</para>
        /// </summary>
        /// <param name="masterConnectionString">Master node connection string
        /// <para>Redis主节点连接字符串</para>
        /// </param>
        /// <param name="slaveConnectionStrings">Slave node connection string
        /// <para>Redis从节点连接字符串</para>
        /// </param>
        /// <param name="optionsAction">The first parameter configures the object for the primary node and the second parameter configures the object for the secondary node
        /// <para>第一个参数为主节点配置对象, 第二个参数为从节点配置对象</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static Redis UseMasterSlave(string masterConnectionString, string[] slaveConnectionStrings, Action<ConnectionOptions, ConnectionOptions[]>? optionsAction = null)
#else
        public static Redis UseMasterSlave(string masterConnectionString, string[] slaveConnectionStrings, Action<ConnectionOptions, ConnectionOptions[]> optionsAction = null)
#endif
        {
            ConnectionOptions masterConnectionOptions;
            var slaveConnectionOptions = new ConnectionOptions[slaveConnectionStrings.Length];
            if (string.IsNullOrEmpty(masterConnectionString))
            {
                masterConnectionOptions = new ConnectionOptions();
            }
            else
            {
                masterConnectionOptions = new ConnectionOptions(masterConnectionString);
            }
            for (int i = 0; i < slaveConnectionStrings.Length; i++)
            {
                if (string.IsNullOrEmpty(slaveConnectionStrings[i]))
                {
                    slaveConnectionOptions[i] = new ConnectionOptions();
                }
                else
                {
                    slaveConnectionOptions[i] = new ConnectionOptions(slaveConnectionStrings[i]);
                }
            }
            optionsAction?.Invoke(masterConnectionOptions, slaveConnectionOptions);
            var redis = new Redis(masterConnectionOptions, slaveConnectionOptions);
            masterConnectionOptions.UseClientSideCaching(redis);
            return redis;
        }

        /// <summary>
        /// Connect to master-slave Redis
        /// <para>连接主从Redis</para>
        /// </summary>
        /// <param name="slaveCount">Slave node count
        /// <para>从节点数量</para>
        /// </param>
        /// <param name="optionsAction">The first parameter configures the object for the primary node and the second parameter configures the object for the secondary node
        /// <para>第一个参数为主节点配置对象, 第二个参数为从节点配置对象</para>
        /// </param>
        /// <returns></returns>
        public static Redis UseMasterSlave(int slaveCount, Action<ConnectionOptions, ConnectionOptions[]> optionsAction)
        {
            return Redis.UseMasterSlave(string.Empty, new string[slaveCount], optionsAction);
        }
        #endregion
    }
}
