using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using System;
using System.Collections.Generic;
using static SharpRedis.Modules.RedisSearch;

namespace SharpRedis.Commands;

internal static class RedisSearchCommands
{
    private static readonly SharpValue _ftcreate = new("FT.CREATE", CommandItemCategory.Command);
    private static readonly SharpValue _ft_list = new("FT._LIST", CommandItemCategory.Command);

    internal static CommandPacket FT_Create(
        string index,
        Field field,
#if MODERN_NET
        CreateIndexOptions? indexOptions
#else
        CreateIndexOptions indexOptions
#endif
    )
    {
        if (field is null) throw new ArgumentNullException(nameof(field), "The field cannot be null.");
        var command = new CommandPacket(RedisSearchCommands._ftcreate, CommandMode.Search | CommandMode.WithWrite)
            .WriteKeys(index);
        _ = indexOptions?.AppendToCommand(ref command);
        _ = command.WriteArguments("SCHEMA");
        return field.AppendToCommand(ref command);
    }

    internal static CommandPacket FT_Create(
        string index,
        ReadOnlySpan<Field> schema,
#if MODERN_NET
        CreateIndexOptions? indexOptions
#else
        CreateIndexOptions indexOptions
#endif
    )
    {
        if (schema.IsEmpty) throw new ArgumentException("Please ensure that there is at least one field in the schema.", nameof(schema));
        var command = new CommandPacket(RedisSearchCommands._ftcreate, CommandMode.Search | CommandMode.WithWrite)
            .WriteKeys(index);
        _ = indexOptions?.AppendToCommand(ref command);
        _ = command.WriteArguments("SCHEMA");
        foreach (var field in schema)
        {
            if (field is null) throw new ArgumentException("The field cannot be null.");
            _ = field.AppendToCommand(ref command);
        }
        return command;
    }

    internal static CommandPacket FT_Create(
        string index,
        ICollection<Field> schema,
#if MODERN_NET
        CreateIndexOptions? indexOptions
#else
        CreateIndexOptions indexOptions
#endif
    )
    {
        if (schema is null or { Count: <= 0 }) throw new ArgumentException("Please ensure that there is at least one field in the schema.", nameof(schema));
        var command = new CommandPacket(RedisSearchCommands._ftcreate, CommandMode.Search | CommandMode.WithWrite)
            .WriteKeys(index);
        _ = indexOptions?.AppendToCommand(ref command);
        _ = command.WriteArguments("SCHEMA");
        foreach (var field in schema)
        {
            if (field is null) throw new ArgumentException("The field cannot be null.");
            _ = field.AppendToCommand(ref command);
        }
        return command;
    }

    internal static CommandPacket FT__List()
    {
        return new CommandPacket(RedisSearchCommands._ft_list, CommandMode.Search);
    }
}
