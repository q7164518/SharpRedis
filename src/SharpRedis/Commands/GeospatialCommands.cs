using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using static SharpRedis.RedisGeospatial;

namespace SharpRedis.Commands;

internal static class GeospatialCommands
{
    private static readonly SharpValue _geoadd = new("GEOADD", CommandItemCategory.Command);
    private static readonly SharpValue _geodist = new("GEODIST", CommandItemCategory.Command);
    private static readonly SharpValue _geohash = new("GEOHASH", CommandItemCategory.Command);
    private static readonly SharpValue _geopos = new("GEOPOS", CommandItemCategory.Command);
    private static readonly SharpValue _georadius = new("GEORADIUS", CommandItemCategory.Command);
    private static readonly SharpValue _georadius_ro = new("GEORADIUS_RO", CommandItemCategory.Command);
    private static readonly SharpValue _georadiusbymember = new("georadiusbymember", CommandItemCategory.Command);
    private static readonly SharpValue _georadiusbymember_ro = new("georadiusbymember_ro", CommandItemCategory.Command);
    private static readonly SharpValue _geosearch = new("GEOSEARCH", CommandItemCategory.Command);
    private static readonly SharpValue _geosearchstore = new("GEOSEARCHSTORE", CommandItemCategory.Command);

    internal static CommandPacket GeoAdd(SharpValue key, GeoExist exist, bool ch, GeoMember position)
    {
        return new CommandPacket(GeospatialCommands._geoadd, CommandMode.Geo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(exist is GeoExist.NX, "NX")
            .WriteArguments(exist is GeoExist.XX, "XX")
            .WriteArguments(ch, "CH")
            .WriteArguments(position.Longitude.ToBytes())
            .WriteArguments(position.Latitude.ToBytes())
            .WriteArguments(position.Member);
    }

    internal static CommandPacket GeoAdd(SharpValue key, GeoExist exist, bool ch, ReadOnlySpan<GeoMember> positions)
    {
        if (positions.IsEmpty) throw new NotSupportedException("Please specify at least one position.");
        var command = new CommandPacket(GeospatialCommands._geoadd, CommandMode.Geo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(exist is GeoExist.NX, "NX")
            .WriteArguments(exist is GeoExist.XX, "XX")
            .WriteArguments(ch, "CH");
        foreach (var position in positions)
        {
            _ = command.WriteArguments(position.Longitude.ToBytes())
                .WriteArguments(position.Latitude.ToBytes())
                .WriteArguments(position.Member);
        }
        return command;
    }

    internal static CommandPacket GeoDist(SharpValue key, SharpValue member1, SharpValue member2, GeoDistUnit unit)
    {
        var command = new CommandPacket(GeospatialCommands._geodist, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member1)
            .WriteArguments(member2);
        return unit switch
        {
            GeoDistUnit.M => command.WriteArguments("m"),
            GeoDistUnit.KM => command.WriteArguments("km"),
            GeoDistUnit.MI => command.WriteArguments("mi"),
            GeoDistUnit.FT => command.WriteArguments("ft"),
            _ => command,
        };
    }

    internal static CommandPacket GeoHash(SharpValue key, SharpValue member)
    {
        return new CommandPacket(GeospatialCommands._geohash, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member);
    }

    internal static CommandPacket GeoHash(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(GeospatialCommands._geohash, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(members);
    }

    internal static CommandPacket GeoPos(SharpValue key, SharpValue member)
    {
        return new CommandPacket(GeospatialCommands._geopos, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member);
    }

    internal static CommandPacket GeoPos(SharpValue key, SharpValueSpan members)
    {
        if (members.IsEmpty) throw new NotSupportedException("Please specify at least one member.");
        return new CommandPacket(GeospatialCommands._geopos, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(members);
    }

    internal static CommandPacket GeoRadius(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count,
        bool any,
        GeoSort sort,
        SharpValue? storeKey,
        bool storeDist)
    {
        return new CommandPacket(GeospatialCommands._georadius, CommandMode.Geo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(pos.Longitude.ToBytes())
            .WriteArguments(pos.Latitude.ToBytes())
            .AppendRadiusArgs(
                radius: radius,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                sort: sort,
                storeKey: storeKey,
                storeDist: storeDist);
    }

    internal static CommandPacket GeoRadius_Ro(
        SharpValue key,
        GeoCoordinate pos,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count,
        bool any,
        GeoSort sort)
    {
        return new CommandPacket(GeospatialCommands._georadius_ro, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(pos.Longitude.ToBytes())
            .WriteArguments(pos.Latitude.ToBytes())
            .AppendRadiusArgs(
                radius: radius,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                sort: sort,
                storeKey: null,
                storeDist: false);
    }

    internal static CommandPacket GeoRadiusByMember(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count,
        bool any,
        GeoSort sort,
        SharpValue? storeKey,
        bool storeDist)
    {
        return new CommandPacket(GeospatialCommands._georadiusbymember, CommandMode.Geo | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(member)
            .AppendRadiusArgs(
                radius: radius,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                sort: sort,
                storeKey: storeKey,
                storeDist: storeDist);
    }

    internal static CommandPacket GeoRadiusByMember_Ro(
        SharpValue key,
        SharpValue member,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count,
        bool any,
        GeoSort sort)
    {
        return new CommandPacket(GeospatialCommands._georadiusbymember_ro, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .WriteArguments(member)
            .AppendRadiusArgs(
                radius: radius,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                count: count,
                any: any,
                sort: sort,
                storeKey: null,
                storeDist: false);
    }

    private static CommandPacket AppendRadiusArgs(
        this CommandPacket command,
        GeoRadiusArg radius,
        bool withCoord,
        bool withDist,
        bool withHash,
        long? count,
        bool any,
        GeoSort sort,
        SharpValue? storeKey,
        bool storeDist)
    {
        _ = command.WriteArguments(radius.Radius.ToBytes());
        _ = radius.Unit switch
        {
            GeoDistUnit.M => command.WriteArguments("m"),
            GeoDistUnit.KM => command.WriteArguments("km"),
            GeoDistUnit.MI => command.WriteArguments("mi"),
            GeoDistUnit.FT => command.WriteArguments("ft"),
            _ => throw new NotSupportedException("Please specify the unit of the radius."),
        };

        _ = command
            .WriteArguments(withCoord, "WITHCOORD")
            .WriteArguments(withDist, "WITHDIST")
            .WriteArguments(withHash, "WITHHASH");

        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes())
                .WriteArguments(any, "ANY");
        }

        switch (sort)
        {
            case GeoSort.Asc:
                _ = command.WriteArguments("ASC");
                break;
            case GeoSort.Desc:
                _ = command.WriteArguments("DESC");
                break;
            default:
                break;
        }

        if (storeKey.HasValue)
        {
            if (storeDist)
            {
                _ = command
                .WriteArguments("STORE")
                .WriteKeys(storeKey.Value);
            }
            else
            {
                _ = command
                    .WriteArguments("STOREDIST")
                    .WriteKeys(storeKey.Value);
            }
        }
        return command;
    }

    internal static CommandPacket GeoSearch(
        SharpValue key,
        GeoCoordinate? pos,
        SharpValue? member,
        GeoRadiusArg? radius,
        GeoBoxArg? box,
        GeoSort sort,
        long? count,
        bool any,
        bool withCoord,
        bool withDist,
        bool withHash)
    {
        return new CommandPacket(GeospatialCommands._geosearch, CommandMode.Geo | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key)
            .AppendGeoSearchArgs(
                pos: pos,
                member: member,
                radius: radius,
                box: box,
                sort: sort,
                count: count,
                any: any,
                withCoord: withCoord,
                withDist: withDist,
                withHash: withHash,
                storedist: false);
    }

    internal static CommandPacket GeoSearchStore(
        SharpValue destination,
        SharpValue source,
        GeoCoordinate? pos,
        SharpValue? member,
        GeoRadiusArg? radius,
        GeoBoxArg? box,
        GeoSort sort,
        long? count,
        bool any,
        bool storedist)
    {
        return new CommandPacket(GeospatialCommands._geosearchstore, CommandMode.Geo | CommandMode.WithWrite)
            .WriteKeys(destination)
            .WriteKeys(source)
            .AppendGeoSearchArgs(
                pos: pos,
                member: member,
                radius: radius,
                box: box,
                sort: sort,
                count: count,
                any: any,
                withCoord: false,
                withDist: false,
                withHash: false,
                storedist: storedist);
    }

    private static CommandPacket AppendGeoSearchArgs(
        this CommandPacket command,
        GeoCoordinate? pos,
        SharpValue? member,
        GeoRadiusArg? radius,
        GeoBoxArg? box,
        GeoSort sort,
        long? count,
        bool any,
        bool withCoord,
        bool withDist,
        bool withHash,
        bool storedist)
    {
        if (member.HasValue)
        {
            _ = command
                .WriteArguments("FROMMEMBER")
                .WriteArguments(member.Value);
        }

        if (pos.HasValue)
        {
            _ = command
                .WriteArguments("FROMLONLAT")
                .WriteArguments(pos.Value.Longitude.ToBytes())
                .WriteArguments(pos.Value.Latitude.ToBytes());
        }

        if (radius.HasValue)
        {
            _ = command
               .WriteArguments("BYRADIUS")
               .WriteArguments(radius.Value.Radius.ToBytes());
            _ = radius.Value.Unit switch
            {
                GeoDistUnit.M => command.WriteArguments("m"),
                GeoDistUnit.KM => command.WriteArguments("km"),
                GeoDistUnit.MI => command.WriteArguments("mi"),
                GeoDistUnit.FT => command.WriteArguments("ft"),
                _ => throw new NotSupportedException("Please specify the unit of the radius."),
            };
        }

        if (box.HasValue)
        {
            _ = command
               .WriteArguments("BYBOX")
               .WriteArguments(box.Value.Width.ToBytes())
               .WriteArguments(box.Value.Height.ToBytes());
            _ = box.Value.Unit switch
            {
                GeoDistUnit.M => command.WriteArguments("m"),
                GeoDistUnit.KM => command.WriteArguments("km"),
                GeoDistUnit.MI => command.WriteArguments("mi"),
                GeoDistUnit.FT => command.WriteArguments("ft"),
                _ => throw new NotSupportedException("Please specify the unit of the radius."),
            };
        }

        switch (sort)
        {
            case GeoSort.Asc:
                _ = command.WriteArguments("ASC");
                break;
            case GeoSort.Desc:
                _ = command.WriteArguments("DESC");
                break;
            default:
                break;
        }

        if (count.HasValue)
        {
            _ = command
                .WriteArguments("COUNT")
                .WriteArguments(count.Value.ToBytes())
                .WriteArguments(any, "ANY");
        }

        return command
            .WriteArguments(withCoord, "WITHCOORD")
            .WriteArguments(withDist, "WITHDIST")
            .WriteArguments(withHash, "WITHHASH")
            .WriteArguments(storedist, "STOREDIST");
    }
}
