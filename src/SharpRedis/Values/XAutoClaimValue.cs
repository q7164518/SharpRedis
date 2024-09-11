#pragma warning disable IDE0130

namespace SharpRedis
{
    public sealed class XAutoClaimValue<TValue>
        where TValue : class
    {
        private readonly string _nextID;

        /// <summary>
        /// A stream ID to be used as the start argument for the next call to XAUTOCLAIM
        /// <para>用于下次调用XAUTOCLAIM的ID</para>
        /// </summary>
        public string NextID => this._nextID;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private readonly StreamValue<TValue>[]? _fvs;
        private readonly string[]? _deleted;

        /// <summary>
        /// An Array reply containing all the successfully claimed messages in the same format as XRANGE
        /// <para>包含所有成功声明消息的数组，其格式与XRANGE相同</para>
        /// </summary>
        public StreamValue<TValue>[]? Messages => this._fvs;

        /// <summary>
        /// An Array reply containing message IDs that no longer exist in the stream, and were deleted from the PEL in which they were found
        /// <para>Available since: 7.0.0</para>
        /// <para>从PEL中清除的已删除条目ID</para>
        /// <para>支持此返回值的Redis版本, 7.0.0+, 低版本永远为null</para>
        /// </summary>
        public string[]? Deleted => this._deleted;

        internal XAutoClaimValue(string nextID, StreamValue<TValue>[]? fvs, string[]? deleted)
#else
        private readonly StreamValue<TValue>[] _fvs;
        private readonly string[] _deleted;

        /// <summary>
        /// An Array reply containing all the successfully claimed messages in the same format as XRANGE
        /// <para>包含所有成功声明消息的数组，其格式与XRANGE相同</para>
        /// </summary>
        public StreamValue<TValue>[] Messages => this._fvs;

        /// <summary>
        /// An Array reply containing message IDs that no longer exist in the stream, and were deleted from the PEL in which they were found
        /// <para>Available since: 7.0.0</para>
        /// <para>从PEL中清除的已删除条目ID</para>
        /// <para>支持此返回值的Redis版本, 7.0.0+, 低版本永远为null</para>
        /// </summary>
        public string[] Deleted => this._deleted;

        internal XAutoClaimValue(string nextID, StreamValue<TValue>[] fvs, string[] deleted)
#endif
        {
            this._nextID = nextID;
            this._fvs = fvs;
            this._deleted = deleted;
        }
    }
}
