using SharpRedis.Network.Standard;
using System;

namespace SharpRedis.Provider.Standard
{
    public abstract class FeatureRedis<T> : BaseRedis
        where T : FeatureRedis<T>
    {
        private protected abstract string DisposedException { get; }

        internal sealed override IConnectionPool ConnectionPool => base._call.ConnectionPool;

        /// <summary>
        /// Gets an object of type Redis connection
        /// <para>获得操作Redis连接的对象</para>
        /// </summary>
        public override RedisConnection Connection
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._connection;
            }
        }

        /// <summary>
        /// Get the subscribe publish action object
        /// <para>获得Redis订阅发布操作对象</para>
        /// </summary>
        public override RedisPubSub PubSub
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._pubsub;
            }
        }

        /// <summary>
        /// Gets an object of type Redis String
        /// <para>获得操作Redis String类型的对象</para>
        /// </summary>
        public override RedisString String
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._string;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Hash
        /// <para>获得Redis Hash类型操作对象</para>
        /// </summary>
        public override RedisHash Hash
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._hash;
            }
        }

        /// <summary>
        /// Gets an object of type Redis List
        /// <para>获得Redis List类型操作对象</para>
        /// </summary>
        public override RedisList List
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._list;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Bitmap
        /// <para>获得Redis Bitmap类型操作对象</para>
        /// </summary>
        public override RedisBitmap Bitmap
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._bitmap;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Set
        /// <para>获得Redis Set类型操作对象</para>
        /// </summary>
        public override RedisSet Set
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._set;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Sorted Set
        /// <para>获得Redis Sorted Set类型操作对象</para>
        /// </summary>
        public override RedisSortedSet SortedSet
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._sortedSet;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Stream
        /// <para>获得Redis Stream类型操作对象</para>
        /// </summary>
        public override RedisStream Stream
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._stream;
            }
        }

        /// <summary>
        /// Gets an object of type Redis HyperLogLog
        /// <para>获得Redis HyperLogLog类型操作对象</para>
        /// </summary>
        public override RedisHyperLogLog HyperLogLog
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._hyperLogLog;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Geospatial Indices
        /// <para>获得Redis Geospatial Indices类型操作对象</para>
        /// </summary>
        public override RedisGeospatialIndices Geospatial
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._geospatial;
            }
        }

        /// <summary>
        /// Gets an object of type Redis lua script
        /// <para>获得Redis Lua脚本操作对象</para>
        /// </summary>
        public override RedisScript Script
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._script;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Key
        /// <para>获得Redis Key操作对象</para>
        /// </summary>
        public override RedisKey Key
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._key;
            }
        }

        /// <summary>
        /// Gets an object of type Redis Server
        /// <para>获得Redis Server端操作对象</para>
        /// </summary>
        public override RedisServer Server
        {
            get
            {
                if (base._disposedValue) throw new ObjectDisposedException(typeof(T).Name, this.DisposedException);
                return base._server;
            }
        }

        private protected FeatureRedis(BaseCall call) : base(call)
        {
        }
    }
}
