using SharpRedis.Entities.Command;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;

namespace SharpRedis.Commands;

internal static class CountMinSketchCommands
{
    private static readonly SharpValue _cmsincrby = new("CMS.INCRBY", CommandItemCategory.Command);
    private static readonly SharpValue _cmsinfo = new("CMS.INFO", CommandItemCategory.Command);
    private static readonly SharpValue _cmsinitbydim = new("CMS.INITBYDIM", CommandItemCategory.Command);
    private static readonly SharpValue _cmsinitbyprob = new("CMS.INITBYPROB", CommandItemCategory.Command);
    private static readonly SharpValue _cmsmerge = new("CMS.MERGE", CommandItemCategory.Command);
    private static readonly SharpValue _cmsquery = new("CMS.QUERY", CommandItemCategory.Command);

    internal static CommandPacket CMS_IncrBy(SharpValue key, SharpValue item, long increment)
    {
        return CountMinSketchCommands.InitCMC_IncrBy(key)
            .WriteArguments(item)
            .WriteArguments(increment.ToBytes());
    }

    internal static CommandPacket CMS_IncrBy(SharpValue key, (SharpValue item, long increment) itemIncrement)
    {
        return CountMinSketchCommands.InitCMC_IncrBy(key)
            .WriteArguments(itemIncrement.item)
            .WriteArguments(itemIncrement.increment.ToBytes());
    }

    internal static CommandPacket CMS_IncrBy(SharpValue key, KeyValuePair<SharpValue, long> itemIncrement)
    {
        return CountMinSketchCommands.InitCMC_IncrBy(key)
            .WriteArguments(itemIncrement.Key)
            .WriteArguments(itemIncrement.Value.ToBytes());
    }

    internal static CommandPacket CMS_IncrBy(SharpValue key, ICollection<(SharpValue item, long increment)> itemIncrements)
    {
        if (itemIncrements is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one Item-Increment.");
        var command = CountMinSketchCommands.InitCMC_IncrBy(key);
        foreach (var item in itemIncrements)
        {
            _ = command
                .WriteArguments(item.item)
                .WriteArguments(item.increment.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_IncrBy(SharpValue key, ICollection<KeyValuePair<SharpValue, long>> itemIncrements)
    {
        if (itemIncrements is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one Item-Increment.");
        var command = CountMinSketchCommands.InitCMC_IncrBy(key);
        foreach (var item in itemIncrements)
        {
            _ = command
                .WriteArguments(item.Key)
                .WriteArguments(item.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_IncrBy(SharpValue key, ReadOnlySpan<(SharpValue item, long increment)> itemIncrements)
    {
        if (itemIncrements.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one Item-Increment.");
        var command = CountMinSketchCommands.InitCMC_IncrBy(key);
        foreach (var item in itemIncrements)
        {
            _ = command
                .WriteArguments(item.item)
                .WriteArguments(item.increment.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_IncrBy(SharpValue key, ReadOnlySpan<KeyValuePair<SharpValue, long>> itemIncrements)
    {
        if (itemIncrements.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one Item-Increment.");
        var command = CountMinSketchCommands.InitCMC_IncrBy(key);
        foreach (var item in itemIncrements)
        {
            _ = command
                .WriteArguments(item.Key)
                .WriteArguments(item.Value.ToBytes());
        }
        return command;
    }

    private static CommandPacket InitCMC_IncrBy(SharpValue key)
    {
        return new CommandPacket(CountMinSketchCommands._cmsincrby, CommandMode.CountMinSketch | CommandMode.WithWrite)
            .WriteKeys(key);
    }

    internal static CommandPacket CMS_Info(SharpValue key)
    {
        return new CommandPacket(CountMinSketchCommands._cmsinfo, CommandMode.CountMinSketch | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }

    internal static CommandPacket CMS_InitByDim(SharpValue key, long width, long depth)
    {
        return new CommandPacket(CountMinSketchCommands._cmsinitbydim, CommandMode.CountMinSketch | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(width.ToBytes())
            .WriteArguments(depth.ToBytes());
    }

    internal static CommandPacket CMS_InitByProb(SharpValue key, double error, double probability)
    {
        return new CommandPacket(CountMinSketchCommands._cmsinitbyprob, CommandMode.CountMinSketch | CommandMode.WithWrite)
            .WriteKeys(key)
            .WriteArguments(error.ToBytes())
            .WriteArguments(probability.ToBytes());
    }

    internal static CommandPacket CMS_Merge(SharpValue destination, SharpValue source, long? weight)
    {
        var command = CountMinSketchCommands.InitCMS_Merge(destination)
            .WriteArguments("1")
            .WriteKeys(source);
        if (weight.HasValue)
        {
            _ = command
                .WriteArguments("WEIGHTS")
                .WriteArguments(weight.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_Merge(SharpValue destination, ICollection<(SharpValue source, long weight)> sourceWeights)
    {
        if (sourceWeights is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one source-weight.");
        var command = CountMinSketchCommands.InitCMS_Merge(destination)
            .WriteArguments(sourceWeights.Count.ToBytes());
        foreach (var (source, _) in sourceWeights)
        {
            _ = command.WriteKeys(source);
        }
        _ = command.WriteArguments("WEIGHTS");
        foreach (var (_, weight) in sourceWeights)
        {
            _ = command.WriteArguments(weight.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_Merge(SharpValue destination, ReadOnlySpan<(SharpValue source, long weight)> sourceWeights)
    {
        if (sourceWeights.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one source-weight.");
        var command = CountMinSketchCommands.InitCMS_Merge(destination)
            .WriteArguments(sourceWeights.Length.ToBytes());
        foreach (var (source, _) in sourceWeights)
        {
            _ = command.WriteKeys(source);
        }
        _ = command.WriteArguments("WEIGHTS");
        foreach (var (_, weight) in sourceWeights)
        {
            _ = command.WriteArguments(weight.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_Merge(SharpValue destination, ICollection<KeyValuePair<SharpValue, long>> sourceWeights)
    {
        if (sourceWeights is null or { Count: <= 0 }) throw new NotSupportedException("Please ensure that there is at least one source-weight.");
        var command = CountMinSketchCommands.InitCMS_Merge(destination)
            .WriteArguments(sourceWeights.Count.ToBytes());
        foreach (var item in sourceWeights)
        {
            _ = command.WriteKeys(item.Key);
        }
        _ = command.WriteArguments("WEIGHTS");
        foreach (var item in sourceWeights)
        {
            _ = command.WriteArguments(item.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_Merge(SharpValue destination, ReadOnlySpan<KeyValuePair<SharpValue, long>> sourceWeights)
    {
        if (sourceWeights.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one source-weight.");
        var command = CountMinSketchCommands.InitCMS_Merge(destination)
            .WriteArguments(sourceWeights.Length.ToBytes());
        foreach (var item in sourceWeights)
        {
            _ = command.WriteKeys(item.Key);
        }
        _ = command.WriteArguments("WEIGHTS");
        foreach (var item in sourceWeights)
        {
            _ = command.WriteArguments(item.Value.ToBytes());
        }
        return command;
    }

    internal static CommandPacket CMS_Merge(
        SharpValue destination,
        SharpValueSpan sourceSpan,
#if MODERN_NET
        long[]? weights
#else
        long[] weights
#endif
    )
    {
        if (sourceSpan.IsEmpty) throw new NotSupportedException("Please ensure that there is at least one source.");
        if (weights is { Length: > 0 })
        {
            if (weights.Length != sourceSpan.Length)
            {
                throw new NotSupportedException("wrong number of keys/weights");
            }
        }
        var command = CountMinSketchCommands.InitCMS_Merge(destination)
            .WriteArguments(sourceSpan.Length.ToBytes());
        foreach (var source in sourceSpan)
        {
            _ = command.WriteKeys(source);
        }
        if (weights is { Length: > 0 })
        {
            _ = command.WriteArguments("WEIGHTS");
            foreach (var item in weights)
            {
                _ = command.WriteArguments(item.ToBytes());
            }
        }
        return command;
    }

    private static CommandPacket InitCMS_Merge(SharpValue destination)
    {
        return new CommandPacket(CountMinSketchCommands._cmsmerge, CommandMode.CountMinSketch | CommandMode.WithWrite)
            .WriteKeys(destination);
    }

    internal static CommandPacket CMS_Query(SharpValue key, SharpValue item)
    {
        return CountMinSketchCommands.InitCMS_Query(key)
            .WriteArguments(item);
    }

    internal static CommandPacket CMS_Query(SharpValue key, SharpValueSpan items)
    {
        return CountMinSketchCommands.InitCMS_Query(key)
            .WriteArguments(items);
    }

    private static CommandPacket InitCMS_Query(SharpValue key)
    {
        return new CommandPacket(CountMinSketchCommands._cmsquery, CommandMode.CountMinSketch | CommandMode.WithRead | CommandMode.WithClientCaching)
            .WriteKeys(key);
    }
}
