#pragma warning disable IDE0130

namespace SharpRedis
{
    /// <summary>
    /// [GEORADIUS] result
    /// <para>[GEORADIUS]返回值</para>
    /// </summary>
    public sealed class GeoRadiusValue<TMember>
        where TMember : class
    {
        private readonly TMember _member;
        private readonly double? _dist;
        private readonly ulong? _hash;
        private readonly CoordinateValue? _coordinate;

        /// <summary>
        /// Get member
        /// <para>获得成员</para>
        /// </summary>
        public TMember Member => this._member;

        /// <summary>
        /// Get distance
        /// <para>获得相差距离</para>
        /// </summary>
        public double? Dist => this._dist;

        /// <summary>
        /// Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>获得以52位无符号整数的形式返回该项的原始geohash编码的排序集分数</para>
        /// </summary>
        public ulong? Hash => this._hash;

        /// <summary>
        /// Get the latitude and longitude coordinates
        /// <para>获得经纬度坐标</para>
        /// </summary>
        public CoordinateValue? Coordinate => this._coordinate;

        internal GeoRadiusValue(TMember member, double? dist, ulong? hash, CoordinateValue? coordinate)
        {
            this._member = member;
            this._dist = dist;
            this._hash = hash;
            this._coordinate = coordinate;
        }
    }
}
