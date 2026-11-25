using SharpRedis.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public sealed partial class RedisGeospatial
{
    /// <summary>
    /// Adds the specified geospatial items (longitude, latitude, name) to the specified key.
    /// <para>Available since: 3.2.0</para>
    /// <para>将指定的地理空间项目（经度、纬度、名称）添加到指定的键中。</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间信息的Key</para>
    /// </param>
    /// <param name="longitude">longitude<para>经度</para></param>
    /// <param name="latitude">latitude<para>纬度</para></param>
    /// <param name="member">member<para>地理空间位置元素名称</para></param>
    /// <param name="ch">Modify the return value from the number of new elements added, to the total number of elements changed (CH is an abbreviation of changed).
    /// <para>是否将返回值从<b>新增元素数量</b>改为<b>已更改元素数量</b></para>
    /// </param>
    /// <param name="when">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>NX: Don't update already existing elements. Always add new elements.</para>
    /// <para>XX: Only update elements that already exist. Never add elements.</para>
    /// <para>NX: 不更新已存在的元素, 只有在元素不存在的时候进行添加</para>
    /// <para>XX: 只更新已存在的元素, 如果元素不存在, 不进行任何操作</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>When used without optional arguments, the number of elements added to the sorted set (excluding score updates). If the CH option is specified, the number of elements that were changed (added or updated).
    /// <para>未指定ch为true时候, 返回已添加的元素数量. 如果指定ch为true, 则返回已更改的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoAdd">命令文档</see></para>
    /// </remarks>
    public Task<int> GeoAddAsync(SharpValue key, double longitude, double latitude, SharpValue member, bool ch = false, GeoExist when = GeoExist.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoAdd(
            key: key,
            exist: when,
            ch: ch,
            position: new(longitude, latitude, member)) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds the specified geospatial items (longitude, latitude, name) to the specified key.
    /// <para>Available since: 3.2.0</para>
    /// <para>将指定的地理空间项目（经度、纬度、名称）添加到指定的键中。</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间信息的Key</para>
    /// </param>
    /// <param name="position">A geographical location information with latitude and longitude coordinates
    /// <para>一个由经纬度表示的地理位置信息</para>
    /// </param>
    /// <param name="ch">Modify the return value from the number of new elements added, to the total number of elements changed (CH is an abbreviation of changed).
    /// <para>是否将返回值从<b>新增元素数量</b>改为<b>已更改元素数量</b></para>
    /// </param>
    /// <param name="when">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>NX: Don't update already existing elements. Always add new elements.</para>
    /// <para>XX: Only update elements that already exist. Never add elements.</para>
    /// <para>NX: 不更新已存在的元素, 只有在元素不存在的时候进行添加</para>
    /// <para>XX: 只更新已存在的元素, 如果元素不存在, 不进行任何操作</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>When used without optional arguments, the number of elements added to the sorted set (excluding score updates). If the CH option is specified, the number of elements that were changed (added or updated).
    /// <para>未指定ch为true时候, 返回已添加的元素数量. 如果指定ch为true, 则返回已更改的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoAdd">命令文档</see></para>
    /// </remarks>
    public Task<int> GeoAddAsync(SharpValue key, GeoMember position, bool ch = false, GeoExist when = GeoExist.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoAdd(
            key: key,
            exist: when,
            ch: ch,
            position: position) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds the specified geospatial items (longitude, latitude, name) to the specified key.
    /// <para>Available since: 3.2.0</para>
    /// <para>将指定的地理空间项目（经度、纬度、名称）添加到指定的键中。</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间信息的Key</para>
    /// </param>
    /// <param name="positions">A geographical location information with latitude and longitude coordinates
    /// <para>一个由经纬度表示的地理位置信息</para>
    /// </param>
    /// <param name="ch">Modify the return value from the number of new elements added, to the total number of elements changed (CH is an abbreviation of changed).
    /// <para>是否将返回值从<b>新增元素数量</b>改为<b>已更改元素数量</b></para>
    /// </param>
    /// <param name="when">
    /// <b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// <para>NX: Don't update already existing elements. Always add new elements.</para>
    /// <para>XX: Only update elements that already exist. Never add elements.</para>
    /// <para>NX: 不更新已存在的元素, 只有在元素不存在的时候进行添加</para>
    /// <para>XX: 只更新已存在的元素, 如果元素不存在, 不进行任何操作</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>When used without optional arguments, the number of elements added to the sorted set (excluding score updates). If the CH option is specified, the number of elements that were changed (added or updated).
    /// <para>未指定ch为true时候, 返回已添加的元素数量. 如果指定ch为true, 则返回已更改的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoAdd">命令文档</see></para>
    /// </remarks>
    public Task<int> GeoAddAsync(SharpValue key, ReadOnlySpan<GeoMember> positions, bool ch = false, GeoExist when = GeoExist.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoAdd(
            key: key,
            exist: when,
            ch: ch,
            positions: positions) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the distance between two members in the geospatial index represented by the sorted set.
    /// <para>Available since: 3.2.0</para>
    /// <para>返回两个地理位置间的距离</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key。</para>
    /// </param>
    /// <param name="member1">The first member.
    /// <para>第一个地理位置元素名称</para>
    /// </param>
    /// <param name="member2">The second member.
    /// <para>第二个地理位置元素名称</para>
    /// </param>
    /// <param name="unit">The unit for the returned distance: meters (M, the default), kilometers (KM), feet (FT), or miles (MI).
    /// <para>返回的距离单位, 默认为: 米</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>NULL: one or both of the elements are missing. distance as a double in the specified units.
    /// <para>如果元素不存在返回null, 否则返回距离</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoDist">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoDist">命令文档</see></para>
    /// </remarks>
    public Task<double?> GeoDistAsync(SharpValue key, SharpValue member1, SharpValue member2, GeoDistUnit unit = GeoDistUnit.None, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoDist(key, member1, member2, unit) | flags, cancellationToken);
    }

    /// <summary>
    /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index (where elements were added using GEOADD).
    /// <para>Available since: 3.2.0</para>
    /// <para>获得一个地理位置所在集合中的位置</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key。</para>
    /// </param>
    /// <param name="member">One member whose Geohash strings to return.
    /// <para>要获得的元素名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoHash">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoHash">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    GeoHashAsync(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoHash(key, member) | flags, cancellationToken);
    }

    /// <summary>
    /// Return valid Geohash strings representing the position of one or more elements in a sorted set value representing a geospatial index (where elements were added using GEOADD).
    /// <para>Available since: 3.2.0</para>
    /// <para>获得一个地理位置所在集合中的位置</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key。</para>
    /// </param>
    /// <param name="members">One or more members whose Geohash strings to return.
    /// <para>要获得的元素名称集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is the Geohash corresponding to each member name passed as an argument to the command.
    /// <para>数组, 按传入元素名称顺序对应的元素位置值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoHash">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoHash">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?[]>
#else
    Task<string[]>
#endif
    GeoHashAsync(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoHash(key, members) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
    /// <para>Available since: 3.2.0</para>
    /// <para>获得指定地理元素的经纬度</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key。</para>
    /// </param>
    /// <param name="member">One member whose longitude and latitude to return.
    /// <para>要获得的经纬度元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoPos">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoPos">命令文档</see></para>
    /// </remarks>
    public Task<GeoCoordinate?> GeoPosAsync(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoPos(key, member) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
    /// <para>Available since: 3.2.0</para>
    /// <para>获得指定地理元素的经纬度</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key。</para>
    /// </param>
    /// <param name="members">One or more members whose longitude and latitude to return.
    /// <para>要获得的经纬度元素数组</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The array of the element's latitude and longitude, and null is used for elements that do not exist.
    /// <para>元素的经纬度数组, 如果元素不存在, 返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoPos">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoPos">命令文档</see></para>
    /// </remarks>
    public Task<GeoCoordinate?[]> GeoPosAsync(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoPos(key, members) | flags, cancellationToken);
    }

    #region GeoRadius
    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: pos,
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: pos,
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: pos,
            radius: new(radius, unit),
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: pos,
            radius: radius,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: new(radius, unit),
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: radius,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素, 将结果存入新的Key中</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="storeKey">Store the results as a geospatial index in key instead of returning them.
    /// <para>存储结果的Key</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入结果Key, 而不是将地理位置存入结果Key</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that match and are stored in the specified Key
    /// <para>匹配并存入指定Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoRadiusStoreAsync(
        SharpValue key,
        SharpValue storeKey,
        bool storeDist,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: pos,
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: storeKey,
            storeDist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素, 将结果存入新的Key中</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="storeKey">Store the results as a geospatial index in key instead of returning them.
    /// <para>存储结果的Key</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入结果Key, 而不是将地理位置存入结果Key</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that match and are stored in the specified Key
    /// <para>匹配并存入指定Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoRadiusStoreAsync(
        SharpValue key,
        SharpValue storeKey,
        bool storeDist,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: pos,
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: storeKey,
            storeDist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素, 将结果存入新的Key中</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="storeKey">Store the results as a geospatial index in key instead of returning them.
    /// <para>存储结果的Key</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入结果Key, 而不是将地理位置存入结果Key</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that match and are stored in the specified Key
    /// <para>匹配并存入指定Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoRadiusStoreAsync(
        SharpValue key,
        SharpValue storeKey,
        bool storeDist,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: storeKey,
            storeDist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>获得指定经纬度为中心, 指定半径范围内的元素, 将结果存入新的Key中</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="storeKey">Store the results as a geospatial index in key instead of returning them.
    /// <para>存储结果的Key</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入结果Key, 而不是将地理位置存入结果Key</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that match and are stored in the specified Key
    /// <para>匹配并存入指定Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoRadiusStoreAsync(
        SharpValue key,
        SharpValue storeKey,
        bool storeDist,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: storeKey,
            storeDist: storeDist) | flags, cancellationToken);
    }
    #endregion

    #region GeoRadius_Ro
    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: pos,
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: pos,
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: pos,
            radius: new(radius, unit),
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken);
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">The longitude and latitude of the center point
    /// <para>中心点的经纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: pos,
            radius: radius,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken);
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: new(radius, unit),
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken);
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUS</b> command. This command is identical to the <b>GEORADIUS</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUS</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadius_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadius_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadius_RoAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadius_Ro(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            radius: radius,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken);
    }
    #endregion

    #region GeoRadiusByMember
    /// <summary>
    /// This command is exactly like <b>GEORADIUS</b> with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set.
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>此命令和<b>GEORADIUS</b>行为结果完全一致, 只不过不是传入经纬度为中心, 而是指定集合中存在的地理位置元素为中心</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusByMemberAsync(
        SharpValue key,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember(
            key: key,
            member: member,
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// This command is exactly like <b>GEORADIUS</b> with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set.
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>此命令和<b>GEORADIUS</b>行为结果完全一致, 只不过不是传入经纬度为中心, 而是指定集合中存在的地理位置元素为中心</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusByMemberAsync(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember(
            key: key,
            member: member,
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// This command is exactly like <b>GEORADIUS</b> with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set.
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>此命令和<b>GEORADIUS</b>行为结果完全一致, 只不过不是传入经纬度为中心, 而是指定集合中存在的地理位置元素为中心</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusByMemberAsync(
        SharpValue key,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember(
            key: key,
            member: member,
            radius: new(radius, unit),
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is exactly like <b>GEORADIUS</b> with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set.
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>此命令和<b>GEORADIUS</b>行为结果完全一致, 只不过不是传入经纬度为中心, 而是指定集合中存在的地理位置元素为中心</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusByMemberAsync(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember(
            key: key,
            member: member,
            radius: radius,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort,
            storeKey: null,
            storeDist: false) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is exactly like <b>GEORADIUS</b> with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set.
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>此命令和<b>GEORADIUS</b>行为结果完全一致, 只不过不是传入经纬度为中心, 而是指定集合中存在的地理位置元素为中心</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="storeKey">Store the results as a geospatial index in key instead of returning them.
    /// <para>存储结果的Key</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入结果Key, 而不是将地理位置存入结果Key</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that match and are stored in the specified Key
    /// <para>匹配并存入指定Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoRadiusByMemberStoreAsync(
        SharpValue key,
        SharpValue storeKey,
        bool storeDist,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember(
            key: key,
            member: member,
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: storeKey,
            storeDist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is exactly like <b>GEORADIUS</b> with the sole difference that instead of taking, as the center of the area to query, a longitude and latitude value, it takes the name of a member already existing inside the geospatial index represented by the sorted set.
    /// <para>Available since: 3.2.0 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para>此命令和<b>GEORADIUS</b>行为结果完全一致, 只不过不是传入经纬度为中心, 而是指定集合中存在的地理位置元素为中心</para>
    /// <para>支持此命令的Redis版本, 3.2.0+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="storeKey">Store the results as a geospatial index in key instead of returning them.
    /// <para>存储结果的Key</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入结果Key, 而不是将地理位置存入结果Key</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that match and are stored in the specified Key
    /// <para>匹配并存入指定Key中的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoRadiusByMemberStoreAsync(
        SharpValue key,
        SharpValue storeKey,
        bool storeDist,
        SharpValue member,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember(
            key: key,
            member: member,
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort,
            storeKey: storeKey,
            storeDist: storeDist) | flags, cancellationToken);
    }
    #endregion

    #region GeoRadiusByMember_Ro
    /// <summary>
    /// Read-only variant of the <b>GEORADIUSBYMEMBER</b> command. This command is identical to the <b>GEORADIUSBYMEMBER</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUSBYMEMBER</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusByMember_RoAsync(
        SharpValue key,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember_Ro(
            key: key,
            member: member,
            radius: new(radius, unit),
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUSBYMEMBER</b> command. This command is identical to the <b>GEORADIUSBYMEMBER</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUSBYMEMBER</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoRadiusByMember_RoAsync(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember_Ro(
            key: key,
            member: member,
            radius: radius,
            withCoord: false,
            withDist: false,
            withHash: false,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUSBYMEMBER</b> command. This command is identical to the <b>GEORADIUSBYMEMBER</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUSBYMEMBER</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusByMember_RoAsync(
        SharpValue key,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember_Ro(
            key: key,
            member: member,
            radius: new(radius, unit),
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken);
    }

    /// <summary>
    /// Read-only variant of the <b>GEORADIUSBYMEMBER</b> command. This command is identical to the <b>GEORADIUSBYMEMBER</b> command, except that it doesn't support the optional STORE and STOREDIST parameters.
    /// <para>Available since: 3.2.10 <b>[As of Redis version 6.2.0, this command is regarded as deprecated.]</b></para>
    /// <para><b>GEORADIUSBYMEMBER</b> 命令的只读版, 此命令无法设置STORE 和 STOREDIST参数</para>
    /// <para>支持此命令的Redis版本, 3.2.10+ <b>此命令在Redis 6.2.0+ 被视为过期的, 弃用的</b></para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para><b>Starting from version 6.2.0 of Redis, this parameter has been supported.</b></para>
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// <para><b>Redis 6.2.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoRadiusByMember_Ro">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoRadiusByMember_Ro">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoRadiusByMember_RoAsync(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count = null,
        bool any = false,
        GeoSort sort = GeoSort.None,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoRadiusByMember_Ro(
            key: key,
            member: member,
            radius: radius,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash,
            count: count,
            any: any,
            sort: sort) | flags, cancellationToken);
    }
    #endregion

    #region GeoSearch
    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        GeoBoxArg box,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        double width,
        double height,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoBoxArg box,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoBoxArg box,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        double width,
        double height,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of geographical location element names within the range
    /// <para>范围内的, 地理位置元素名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double width,
        double height,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            withCoord: false,
            withDist: false,
            withHash: false) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的半径, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        GeoBoxArg box,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        SharpValue member,
        double width,
        double height,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: null,
            member: member,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        GeoBoxArg box,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        GeoBoxArg box,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        GeoCoordinate pos,
        double width,
        double height,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: pos,
            member: null,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }

    /// <summary>
    /// Return the members of a sorted set populated with geospatial information using GEOADD, which are within the borders of the area specified by a given shape. 
    /// <para>Available since: 6.2.0</para>
    /// <para>根据指定的宽度和高度, 搜索范围内的地理位置成员</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the geospatial index (a sorted set).
    /// <para>存储地理空间索引（有序集合）的Key</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="withCoord">Also return the longitude and latitude of each matching item.
    /// <para>结果中是否要包含经纬度</para>
    /// </param>
    /// <param name="withDist">Also return the distance of each matching item from the center, in the same unit as the radius.
    /// <para>结果中是否要包含距离中心点的距离, 单位为unit指定的单位</para>
    /// </param>
    /// <param name="withHash">Also return the raw 52-bit geohash-encoded score of each matching item.
    /// <para>结果中是否要包含位置信息的哈希编码值</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>返回的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearch">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearch">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GeoSearchResult[]?>
#else
    Task<GeoSearchResult[]>
#endif
    GeoSearchAsync(
        SharpValue key,
        double longitude,
        double latitude,
        double width,
        double height,
        GeoDistUnit unit,
        bool withCoord,
        bool withDist,
        bool withHash,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearch(
            key: key,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            withCoord: withCoord,
            withDist: withDist,
            withHash: withHash) | flags, cancellationToken);
    }
    #endregion

    #region GeoSearchStore
    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        SharpValue member,
        GeoRadiusArg radius,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: null,
            member: member,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        SharpValue member,
        double radius,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: null,
            member: member,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        SharpValue member,
        GeoBoxArg box,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: null,
            member: member,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="member">The member whose position is used as the center of the search.
    /// <para>作为中心的地理位置元素名称</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        SharpValue member,
        double width,
        double height,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: null,
            member: member,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: pos,
            member: null,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        GeoCoordinate pos,
        double radius,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: pos,
            member: null,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="unit">The unit for radius: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>半径单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        double longitude,
        double latitude,
        double radius,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: new(radius, unit),
            box: null,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="radius">The radius of the search circle
    /// <para>半径范围</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        double longitude,
        double latitude,
        GeoRadiusArg radius,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: radius,
            box: null,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        GeoCoordinate pos,
        GeoBoxArg box,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: pos,
            member: null,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="box">Search within an axis-aligned box of the given width and height in the specified unit
    /// <para>搜索的矩形范围宽高度</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        double longitude,
        double latitude,
        GeoBoxArg box,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: null,
            box: box,
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="pos">Center point latitude and longitude coordinates
    /// <para>中心点经纬度坐标</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        GeoCoordinate pos,
        double width,
        double height,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: pos,
            member: null,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }

    /// <summary>
    /// This command is similar to <b>GEOSEARCH</b>, but it stores the result in the provided destination-key.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令和 <b>GEOSEARCH</b> 类似, 只不过不返回结果, 而是将结果存入目标key中</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="destinationKey">The key to store the results in.
    /// <para>存储搜索结果的目标Key</para>
    /// </param>
    /// <param name="sourceKey">The geospatial index key to search.
    /// <para>要搜索的地理位置来源Key (源key)</para>
    /// </param>
    /// <param name="longitude">The longitude of the center point.
    /// <para>中心点经度</para>
    /// </param>
    /// <param name="latitude">The latitude of the center point.
    /// <para>中心点纬度</para>
    /// </param>
    /// <param name="width">Range width
    /// <para>范围宽度</para>
    /// </param>
    /// <param name="height">Range height
    /// <para>范围高度</para>
    /// </param>
    /// <param name="unit">The unit for width and heighr: meters (M), kilometers (KM), feet (FT), or miles (MI).
    /// <para>宽高度单位, 米（ M ）、千米（ KM ）、英尺（ FT ）或英里（ MI ）</para>
    /// </param>
    /// <param name="sort">Sort the results by distance from the center: nearest first (ASC) or farthest first (DESC).
    /// <para>排序方式, 距离升序或者降序</para>
    /// </param>
    /// <param name="count">Return at most count matches
    /// <para>匹配的最多元素数量</para>
    /// </param>
    /// <param name="any">With ANY, the command returns as soon as enough matches are found — faster, but the results may be unsorted.
    /// <para>使用 ANY 时，命令会在找到足够匹配项后立即返回——速度更快，但结果可能未排序。</para>
    /// </param>
    /// <param name="storeDist">Store the distance from the center as the sorted-set score, instead of the geohash-encoded position.
    /// <para>是否将距离存入目标Key, 而不是将经纬度编码存入目标Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set
    /// <para>存入目标Key中的, 匹配的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/GeoSearchStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/GeoSearchStore">命令文档</see></para>
    /// </remarks>
    public Task<long> GeoSearchStoreAsync(
        SharpValue destinationKey,
        SharpValue sourceKey,
        double longitude,
        double latitude,
        double width,
        double height,
        GeoDistUnit unit,
        GeoSort sort = GeoSort.None,
        long? count = null,
        bool any = false,
        bool storeDist = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(GeospatialCommands.GeoSearchStore(
            destination: destinationKey,
            source: sourceKey,
            pos: new(longitude: longitude, latitude: latitude),
            member: null,
            radius: null,
            box: new(width, height, unit),
            sort: sort,
            count: count,
            any: any,
            storedist: storeDist) | flags, cancellationToken);
    }
    #endregion
}
