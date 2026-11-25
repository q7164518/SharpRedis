using SharpRedis.Commands;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SharpRedis.Entities.Command;

internal sealed class CommandPacket : ICommand
{
    public CancellationToken CancellationToken { get; set; }

    public DateTime TimeoutAt { get; set; } = DateTime.MaxValue;

    #region Fields
    private SharpValue[] _items;
    private int _itemCount = 0;
    private CommandMode _mode;
    private CommandFlags _flags = CommandFlags.None;
    private int _database;
    private CommandLifetime _lifetime;
    #endregion

    #region Properties
    internal int ItemCount => this._itemCount;

    public ReadOnlySpan<SharpValue> Items => new(this._items, 0, this._itemCount);

    public CommandMode Mode => this._mode;

    public CommandFlags Flags => this._flags;

    public bool IsBatch => false;

    public int ResultCount => 1;

    public int DataBaseIndex => this._database;

    public ReadOnlySpan<CommandPacket> BatchCommands => throw new NotSupportedException("Non-batch pipeline command");
    #endregion

    internal CommandPacket(SharpValue command, CommandMode mode)
    {
        this._items = SharedPool.CommandShard.Rent(4);
        this.AddCommandItem(command, CommandItemCategory.Command);
        this._mode = mode;
    }

    #region keys methods
    internal CommandPacket WriteKeys(SharpValue key)
    {
        this.AddCommandItem(key, CommandItemCategory.Key);
        return this;
    }

    internal CommandPacket WriteKeys(bool condition, SharpValue key)
    {
        return !condition ? this : this.WriteKeys(key);
    }

    internal CommandPacket WriteKeys(ReadOnlySpan<SharpValue> keys)
    {
        foreach (var key in keys)
        {
            _ = this.WriteKeys(key);
        }
        return this;
    }

    internal CommandPacket WriteKeys(bool condition, ReadOnlySpan<SharpValue> keys)
    {
        return !condition ? this : this.WriteKeys(keys);
    }

    internal CommandPacket WriteKeys(SharpValueSpan keys)
    {
        foreach (var key in keys)
        {
            _ = this.WriteKeys(key);
        }
        return this;
    }

    internal CommandPacket WriteKeys(bool condition, SharpValueSpan keys)
    {
        return !condition ? this : this.WriteKeys(keys);
    }
    #endregion

    #region arg methods
    internal CommandPacket WriteArguments(ReadOnlySpan<SharpValue> args)
    {
        foreach (var arg in args)
        {
            _ = this.WriteArguments(arg);
        }
        return this;
    }

    internal CommandPacket WriteArguments(bool condition, ReadOnlySpan<SharpValue> args)
    {
        return !condition ? this : this.WriteArguments(args);
    }

    internal CommandPacket WriteArguments(SharpValue arg)
    {
        this.AddCommandItem(arg, CommandItemCategory.Argument);
        return this;
    }

    internal CommandPacket WriteArguments(SharpValueSpan args)
    {
        foreach (var arg in args)
        {
            _ = this.WriteArguments(arg);
        }
        return this;
    }

    internal CommandPacket WriteArguments(bool condition, SharpValueSpan args)
    {
        return !condition ? this : this.WriteArguments(args);
    }

    internal CommandPacket WriteArguments(string arg)
    {
        this.AddCommandItem(arg, CommandItemCategory.Argument);
        return this;
    }

    internal CommandPacket WriteArguments(bool condition, SharpValue arg)
    {
        return !condition ? this : this.WriteArguments(arg);
    }

    internal CommandPacket WriteArguments(bool condition, string arg)
    {
        return !condition ? this : this.WriteArguments(arg);
    }

    internal CommandPacket WriteArguments(ICollection<SharpValue> args)
    {
        foreach (var arg in args)
        {
            _ = this.WriteArguments(arg);
        }
        return this;
    }

    internal CommandPacket WriteArguments(ICollection<string> args)
    {
        foreach (var arg in args)
        {
            _ = this.WriteArguments(arg);
        }
        return this;
    }

    internal CommandPacket WriteArguments(bool condition, ICollection<SharpValue> args)
    {
        return !condition ? this : this.WriteArguments(args);
    }
    #endregion

    #region Dispose
    public bool TryRetainForSend() => this._lifetime.TryRetain();

    public void ReleaseAfterSend()
    {
        if (this._lifetime.Release()) this.ReleaseResources();
    }

    private void ReleaseResources()
    {
        if (this._items != null) SharedPool.CommandShard.Return(this._items);
        this._items = null!;
    }

    public void Dispose()
    {
        if (this._lifetime.DisposeOwner()) this.ReleaseResources();
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Operator extension
    public static CommandPacket operator |(CommandPacket command, CommandFlags flags)
    {
        if (flags == CommandFlags.None) return command;
        command._flags |= flags;
        return command;
    }

    public static CommandPacket operator |(CommandPacket command, CommandMode mode)
    {
        command._mode |= mode;
        return command;
    }

    public static CommandPacket operator &(CommandPacket command, CommandMode mode)
    {
        command._mode &= ~mode;
        return command;
    }

    public static CommandPacket operator |(CommandPacket command, int database)
    {
        if (command._database != database)
        {
            command._database = database;
        }
        return command;
    }
    #endregion

    internal CommandPacket WriteCommand(SharpValue command)
    {
        this.AddCommandItem(command, CommandItemCategory.Command);
        return this;
    }

    private void AddCommandItem(SharpValue command, CommandItemCategory category)
    {
        if (this._itemCount >= this._items.Length)
        {
            var newCommands = SharedPool.CommandShard.Rent(this._items.Length + 4);
            Array.Copy(this._items, 0, newCommands, 0, this._items.Length);
            SharedPool.CommandShard.Return(this._items);
            this._items = newCommands;
        }
        this._items[this._itemCount++] = command.With(category);
    }

    public CommandBatch AddCommand(CommandPacket command)
    {
        return new CommandBatch()
            .AddCommand(this)
            .AddCommand(command);
    }

    public CommandBatch EnableCachingYes()
    {
        var cachingYesCommand = ConnectionCommands.ClientCaching(true);
        return new CommandBatch()
            .AddCommand(cachingYesCommand)
            .BorrowCommand(this);
    }

    public CommandBatch AppendCommandWithCachingYes(CommandPacket command)
    {
        var cachingYesCommand = ConnectionCommands.ClientCaching(true);
        return new CommandBatch()
            .BorrowCommand(this)
            .AddCommand(cachingYesCommand)
            .BorrowCommand(command);
    }

    public void WithFlags(CommandFlags flags)
    {
        _ = this | flags;
    }

    public void RemoveMode(CommandMode mode)
    {
        _ = this & mode;
    }

    public string GetCommandString(Encoding encoding)
    {
        string commandString = string.Empty;
        foreach (var item in this.Items)
        {
            if (item.Category is CommandItemCategory.Command)
            {
                commandString += item.ToString(encoding) + " ";
            }
        }
        return commandString.Trim();
    }

    public string CacheKey(Encoding encoding, string keyPrefix, string keySuffix)
    {
        StringBuilder sb = new();
        _ = sb.Append(this._database).Append(':');
        foreach (var item in this.Items)
        {
            // Length framing preserves argument boundaries, including spaces and delimiters.
            // Binary arguments must not pass through a potentially lossy text decoder.
            bool isKey = item.Category == CommandItemCategory.Key;
            if (item.MemoryByte is { } bytes)
            {
                var value = Convert.ToBase64String(bytes.ToArray());
                _ = sb.Append('B');
                AppendPart(sb, isKey ? keyPrefix : string.Empty);
                AppendPart(sb, value);
                AppendPart(sb, isKey ? keySuffix : string.Empty);
            }
            else
            {
                var value = item.ToString(encoding);
                _ = sb.Append('T').Append(value.Length + (isKey ? keyPrefix.Length + keySuffix.Length : 0)).Append(':');
                if (isKey) _ = sb.Append(keyPrefix);
                _ = sb.Append(value);
                if (isKey) _ = sb.Append(keySuffix);
            }
        }
        return sb.ToString();

        static void AppendPart(StringBuilder builder, string value)
            => builder.Append(value.Length).Append(':').Append(value);
    }

    public void WriteToResp(Encoding encoding, ref byte[] buffer, ref int pos, byte[] keyPrefix, byte[] keySuffix)
    {
        RESP.WriteArrayHeader(this._itemCount, ref buffer, ref pos);
        foreach (ref readonly var item in this.Items)
        {
            bool isKey = item.Category == CommandItemCategory.Key;
            RESP.SharpValueToResp(
                in item,
                ref buffer,
                ref pos,
                isKey ? keyPrefix : [],
                isKey ? keySuffix : [],
                encoding);
        }
    }

    public bool RetutnBytes(int index)
    {
        return (this._flags & CommandFlags.ReturnBytes) == CommandFlags.ReturnBytes;
    }
}
