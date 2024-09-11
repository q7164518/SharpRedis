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
                //case RedisServerMode.Sentinel:
                //case RedisServerMode.MasterSlave: tcall = new MasterSlaveCall(connectionPool); break;
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

        #region Created SharpRedis
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
    }
}
