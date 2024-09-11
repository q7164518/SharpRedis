#pragma warning disable IDE0130

namespace SharpRedis
{
    /// <summary>
    /// SortedSet score: rank
    /// </summary>
    /// <para>排序分数类型, 只能是数字类型</para>
    public readonly struct ScoreRankValue
    {
        private readonly long _rank;
        private readonly NumberValue _score;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        /// <summary>
        /// Get rank
        /// <para>获得排名</para>
        /// </summary>
        public readonly long Rank => this._rank;

        /// <summary>
        /// Get score
        /// <para>获得排序分数</para>
        /// </summary>
        public readonly NumberValue Score => this._score;
#else
        /// <summary>
        /// Get rank
        /// <para>获得排名</para>
        /// </summary>
        public long Rank => this._rank;

        /// <summary>
        /// Get score
        /// <para>获得排序分数</para>
        /// </summary>
        public NumberValue Score => this._score;
#endif

        /// <summary>
        /// SortedSet score: rank
        /// </summary>
        /// <param name="rank">rank<para>排名</para></param>
        /// <param name="score">score<para>排序分数</para></param>
        internal ScoreRankValue(long rank, NumberValue score)
        {
            this._rank = rank;
            this._score = score;
        }

        public override string ToString()
        {
            return $"{this._rank}: {this._score}";
        }

        /// <summary>
        /// Deconstruct, var (rank, score) = this
        /// <para>解构函数, var (rank, score) = this</para>
        /// </summary>
        /// <param name="rank">rank<para>排名</para></param>
        /// <param name="score">score<para>排序分数</para></param>
        public void Deconstruct(out long rank, out NumberValue score)
        {
            rank = this.Rank;
            score = this.Score;
        }
    }
}
