#pragma warning disable IDE0130
#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
namespace SharpRedis
{
    public sealed class StreamTrimOptions : System.IEquatable<StreamTrimOptions>
    {
        private readonly static StreamTrimOptions _empty = new StreamTrimOptions();

        private readonly StreamTrimMode _trimMode;
        private readonly StreamTrimStrategy _trimStrategy;
        private readonly string _threshold;
        private readonly ulong _limit;

        internal StreamTrimMode TrimMode => this._trimMode;

        internal StreamTrimStrategy TrimStrategy => this._trimStrategy;

        internal string Threshold => this._threshold;

        internal ulong Limit => this._limit;

        /// <summary>
        /// Gets empty StreamTrimOptions
        /// <para>获得一个空的修剪配置</para>
        /// </summary>
        public static StreamTrimOptions Empty => StreamTrimOptions._empty;

        private StreamTrimOptions()
        {
            this._threshold = string.Empty;
            this._trimMode = StreamTrimMode.None;
            this._trimStrategy = StreamTrimStrategy.None;
        }

        /// <summary>
        /// Create StreamTrimOptions
        /// </summary>
        /// <param name="trimMode">MAXLEN | MINID</param>
        /// <param name="trimStrategy">= | ~</param>
        /// <param name="threshold">threshold</param>
        /// <param name="limit">limit count. The default 0 indicates no limit
        /// <para>Available since: 6.2.0</para>
        /// <para>删除个数, 默认0表示无限制</para>
        /// <para>支持此参数的Redis版本: 6.2.0+</para>
        /// </param>
        public StreamTrimOptions(StreamTrimMode trimMode, StreamTrimStrategy trimStrategy, string threshold, ulong limit = 0)
        {
            this._trimMode = trimMode;
            this._trimStrategy = trimStrategy;
            this._threshold = threshold;
            this._limit = limit;
        }

        public override string ToString()
        {
            if (this._limit > 0)
            {
                return $"{this._trimMode.ToString().ToUpper()} {this._trimStrategy.ToString().ToUpper()} {this._threshold} LIMIT {this._limit}";
            }
            else
            {
                return $"{this._trimMode.ToString().ToUpper()} {this._trimStrategy.ToString().ToUpper()} {this._threshold}";
            }
        }

        public override int GetHashCode()
        {
            return this._trimMode.GetHashCode() ^ this._trimStrategy.GetHashCode() ^ this._limit.GetHashCode() ^ this._threshold.GetHashCode();
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override bool Equals(object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is StreamTrimOptions other)
            {
                return other == this;
            }
            return false;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public bool Equals(StreamTrimOptions? other)
#else
        public bool Equals(StreamTrimOptions other)
#endif
        {
            if (other is null) return false;
            return this._threshold == other._threshold
                && this._limit == other._limit
                && this._trimMode == other._trimMode
                && this._trimStrategy == other._trimStrategy;
        }

        /// <summary>
        /// Create StreamTrimOptions
        /// </summary>
        /// <param name="trimMode">MAXLEN | MINID</param>
        /// <param name="trimStrategy">= | ~</param>
        /// <param name="threshold">threshold</param>
        /// <param name="limit">limit count. The default 0 indicates no limit
        /// <para>Available since: 6.2.0</para>
        /// <para>删除个数, 默认0表示无限制</para>
        /// <para>支持此参数的Redis版本: 6.2.0+</para>
        /// </param>
        public static StreamTrimOptions Create(StreamTrimMode trimMode, StreamTrimStrategy trimStrategy, string threshold, ulong limit = 0)
        {
            return new StreamTrimOptions(trimMode, trimStrategy, threshold, limit);
        }

        public static bool operator ==(StreamTrimOptions left, StreamTrimOptions right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StreamTrimOptions left, StreamTrimOptions right)
        {
            return !left.Equals(right);
        }
    }
}
