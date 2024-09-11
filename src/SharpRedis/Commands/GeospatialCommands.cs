using SharpRedis.Models;
using System.Collections.Generic;

namespace SharpRedis.Commands
{
    internal static class GeospatialCommands
    {
        private static CommandPacket GeoAdd(string key, NxXx nxx, bool ch)
        {
            return new CommandPacket("GEOADD", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(nxx != NxXx.None, nxx == NxXx.Xx ? "XX" : "NX")
                .WriteArg(ch, "CH");
        }

        internal static CommandPacket GeoAdd<TMember>(string key, TMember member, double longitude, double latitude, NxXx nxx, bool ch)
            where TMember : class
        {
            return GeospatialCommands.GeoAdd(key, nxx, ch)
                .WriteArg(longitude)
                .WriteArg(latitude)
                .WriteValue(member);
        }

        internal static CommandPacket GeoAdd<TMember>(string key, Dictionary<TMember, CoordinateValue> members, NxXx nxx, bool ch)
            where TMember : class
        {
            var command = GeospatialCommands.GeoAdd(key, nxx, ch);
            foreach (var member in members)
            {
                command
                    .WriteArg(member.Value.Longitude)
                    .WriteArg(member.Value.Latitude)
                    .WriteValue(member.Key);
            }
            return command;
        }

        internal static CommandPacket GeoDist<TMember>(string key, TMember member1, TMember member2, DistanceUnit unit)
            where TMember : class
        {
            return new CommandPacket("GEODIST", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(member1)
                .WriteValue(member2)
                .WriteArg(unit is DistanceUnit.km, "km")
                .WriteArg(unit is DistanceUnit.ft, "ft")
                .WriteArg(unit is DistanceUnit.mi, "mi");
        }

        internal static CommandPacket GeoHash<TMember>(string key, params TMember[] members)
            where TMember : class
        {
            if (members is null || members.Length == 0) throw new RedisException("Please ensure at least one member");
            return new CommandPacket("GEOHASH", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValues(members);
        }

        internal static CommandPacket GeoPos<TMember>(string key, params TMember[] members)
            where TMember : class
        {
            if (members is null || members.Length == 0) throw new RedisException("Please ensure at least one member");
            return new CommandPacket("GEOPOS", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValues(members);
        }

        internal static CommandPacket GeoRadius<TRadius>(
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord = false,
            bool withDist = false,
            bool withHash = false,
            ulong count = 0,
            bool any = false,
            OrderType order = OrderType.Default,
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            string? storeKey = null,
            string? storeDistKey = null
#else
            string storeKey = null,
            string storeDistKey = null
#endif
            )
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            if (storeKey != null && storeDistKey != null) throw new RedisException("[STORE] and [STOREDIST] cannot be set at the same time");
            return new CommandPacket("GEORADIUS", CommandMode.Write)
                .WriteKey(key)
                .WriteArg(longitude)
                .WriteArg(latitude)
                .WriteArg(radius)
                .WriteArg(unit is DistanceUnit.m, "m")
                .WriteArg(unit is DistanceUnit.km, "km")
                .WriteArg(unit is DistanceUnit.ft, "ft")
                .WriteArg(unit is DistanceUnit.mi, "mi")
                .WriteArg(withCoord, "WITHCOORD")
                .WriteArg(withDist, "WITHDIST")
                .WriteArg(withHash, "WITHHASH")
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(any && count > 0, "ANY")
                .WriteArg(order == OrderType.Asc, "ASC")
                .WriteArg(order == OrderType.Desc, "DESC")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArgKey(storeKey != null, "STORE", storeKey!)
                .WriteArgKey(storeDistKey != null, "STOREDIST", storeDistKey!)
#else
                .WriteArgKey(storeKey != null, "STORE", storeKey)
                .WriteArgKey(storeDistKey != null, "STOREDIST", storeDistKey)
#endif
                ;
        }

        internal static CommandPacket GeoRadius_Ro<TRadius>(
            string key,
            double longitude,
            double latitude,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord = false,
            bool withDist = false,
            bool withHash = false,
            ulong count = 0,
            bool any = false,
            OrderType order = OrderType.Default
            )
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
        {
            return new CommandPacket("GEORADIUS_RO", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteArg(longitude)
                .WriteArg(latitude)
                .WriteArg(radius)
                .WriteArg(unit is DistanceUnit.m, "m")
                .WriteArg(unit is DistanceUnit.km, "km")
                .WriteArg(unit is DistanceUnit.ft, "ft")
                .WriteArg(unit is DistanceUnit.mi, "mi")
                .WriteArg(withCoord, "WITHCOORD")
                .WriteArg(withDist, "WITHDIST")
                .WriteArg(withHash, "WITHHASH")
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(any && count > 0, "ANY")
                .WriteArg(order == OrderType.Asc, "ASC")
                .WriteArg(order == OrderType.Desc, "DESC");
        }

        internal static CommandPacket GeoRadiusByMember<TRadius, TMember>(
            string key,
            TMember member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord = false,
            bool withDist = false,
            bool withHash = false,
            ulong count = 0,
            bool any = false,
            OrderType order = OrderType.Default,
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            string? storeKey = null,
            string? storeDistKey = null
#else
            string storeKey = null,
            string storeDistKey = null
#endif
            )
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
            where TMember : class
        {
            if (storeKey != null && storeDistKey != null) throw new RedisException("[STORE] and [STOREDIST] cannot be set at the same time");
            return new CommandPacket("GEORADIUSBYMEMBER", CommandMode.Write)
                .WriteKey(key)
                .WriteValue(member)
                .WriteArg(radius)
                .WriteArg(unit is DistanceUnit.m, "m")
                .WriteArg(unit is DistanceUnit.km, "km")
                .WriteArg(unit is DistanceUnit.ft, "ft")
                .WriteArg(unit is DistanceUnit.mi, "mi")
                .WriteArg(withCoord, "WITHCOORD")
                .WriteArg(withDist, "WITHDIST")
                .WriteArg(withHash, "WITHHASH")
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(any && count > 0, "ANY")
                .WriteArg(order is OrderType.Asc, "ASC")
                .WriteArg(order is OrderType.Desc, "DESC")
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteArgKey(storeKey != null, "STORE", storeKey!)
                .WriteArgKey(storeDistKey != null, "STOREDIST", storeDistKey!)
#else
                .WriteArgKey(storeKey != null, "STORE", storeKey)
                .WriteArgKey(storeDistKey != null, "STOREDIST", storeDistKey)
#endif
                ;
        }

        internal static CommandPacket GeoRadiusByMember_Ro<TRadius, TMember>(
            string key,
            TMember member,
            TRadius radius,
            DistanceUnit unit,
            bool withCoord = false,
            bool withDist = false,
            bool withHash = false,
            ulong count = 0,
            bool any = false,
            OrderType order = OrderType.Default)
#if NET7_0_OR_GREATER
            where TRadius : struct, System.Numerics.INumber<TRadius>
#else
            where TRadius : struct, System.IEquatable<TRadius>
#endif
            where TMember : class
        {
            return new CommandPacket("GEORADIUSBYMEMBER_RO", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
                .WriteValue(member)
                .WriteArg(radius)
                .WriteArg(unit is DistanceUnit.m, "m")
                .WriteArg(unit is DistanceUnit.km, "km")
                .WriteArg(unit is DistanceUnit.ft, "ft")
                .WriteArg(unit is DistanceUnit.mi, "mi")
                .WriteArg(withCoord, "WITHCOORD")
                .WriteArg(withDist, "WITHDIST")
                .WriteArg(withHash, "WITHHASH")
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(any && count > 0, "ANY")
                .WriteArg(order is OrderType.Asc, "ASC")
                .WriteArg(order is OrderType.Desc, "DESC");
        }

        internal static CommandPacket GeoSearch<TCoordinate, TMember>(
            string key,
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            TMember? member,
#else
            TMember member,
#endif
            double? longitude,
            double? latitude,
            TCoordinate? radius,
            TCoordinate? width,
            TCoordinate? height,
            DistanceUnit unit,
            OrderType order = OrderType.Default,
            ulong count = 0,
            bool any = false,
            bool withCoord = false,
            bool withDist = false,
            bool withHash = false)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
            where TMember : class
        {
            return new CommandPacket("GEOSEARCH", CommandMode.Read | CommandMode.WithClientSideCache)
                .WriteKey(key)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteValue(member != null, "FROMMEMBER", member!)
#else
                .WriteValue(member != null, "FROMMEMBER", member)
#endif
                .WriteArg(longitude.HasValue && latitude.HasValue, "FROMLONLAT", longitude ?? default, latitude ?? default)
                .WriteArg(radius.HasValue, "BYRADIUS", radius ?? default)
                .WriteArg(width.HasValue && height.HasValue, "BYBOX", width ?? default, height ?? default)
                .WriteArg(unit is DistanceUnit.m, "m")
                .WriteArg(unit is DistanceUnit.km, "km")
                .WriteArg(unit is DistanceUnit.ft, "ft")
                .WriteArg(unit is DistanceUnit.mi, "mi")
                .WriteArg(order is OrderType.Asc, "ASC")
                .WriteArg(order is OrderType.Desc, "DESC")
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(any && count > 0, "ANY")
                .WriteArg(withCoord, "WITHCOORD")
                .WriteArg(withDist, "WITHDIST")
                .WriteArg(withHash, "WITHHASH");
        }

        internal static CommandPacket GeoSearchStore<TCoordinate, TMember>(
            string destination,
            string source,
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            TMember? member,
#else
            TMember member,
#endif
            double? longitude,
            double? latitude,
            TCoordinate? radius,
            TCoordinate? width,
            TCoordinate? height,
            DistanceUnit unit,
            OrderType order = OrderType.Default,
            ulong count = 0,
            bool any = false,
            bool storeDist = false)
#if NET7_0_OR_GREATER
            where TCoordinate : struct, System.Numerics.INumber<TCoordinate>
#else
            where TCoordinate : struct, System.IEquatable<TCoordinate>
#endif
            where TMember : class
        {
            return new CommandPacket("GEOSEARCHSTORE", CommandMode.Write)
                .WriteKey(destination)
                .WriteKey(source)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                .WriteValue(member != null, "FROMMEMBER", member!)
#else
                .WriteValue(member != null, "FROMMEMBER", member)
#endif
                .WriteArg(longitude.HasValue && latitude.HasValue, "FROMLONLAT", longitude ?? default, latitude ?? default)
                .WriteArg(radius.HasValue, "BYRADIUS", radius ?? default)
                .WriteArg(width.HasValue && height.HasValue, "BYBOX", width ?? default, height ?? default)
                .WriteArg(unit is DistanceUnit.m, "m")
                .WriteArg(unit is DistanceUnit.km, "km")
                .WriteArg(unit is DistanceUnit.ft, "ft")
                .WriteArg(unit is DistanceUnit.mi, "mi")
                .WriteArg(order is OrderType.Asc, "ASC")
                .WriteArg(order is OrderType.Desc, "DESC")
                .WriteArg(count > 0, "COUNT", count)
                .WriteArg(any && count > 0, "ANY")
                .WriteArg(storeDist, "STOREDIST");
        }
    }
}
