using SharpRedis.Network.Standard;
using System;
using System.Text;

namespace SharpRedis.Provider.Standard
{
    public abstract class BaseRedis : IDisposable
    {
        private protected bool _disposedValue;
        private protected BaseCall _call;
        private protected RedisString _string;
        private protected RedisPubSub _pubsub;
        private protected RedisConnection _connection;
        private protected RedisHash _hash;
        private protected RedisList _list;
        private protected RedisBitmap _bitmap;
        private protected RedisSet _set;
        private protected RedisSortedSet _sortedSet;
        private protected RedisStream _stream;
        private protected RedisHyperLogLog _hyperLogLog;
        private protected RedisGeospatialIndices _geospatial;
        private protected RedisScript _script;
        private protected RedisKey _key;
        private protected RedisServer _server;

        internal BaseCall TCall => this._call;

        internal abstract IConnectionPool ConnectionPool { get; }

        /// <summary>
        /// Gets an object of type Redis String
        /// <para>获得操作Redis String类型的对象</para>
        /// </summary>
        public virtual RedisString String => this._string;

        /// <summary>
        /// Get the subscribe publish action object
        /// <para>获得Redis订阅发布操作对象</para>
        /// </summary>
        public virtual RedisPubSub PubSub => this._pubsub;

        /// <summary>
        /// Gets an object of type Redis connection
        /// <para>获得操作Redis连接的对象</para>
        /// </summary>
        public virtual RedisConnection Connection => this._connection;

        /// <summary>
        /// Gets an object of type Redis Hash
        /// <para>获得Redis Hash类型操作对象</para>
        /// </summary>
        public virtual RedisHash Hash => this._hash;

        /// <summary>
        /// Gets an object of type Redis List
        /// <para>获得Redis List类型操作对象</para>
        /// </summary>
        public virtual RedisList List => this._list;

        /// <summary>
        /// Gets an object of type Redis Bitmap
        /// <para>获得Redis Bitmap类型操作对象</para>
        /// </summary>
        public virtual RedisBitmap Bitmap => this._bitmap;

        /// <summary>
        /// Gets an object of type Redis Set
        /// <para>获得Redis Set类型操作对象</para>
        /// </summary>
        public virtual RedisSet Set => this._set;

        /// <summary>
        /// Gets an object of type Redis Sorted Set
        /// <para>获得Redis Sorted Set类型操作对象</para>
        /// </summary>
        public virtual RedisSortedSet SortedSet => this._sortedSet;

        /// <summary>
        /// Gets an object of type Redis Stream
        /// <para>获得Redis Stream类型操作对象</para>
        /// </summary>
        public virtual RedisStream Stream => this._stream;

        /// <summary>
        /// Gets an object of type Redis HyperLogLog
        /// <para>获得Redis HyperLogLog类型操作对象</para>
        /// </summary>
        public virtual RedisHyperLogLog HyperLogLog => this._hyperLogLog;

        /// <summary>
        /// Gets an object of type Redis Geospatial Indices
        /// <para>获得Redis Geospatial Indices类型操作对象</para>
        /// </summary>
        public virtual RedisGeospatialIndices Geospatial => this._geospatial;

        /// <summary>
        /// Gets an object of type Redis lua script
        /// <para>获得Redis Lua脚本操作对象</para>
        /// </summary>
        public virtual RedisScript Script => this._script;

        /// <summary>
        /// Gets an object of type Redis Key
        /// <para>获得Redis Key操作对象</para>
        /// </summary>
        public virtual RedisKey Key => this._key;

        /// <summary>
        /// Gets an object of type Redis Server
        /// <para>获得Redis Server端操作对象</para>
        /// </summary>
        public virtual RedisServer Server => this._server;

        private protected BaseRedis(BaseCall call)
        {
            this._call = call;
            this._string = new RedisString(this._call);
            this._pubsub = new RedisPubSub(this._call);
            this._connection = new RedisConnection(this._call);
            this._hash = new RedisHash(this._call);
            this._list = new RedisList(this._call);
            this._bitmap = new RedisBitmap(this._call);
            this._set = new RedisSet(this._call);
            this._sortedSet = new RedisSortedSet(this._call);
            this._stream = new RedisStream(this._call);
            this._hyperLogLog = new RedisHyperLogLog(this._call);
            this._geospatial = new RedisGeospatialIndices(this._call);
            this._script = new RedisScript(this._call);
            this._key = new RedisKey(this._call);
            this._server = new RedisServer(this._call);
        }

        private protected void SetCall(BaseCall call)
        {
            this._call = call;
            this._string.SetCall(call);
            this._pubsub.SetCall(call);
            this._connection.SetCall(call);
            this._hash.SetCall(call);
            this._list = new RedisList(this._call);
            this._bitmap.SetCall(call);
            this._set.SetCall(call);
            this._sortedSet.SetCall(call);
            this._stream.SetCall(call);
            this._hyperLogLog.SetCall(call);
            this._geospatial.SetCall(call);
            this._script.SetCall(call);
            this._key.SetCall(call);
            this._server.SetCall(call);
        }

        #region Dispose
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8625
#endif
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                    this._connection.Dispose();
                    this._call.Dispose();
                    this._string.Dispose();
                    this._pubsub.Dispose();
                    this._hash.Dispose();
                    this._list.Dispose();
                    this._bitmap.Dispose();
                    this._set.Dispose();
                    this._sortedSet.Dispose();
                    this._stream.Dispose();
                    this._hyperLogLog.Dispose();
                    this._geospatial.Dispose();
                    this._script.Dispose();
                    this._key.Dispose();
                    this._server.Dispose();
                }

                this._connection = null;
                this._string = null;
                this._call = null;
                this._pubsub = null;
                this._hash = null;
                this._list = null;
                this._bitmap = null;
                this._set = null;
                this._sortedSet = null;
                this._stream = null;
                this._hyperLogLog = null;
                this._geospatial = null;
                this._script = null;
                this._key = null;
                this._server = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
