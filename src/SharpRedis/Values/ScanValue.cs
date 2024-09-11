#pragma warning disable IDE0130

namespace SharpRedis
{
    /// <summary>
    /// SCAN result model
    /// <para>SCAN返回结果类</para>
    /// </summary>
    public sealed class ScanValue<TData>
        where TData : class, System.Collections.ICollection
    {
        private readonly ulong _cursor;
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private readonly TData? _data;

        /// <summary>
        /// Get the result returned by SCAN
        /// <para>获得SCAN返回的结果</para>
        /// </summary>
        public TData? Data => this._data;
#else
        private readonly TData _data;

        /// <summary>
        /// Get the result returned by SCAN
        /// <para>获得SCAN返回的结果</para>
        /// </summary>
        public TData Data => this._data;
#endif

        /// <summary>
        /// Get the cursor returned by SCAN
        /// <para>获得SCAN返回的游标</para>
        /// </summary>
        public ulong Cursor => this._cursor;

        /// <summary>
        /// Whether it has ended
        /// <para>是否已结束</para>
        /// </summary>
        public bool IsEnd => this._cursor == 0;

        /// <summary>
        /// Gets the count of the result returned by SCAN
        /// <para>获得SCAN返回的结果长度</para>
        /// </summary>
        public int Count
        {
            get
            {
                if (this._data is null) return -1;
                return this._data.Count;
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal ScanValue(ulong cursor, TData? data)
#else
        internal ScanValue(ulong cursor, TData data)
#endif
        {
            this._cursor = cursor;
            this._data = data;
        }
    }
}
