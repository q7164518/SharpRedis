#pragma warning disable IDE0130
using SharpRedis.Extensions;
using System;
using System.Text;

namespace SharpRedis
{
    /// <summary>
    /// Latitude and Longitude
    /// <para>经纬度</para>
    /// </summary>
    public readonly struct CoordinateValue : System.IEquatable<CoordinateValue>
    {
        private readonly double _longitude;
        private readonly double _latitude;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        /// <summary>
        /// Get longitude
        /// <para>获得经度</para>
        /// </summary>
        public readonly double Longitude => this._longitude;

        /// <summary>
        /// Get latitude
        /// <para>获得纬度</para>
        /// </summary>
        public readonly double Latitude => this._latitude;
#else
        /// <summary>
        /// Get longitude
        /// <para>获得经度</para>
        /// </summary>
        public double Longitude => this._longitude;

        /// <summary>
        /// Get latitude
        /// <para>获得纬度</para>
        /// </summary>
        public double Latitude => this._latitude;
#endif

        /// <summary>
        /// Create a latitude and longitude value
        /// <para>创建一个经纬度值</para>
        /// </summary>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        public CoordinateValue(double longitude, double latitude)
        {
            this._longitude = longitude;
            this._latitude = latitude;
        }

        internal CoordinateValue(object[] array, Encoding encoding)
        {
            if (array[0] is object[] pos && pos.Length is 2)
            {
                this._longitude = ConvertExtensions.To<double>(pos[0], ResultType.Double, encoding);
                this._latitude = ConvertExtensions.To<double>(pos[1], ResultType.Double, encoding);
            }
            else if (array.Length is 2)
            {
                this._longitude = ConvertExtensions.To<double>(array[0], ResultType.Double, encoding);
                this._latitude = ConvertExtensions.To<double>(array[1], ResultType.Double, encoding);
            }
            else
            {
                throw new FormatException($"The data is not a valid CoordinateValue, The actual type is {array.GetType().FullName}");
            }
        }

        public override int GetHashCode()
        {
            return this._longitude.GetHashCode() ^ this._latitude.GetHashCode();
        }

        public override string ToString()
        {
            return $"Longitude: {this._longitude}, Latitude: {this._latitude}";
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public override bool Equals(object? obj)
#else
        public override bool Equals(object obj)
#endif
        {
            if (obj is CoordinateValue other)
            {
                return other == this;
            }
            return false;
        }

        public bool Equals(CoordinateValue other)
        {
            return other._latitude == this._latitude && other._longitude == this._longitude;
        }

        /// <summary>
        /// Deconstruct, var (longitude, latitude) = this
        /// <para>解构函数var (longitude, latitude) = this</para>
        /// </summary>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        public void Deconstruct(out double longitude, out double latitude)
        {
            longitude = this._longitude;
            latitude = this._latitude;
        }

        public static bool operator ==(CoordinateValue left, CoordinateValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CoordinateValue left, CoordinateValue right)
        {
            return !left.Equals(right);
        }
    }
}
