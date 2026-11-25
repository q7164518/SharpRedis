using SharpRedis.Enums;
using System;

namespace SharpRedis;

public sealed partial class RedisGeospatial
{
    /// <summary>
    /// The return value structure of the <b>GEORADIUS</b> or <b>GEOSEARCH</b> command
    /// <para><b>GEORADIUS</b> 或 <b>GEOSEARCH</b> 命令单项返回值</para>
    /// </summary>
    public readonly struct GeoSearchResult : IEquatable<GeoSearchResult>
    {
        private readonly RedisResult _member;
        private readonly GeoCoordinate? _pos;
        private readonly double? _dist;
        private readonly long? _hash;

        /// <summary>
        /// Gets the name of the location member
        /// <para>获得位置成员的名称</para>
        /// </summary>
        public readonly RedisResult Member => this._member;

        /// <summary>
        /// Gets the latitude and longitude coordinates of the current location
        /// <para>If the <b>WITHCOORD</b> parameter is not specified, it will always return null.</para>
        /// <para>获得当前位置的经纬度坐标. 如果未指定 <b>WITHCOORD</b> 参数, 返回null</para>
        /// </summary>
        public readonly GeoCoordinate? Coord => this._pos;

        /// <summary>
        /// Gets the distance from the center as a floating point number, in the same unit specified in the radius.
        /// <para>If the <b>WITHDIST</b> parameter is not specified, it will always return null.</para>
        /// <para>获得当前位置成员距离中心点的距离, 距离单位和执行命令时指定的单位一致</para>
        /// <para>如果未指定 <b>WITHDIST</b> 参数, 返回null</para>
        /// </summary>
        public readonly double? Dist => this._dist;

        /// <summary>
        /// Gets the raw 52-bit geohash-encoded score of matching item.
        /// <para>If the <b>WITHHASH</b> parameter is not specified, it will always return null.</para>
        /// <para>获得当前位置成员, 位置信息的Hash编码分数</para>
        /// <para>如果未指定 <b>WITHHASH</b> 参数, 返回null</para>
        /// </summary>
        public readonly long? Hash => this._hash;

        internal GeoSearchResult(RedisResult value)
        {
            if (value.Type == RespType.Array && value.RawValue is RedisResult[] array)
            {
                this._member = array[0];

                for (int i = 1; i < array.Length; i++)
                {
                    var item = array[i];
                    if (item.Type is RespType.Array && item.RawValue is RedisResult[] { Length: 2 } pos)
                    {
                        //position
                        this._pos = new(longitude: pos[0], latitude: pos[1]);
                    }
                    else if (item.Type is RespType.Integer)
                    {
                        this._hash = item;
                    }
                    else
                    {
                        this._dist = item;
                    }
                }
            }
            else if (value.Type == RespType.BulkStrings)
            {
                this._member = value;
            }
            else
            {
                throw new InvalidCastException($"The return value is not a GeoRadiusResult. The actual type is {value.RawValue.GetType().FullName}");
            }
        }

        public bool Equals(GeoSearchResult other)
        {
            return other._member.RawValue.Equals(this._member.RawValue)
                && other._dist == this._dist
                && other._hash == this._hash
                && other._pos == this._pos;
        }

#if MODERN_NET
        public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is not GeoSearchResult other) return false;
            return other == this;
        }

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            unchecked
            {
                int hash = 23;
                hash = hash * 31 + this._member.GetHashCode();
                hash = hash * 31 + this._dist.GetHashCode();
                hash = hash * 31 + this._hash.GetHashCode();
                hash = hash * 31 + this._pos.GetHashCode();
                return hash;
            }
#else
            return HashCode.Combine(this._member, this._dist, this._hash, this._pos);
#endif
        }

        public static bool operator ==(GeoSearchResult left, GeoSearchResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GeoSearchResult left, GeoSearchResult right)
        {
            return !left.Equals(right);
        }
    }
}
