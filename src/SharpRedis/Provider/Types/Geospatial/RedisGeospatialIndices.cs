#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using SharpRedis.Provider.Standard;
using SharpRedis.Commands;
using System.Collections.Generic;

namespace SharpRedis
{
    public sealed partial class RedisGeospatialIndices : BaseType
    {
        internal RedisGeospatialIndices(BaseCall call) : base(call)
        {
        }

        /// <summary>
        /// Adds the specified geospatial items (longitude, latitude, member) to the specified key
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>将指定的地理位置信息添加到Key</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0</para>
        /// </summary>
        /// <param name="key">GEO Key</param>
        /// <param name="member">member</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="nxx">
        /// Available since: 6.2.0. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="ch">
        /// Available since: 6.2.0. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members when the CH option is not used.
        /// <para>The number of new or updated members when the CH option is used.</para>
        /// <para>ch为false新增元素数量. ch为true新增和更新的元素数量</para>
        /// </returns>
        public long GeoAdd(string key, string member, double longitude, double latitude, NxXx nxx = NxXx.None, bool ch = false, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoAdd(key, member, longitude, latitude, nxx, ch), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Adds the specified geospatial items (longitude, latitude, member) to the specified key
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>将指定的地理位置信息添加到Key</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0</para>
        /// </summary>
        /// <param name="key">GEO Key</param>
        /// <param name="member">member</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="nxx">
        /// Available since: 6.2.0. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="ch">
        /// Available since: 6.2.0. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members when the CH option is not used.
        /// <para>The number of new or updated members when the CH option is used.</para>
        /// <para>ch为false新增元素数量. ch为true新增和更新的元素数量</para>
        /// </returns>
        public long GeoAdd(string key, byte[] member, double longitude, double latitude, NxXx nxx = NxXx.None, bool ch = false, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoAdd(key, member, longitude, latitude, nxx, ch), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Adds the specified geospatial items (longitude, latitude, member) to the specified key
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>将指定的地理位置信息添加到Key</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0</para>
        /// </summary>
        /// <param name="key">GEO Key</param>
        /// <param name="members">members</param>
        /// <param name="nxx">
        /// Available since: 6.2.0. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="ch">
        /// Available since: 6.2.0. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members when the CH option is not used.
        /// <para>The number of new or updated members when the CH option is used.</para>
        /// <para>ch为false新增元素数量. ch为true新增和更新的元素数量</para>
        /// </returns>
        public long GeoAdd(string key, Dictionary<string, CoordinateValue> members, NxXx nxx = NxXx.None, bool ch = false, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoAdd(key, members, nxx, ch), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Adds the specified geospatial items (longitude, latitude, member) to the specified key
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>将指定的地理位置信息添加到Key</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0</para>
        /// </summary>
        /// <param name="key">GEO Key</param>
        /// <param name="members">members</param>
        /// <param name="nxx">
        /// Available since: 6.2.0. Nx: Only add new elements. Don't update already existing elements.
        /// <para>Xx: Only update elements that already exist. Don't add new elements.</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. Nx: 仅添加新元素, 不更新已经存在的元素</para>
        /// <para>Xx: 只更新已经存在的元素, 不添加新元素</para>
        /// </param>
        /// <param name="ch">
        /// Available since: 6.2.0. Whether to change the return value to the number of affected members. By default, only the number of new members is counted.
        /// <para>If the elements and scores are the same, they are not counted</para>
        /// <para>支持此参数的Redis版本, 6.2.0+. 是否返回受影响的元素个数. 默认情况只统计新增元素数量. 如果元素和分数都一样, 也不计入统计</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of new members when the CH option is not used.
        /// <para>The number of new or updated members when the CH option is used.</para>
        /// <para>ch为false新增元素数量. ch为true新增和更新的元素数量</para>
        /// </returns>
        public long GeoAdd(string key, Dictionary<byte[], CoordinateValue> members, NxXx nxx = NxXx.None, bool ch = false, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoAdd(key, members, nxx, ch), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Return the distance between two members in the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0</para>
        /// <para>返回两个成员之间的距离</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">GEO Key</param>
        /// <param name="member1">member1</param>
        /// <param name="member2">member2</param>
        /// <param name="unit">The unit must be one of the following, and defaults to meters
        /// <para>返回距离的单位, 默认为米</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public double? GeoDist(string key, string member1, string member2, DistanceUnit unit = DistanceUnit.m, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullNumber<double>(GeospatialCommands.GeoDist(key, member1, member2, unit), ResultType.Nullable | ResultType.Double, cancellationToken);
        }

        /// <summary>
        /// Return the distance between two members in the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0</para>
        /// <para>返回两个成员之间的距离</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">GEO Key</param>
        /// <param name="member1">member1</param>
        /// <param name="member2">member2</param>
        /// <param name="unit">The unit must be one of the following, and defaults to meters
        /// <para>返回距离的单位, 默认为米</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public double? GeoDist(string key, byte[] member1, byte[] member2, DistanceUnit unit = DistanceUnit.m, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullNumber<double>(GeospatialCommands.GeoDist(key, member1, member2, unit), ResultType.Nullable | ResultType.Double, cancellationToken);
        }

        /// <summary>
        /// Return valid Geohash strings representing the position of one element in a sorted set value representing a geospatial index
        /// <para>Available since: 3.2.0</para>
        /// <para>获得一个成员的地理位置信息的Geohash字符串</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Geo key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GeoHash(string key, string member, CancellationToken cancellationToken = default)
#else
        public string GeoHash(string key, string member, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(GeospatialCommands.GeoHash(key, member), cancellationToken);
        }

        /// <summary>
        /// Return valid Geohash strings representing the position of one element in a sorted set value representing a geospatial index
        /// <para>Available since: 3.2.0</para>
        /// <para>获得一个成员的地理位置信息的Geohash字符串</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Geo key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GeoHash(string key, byte[] member, CancellationToken cancellationToken = default)
#else
        public string GeoHash(string key, byte[] member, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(GeospatialCommands.GeoHash<byte[]>(key, member), cancellationToken);
        }

        /// <summary>
        /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index
        /// <para>Available since: 3.2.0</para>
        /// <para>获得一个或多个成员的地理位置信息的Geohash字符串</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Geo key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string?[] GeoHash(string key, string[] members, CancellationToken cancellationToken = default)
#else
        public string[] GeoHash(string key, string[] members, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoHash(key, members), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index
        /// <para>Available since: 3.2.0</para>
        /// <para>获得一个或多个成员的地理位置信息的Geohash字符串</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">Geo key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string?[] GeoHash(string key, byte[][] members, CancellationToken cancellationToken = default)
#else
        public string[] GeoHash(string key, byte[][] members, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoHash(key, members), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key
        /// <para>Available since: 3.2.0</para>
        /// <para>获得指定成员的经纬度</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">geo key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public CoordinateValue? GeoPos(string key, string member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<CoordinateValue>(GeospatialCommands.GeoPos(key, member), ResultType.Nullable | ResultType.Coordinate, cancellationToken);
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key
        /// <para>Available since: 3.2.0</para>
        /// <para>获得指定成员的经纬度</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">geo key</param>
        /// <param name="member">member</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public CoordinateValue? GeoPos(string key, byte[] member, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<CoordinateValue>(GeospatialCommands.GeoPos<byte[]>(key, member), ResultType.Nullable | ResultType.Coordinate, cancellationToken);
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key
        /// <para>Available since: 3.2.0</para>
        /// <para>获得指定成员的经纬度</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">geo key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public CoordinateValue?[] GeoPos(string key, string[] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStructArray<CoordinateValue>(GeospatialCommands.GeoPos(key, members), ResultType.Array | ResultType.Nullable | ResultType.Coordinate, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key
        /// <para>Available since: 3.2.0</para>
        /// <para>获得指定成员的经纬度</para>
        /// <para>支持此命令的Redis版本, 3.2.0+</para>
        /// </summary>
        /// <param name="key">geo key</param>
        /// <param name="members">members</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public CoordinateValue?[] GeoPos(string key, byte[][] members, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStructArray<CoordinateValue>(GeospatialCommands.GeoPos(key, members), ResultType.Array | ResultType.Nullable | ResultType.Coordinate, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoRadius<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoRadius<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoRadius(key, longitude, latitude, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoRadiusBytes<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoRadiusBytes<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoRadius(key, longitude, latitude, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoRadius<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoRadius<TRadius>(
#endif
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoRadius(
                key,
                longitude,
                latitude,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoRadiusBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoRadiusBytes<TRadius>(
#endif
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoRadius(
                key,
                longitude,
                latitude,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Store the items in a sorted set populated with their geospatial information
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员, 并将结果存入指定的Key中</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="storeKey">store key
        /// <para>保存结果的目标Key</para>
        /// </param>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members stored in the target Key
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoRadiusStore<TRadius>(string storeKey, string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoRadius(key, longitude, latitude, radius, unit, count: count, any: any, order: sortord, storeKey: storeKey), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Store the items in a sorted set populated with their distance from the center as a floating point number, in the same unit specified in the radius
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员, 并将距离结果存入指定的Key中</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="storeDistKey">StoreDist key
        /// <para>保存结果的目标Key</para>
        /// </param>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members stored in the target Key
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoRadiusStoreDist<TRadius>(string storeDistKey, string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoRadius(key, longitude, latitude, radius, unit, count: count, any: any, order: sortord, storeDistKey: storeDistKey), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoRadius_Ro<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoRadius_Ro<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoRadius_Ro(key, longitude, latitude, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoRadius_RoBytes<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoRadius_RoBytes<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoRadius_Ro(key, longitude, latitude, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoRadius_Ro<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoRadius_Ro<TRadius>(
#endif
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoRadius_Ro(
                key,
                longitude,
                latitude,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius)
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoRadius_RoBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoRadius_RoBytes<TRadius>(
#endif
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoRadius_Ro(
                key,
                longitude,
                latitude,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoRadiusByMember<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoRadiusByMember<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoRadiusByMemberBytes<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoRadiusByMemberBytes<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoRadiusByMember<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoRadiusByMember<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoRadiusByMemberBytes<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoRadiusByMemberBytes<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoRadiusByMember<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoRadiusByMember<TRadius>(
#endif
            string key,
            string member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoRadiusByMember(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoRadiusByMemberBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoRadiusByMemberBytes<TRadius>(
#endif
            string key,
            string member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoRadiusByMember(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Store the items in a sorted set populated with their geospatial information
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员, 并将结果存入指定的Key中</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="storeKey">store key
        /// <para>保存结果的目标Key</para>
        /// </param>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members stored in the target Key
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoRadiusByMemberStore<TRadius>(string storeKey, string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord, storeKey: storeKey), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Store the items in a sorted set populated with their distance from the center as a floating point number, in the same unit specified in the radius
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员, 并将距离结果存入指定的Key中</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="storeDistKey">StoreDist key
        /// <para>保存结果的目标Key</para>
        /// </param>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members stored in the target Key
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoRadiusByMemberStoreDist<TRadius>(string storeDistKey, string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord, storeDistKey: storeDistKey), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoRadiusByMember<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoRadiusByMember<TRadius>(
#endif
            string key,
            byte[] member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoRadiusByMember(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoRadiusByMemberBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoRadiusByMemberBytes<TRadius>(
#endif
            string key,
            byte[] member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoRadiusByMember(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Store the items in a sorted set populated with their geospatial information
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员, 并将结果存入指定的Key中</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="storeKey">store key
        /// <para>保存结果的目标Key</para>
        /// </param>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members stored in the target Key
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoRadiusByMemberStore<TRadius>(string storeKey, string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord, storeKey: storeKey), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Store the items in a sorted set populated with their distance from the center as a floating point number, in the same unit specified in the radius
        /// <para>Available since: 3.2.0 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员, 并将距离结果存入指定的Key中</para>
        /// <para>支持此命令的Redis版本, 3.2.0+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="storeDistKey">StoreDist key
        /// <para>保存结果的目标Key</para>
        /// </param>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of members stored in the target Key
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoRadiusByMemberStoreDist<TRadius>(string storeDistKey, string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoRadiusByMember(key, member, radius, unit, count: count, any: any, order: sortord, storeDistKey: storeDistKey), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoRadiusByMember_Ro<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoRadiusByMember_Ro<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoRadiusByMember_Ro(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoRadiusByMember_RoBytes<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoRadiusByMember_RoBytes<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoRadiusByMember_Ro(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoRadiusByMember_Ro<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoRadiusByMember_Ro<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoRadiusByMember_Ro(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoRadiusByMember_RoBytes<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoRadiusByMember_RoBytes<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoRadiusByMember_Ro(key, member, radius, unit, count: count, any: any, order: sortord), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoRadiusByMember_Ro<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoRadiusByMember_Ro<TRadius>(
#endif
            string key,
            string member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoRadiusByMember_Ro(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoRadiusByMember_RoBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoRadiusByMember_RoBytes<TRadius>(
#endif
            string key,
            string member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoRadiusByMember_Ro(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoRadiusByMember_Ro<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoRadiusByMember_Ro<TRadius>(
#endif
            string key,
            byte[] member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoRadiusByMember_Ro(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is exactly like GEORADIUS with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set
        /// <para>Available since: 3.2.10 | 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员</para>
        /// <para>支持此命令的Redis版本, 3.2.10+ | 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>指定的成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoRadiusByMember_RoBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoRadiusByMember_RoBytes<TRadius>(
#endif
            string key,
            byte[] member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoRadiusByMember_Ro(
                key,
                member,
                radius,
                unit,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                order: sortord), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoSearch<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoSearch<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                null,
                longitude,
                latitude,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoSearchBytes<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoSearchBytes<TRadius>(string key, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                null,
                longitude,
                latitude,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和宽高, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoSearch<TCoordinate>(string key, double longitude, double latitude, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoSearch<TCoordinate>(string key, double longitude, double latitude, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                null,
                longitude,
                latitude,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和宽高, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoSearchBytes<TCoordinate>(string key, double longitude, double latitude, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoSearchBytes<TCoordinate>(string key, double longitude, double latitude, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                null,
                longitude,
                latitude,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoSearch<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoSearch<TRadius>(
#endif
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                null,
                longitude,
                latitude,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoSearchBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoSearchBytes<TRadius>(
#endif
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                null,
                longitude,
                latitude,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和宽高, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoSearch<TCoordinate>(
#else
        public GeoRadiusValue<string>[] GeoSearch<TCoordinate>(
#endif
            string key,
            double longitude,
            double latitude,
            TCoordinate width,
            TCoordinate height,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                null,
                longitude,
                latitude,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和宽高, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoSearchBytes<TCoordinate>(
#else
        public GeoRadiusValue<byte[]>[] GeoSearchBytes<TCoordinate>(
#endif
            string key,
            double longitude,
            double latitude,
            TCoordinate width,
            TCoordinate height,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                null,
                longitude,
                latitude,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoSearch<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoSearch<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoSearchBytes<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoSearchBytes<TRadius>(string key, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoSearch<TCoordinate>(string key, string member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoSearch<TCoordinate>(string key, string member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoSearchBytes<TCoordinate>(string key, string member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoSearchBytes<TCoordinate>(string key, string member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoSearch<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoSearch<TRadius>(
#endif
            string key,
            string member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoSearchBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoSearchBytes<TRadius>(
#endif
            string key,
            string member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoSearch<TRadius, string>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoSearch<TCoordinate>(
#else
        public GeoRadiusValue<string>[] GeoSearch<TCoordinate>(
#endif
            string key,
            string member,
            TCoordinate width,
            TCoordinate height,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoSearchBytes<TCoordinate>(
#else
        public GeoRadiusValue<byte[]>[] GeoSearchBytes<TCoordinate>(
#endif
            string key,
            string member,
            TCoordinate width,
            TCoordinate height,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoSearch<TCoordinate, string>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoSearch<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoSearch<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoSearch<TRadius, byte[]>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoSearchBytes<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoSearchBytes<TRadius>(string key, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoSearch<TRadius, byte[]>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? GeoSearch<TCoordinate>(string key, byte[] member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public string[] GeoSearch<TCoordinate>(string key, byte[] member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<string>(GeospatialCommands.GeoSearch<TCoordinate, byte[]>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? GeoSearchBytes<TCoordinate>(string key, byte[] member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#else
        public byte[][] GeoSearchBytes<TCoordinate>(string key, byte[] member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#endif
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<byte[]>(GeospatialCommands.GeoSearch<TCoordinate, byte[]>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoSearch<TRadius>(
#else
        public GeoRadiusValue<string>[] GeoSearch<TRadius>(
#endif
            string key,
            byte[] member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoSearch<TRadius, byte[]>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoSearchBytes<TRadius>(
#else
        public GeoRadiusValue<byte[]>[] GeoSearchBytes<TRadius>(
#endif
            string key,
            byte[] member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoSearch<TRadius, byte[]>(
                key,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<string>[]? GeoSearch<TCoordinate>(
#else
        public GeoRadiusValue<string>[] GeoSearch<TCoordinate>(
#endif
            string key,
            byte[] member,
            TCoordinate width,
            TCoordinate height,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<string>>(GeospatialCommands.GeoSearch<TCoordinate, byte[]>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Return the members of a sorted set populated with geospatial information using GEOADD, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员, 获得范围内的成员.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="key">Geo key</param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">distance unit
        /// <para>距离单位</para>
        /// </param>
        /// <param name="withCoord">Also return the longitude,latitude coordinates of the matching items
        /// <para>是否包含经纬度坐标返回</para>
        /// </param>
        /// <param name="withDist">Also return the distance of the returned items from the specified center. The distance is returned in the same unit as the unit specified as the radius argument of the command
        /// <para>是否包含距离返回, 单位和指定的半径单位相同</para>
        /// </param>
        /// <param name="withHash">Also return the raw geohash-encoded sorted set score of the item, in the form of a 52 bit unsigned integer
        /// <para>是否包含HASH返回, 52位正数编码的</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public GeoRadiusValue<byte[]>[]? GeoSearchBytes<TCoordinate>(
#else
        public GeoRadiusValue<byte[]>[] GeoSearchBytes<TCoordinate>(
#endif
            string key,
            byte[] member,
            TCoordinate width,
            TCoordinate height,
            DistanceUnit unit,
            bool withCoord,
            bool withDist,
            bool withHash,
            ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallClassArray<GeoRadiusValue<byte[]>>(GeospatialCommands.GeoSearch<TCoordinate, byte[]>(
                key,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash), ResultType.Array | ResultType.GeoRadiusValue | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// This command is like GEOSEARCH, but stores the result in destination key, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和半径, 获得范围内的成员, 并将结果存入目标Key中.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="destination">destination key<para>存储结果的Key</para></param>
        /// <param name="source">source key<para>搜索的源geo Key</para></param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="radius">radius<para>搜索半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="storeDist">When using the STOREDIST option, the command stores the items in a sorted set populated with their distance from the center of the circle or box, as a floating-point number, in the same unit specified for that shape
        /// <para>默认false, 转存的为距离排序分. 如果为true, 则把距离存储到结果key. 距离单位和设置的半径的单位一致</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting set
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoSearchStore<TRadius>(string destination, string source, double longitude, double latitude, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, bool storeDist = false, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoSearchStore<TRadius, string>(
                destination,
                source,
                null,
                longitude,
                latitude,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                storeDist), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is like GEOSEARCH, but stores the result in destination key, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的经纬度和宽高, 获得范围内的成员, 并将结果存入目标Key中.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="destination">destination key<para>存储结果的Key</para></param>
        /// <param name="source">source key<para>搜索的源geo Key</para></param>
        /// <param name="longitude">longitude<para>经度</para></param>
        /// <param name="latitude">latitude<para>纬度</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="storeDist">When using the STOREDIST option, the command stores the items in a sorted set populated with their distance from the center of the circle or box, as a floating-point number, in the same unit specified for that shape
        /// <para>默认false, 转存的为距离排序分. 如果为true, 则把距离存储到结果key. 距离单位和设置的半径的单位一致</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting set
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoSearchStore<TCoordinate>(string destination, string source, double longitude, double latitude, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, bool storeDist = false, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoSearchStore<TCoordinate, string>(
                destination,
                source,
                null,
                longitude,
                latitude,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                storeDist), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is like GEOSEARCH, but stores the result in destination key, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员和半径, 获得范围内的成员, 并将结果存入目标Key中.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="destination">destination key<para>存储结果的Key</para></param>
        /// <param name="source">source key<para>搜索的源geo Key</para></param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>搜索半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="storeDist">When using the STOREDIST option, the command stores the items in a sorted set populated with their distance from the center of the circle or box, as a floating-point number, in the same unit specified for that shape
        /// <para>默认false, 转存的为距离排序分. 如果为true, 则把距离存储到结果key. 距离单位和设置的半径的单位一致</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting set
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoSearchStore<TRadius>(string destination, string source, string member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, bool storeDist = false, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoSearchStore<TRadius, string>(
                destination,
                source,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                storeDist), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is like GEOSEARCH, but stores the result in destination key, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员度和宽高, 获得范围内的成员, 并将结果存入目标Key中.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="destination">destination key<para>存储结果的Key</para></param>
        /// <param name="source">source key<para>搜索的源geo Key</para></param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="storeDist">When using the STOREDIST option, the command stores the items in a sorted set populated with their distance from the center of the circle or box, as a floating-point number, in the same unit specified for that shape
        /// <para>默认false, 转存的为距离排序分. 如果为true, 则把距离存储到结果key. 距离单位和设置的半径的单位一致</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting set
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoSearchStore<TCoordinate>(string destination, string source, string member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, bool storeDist = false, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoSearchStore<TCoordinate, string>(
                destination,
                source,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                storeDist), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is like GEOSEARCH, but stores the result in destination key, searching within circular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员和半径, 获得范围内的成员, 并将结果存入目标Key中.(圆形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TRadius">Radius</typeparam>
        /// <param name="destination">destination key<para>存储结果的Key</para></param>
        /// <param name="source">source key<para>搜索的源geo Key</para></param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="radius">radius<para>搜索半径</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="storeDist">When using the STOREDIST option, the command stores the items in a sorted set populated with their distance from the center of the circle or box, as a floating-point number, in the same unit specified for that shape
        /// <para>默认false, 转存的为距离排序分. 如果为true, 则把距离存储到结果key. 距离单位和设置的半径的单位一致</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting set
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoSearchStore<TRadius>(string destination, string source, byte[] member, TRadius radius, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, bool storeDist = false, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoSearchStore<TRadius, byte[]>(
                destination,
                source,
                member,
                null,
                null,
                radius,
                null,
                null,
                unit,
                sortord,
                count,
                any,
                storeDist), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// This command is like GEOSEARCH, but stores the result in destination key, searching within rectangular areas
        /// <para>Available since: 6.2.0</para>
        /// <para>根据指定的成员度和宽高, 获得范围内的成员, 并将结果存入目标Key中.(矩形区域内搜索)</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <typeparam name="TCoordinate">Coordinate</typeparam>
        /// <param name="destination">destination key<para>存储结果的Key</para></param>
        /// <param name="source">source key<para>搜索的源geo Key</para></param>
        /// <param name="member">member<para>成员</para></param>
        /// <param name="width">width<para>矩形宽度</para></param>
        /// <param name="height">height<para>矩形高度</para></param>
        /// <param name="unit">The radius is specified in one of the following units
        /// <para>半径单位</para>
        /// </param>
        /// <param name="count">Quantity to be obtained. Get all by default
        /// <para>要获得的个数, 默认为0, 表示获得所有</para>
        /// </param>
        /// <param name="any">Available since: 6.2.0. When ANY is provided the command will return as soon as enough matches are found, so the results may not be the ones closest to the specified point
        /// <para>支持此参数设置为true的Redis版本6.2.0+. 如果设置为true, 当找到和count一样的成员后, 立即停止查找. 所以数据可能有误差</para>
        /// </param>
        /// <param name="sortord">sortord, default ASC<para>排序方式, 默认ASC</para></param>
        /// <param name="storeDist">When using the STOREDIST option, the command stores the items in a sorted set populated with their distance from the center of the circle or box, as a floating-point number, in the same unit specified for that shape
        /// <para>默认false, 转存的为距离排序分. 如果为true, 则把距离存储到结果key. 距离单位和设置的半径的单位一致</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of elements in the resulting set
        /// <para>存入目标Key的成员个数</para>
        /// </returns>
        public long GeoSearchStore<TCoordinate>(string destination, string source, byte[] member, TCoordinate width, TCoordinate height, DistanceUnit unit, ulong count = 0, bool any = false, OrderType sortord = OrderType.Asc, bool storeDist = false, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
        {
            return base._call.CallNumber<long>(GeospatialCommands.GeoSearchStore<TCoordinate, byte[]>(
                destination,
                source,
                member,
                null,
                null,
                null,
                width,
                height,
                unit,
                sortord,
                count,
                any,
                storeDist), ResultType.Int64, cancellationToken);
        }
    }
}
