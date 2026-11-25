using SharpRedis.Commands;
using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Text;
using System.Threading;

namespace SharpRedis.Entities.Command;

internal sealed class CommandBatch : ICommand
{
    public CancellationToken CancellationToken { get; set; }

    public DateTime TimeoutAt { get; set; } = DateTime.MaxValue;

    private const string _cacheKeyExMsg = "Batch pipeline command does not support cache key generation";

    #region fields
    private CommandPacket[] _commands;
    // Allocated only when this batch borrows packets owned by another command.
#if MODERN_NET
    private bool[]? _borrowedCommands;
#else
    private bool[] _borrowedCommands;
#endif
    private int _commandCount = 0;
    private CommandFlags _flags = CommandFlags.None;
    private CommandMode _mode = CommandMode.WithPipelining | CommandMode.DoNotReuseConnection;
    private int _database;
    private CommandLifetime _lifetime;
    #endregion

    #region Properties
    public CommandMode Mode => this._mode;

    public CommandFlags Flags => this._flags;

    public bool IsBatch => true;

    public int ResultCount => this._commandCount;

    public int DataBaseIndex => this._database;

    public ReadOnlySpan<CommandPacket> BatchCommands => new(this._commands, 0, this._commandCount);

    public ReadOnlySpan<SharpValue> Items => throw new NotSupportedException("Batch commands do not support direct item access.");
    #endregion

    internal CommandBatch()
    {
        this._commands = SharedPool.CommandPacketShard.Rent(4);
    }

    public CommandBatch AddCommand(CommandPacket command)
    {
        if (this._commandCount >= this._commands.Length)
        {
            var newCommands = SharedPool.CommandPacketShard.Rent(this._commands.Length + 4);
            Array.Copy(this._commands, 0, newCommands, 0, this._commands.Length);
            SharedPool.CommandPacketShard.Return(this._commands, true);
            this._commands = newCommands;
            if (this._borrowedCommands != null) Array.Resize(ref this._borrowedCommands, newCommands.Length);
        }
        this._commands[this._commandCount++] = command;
        return this;
    }

    internal CommandBatch BorrowCommand(CommandPacket command)
    {
        this.AddCommand(command);
        this._borrowedCommands ??= new bool[this._commands.Length];
        this._borrowedCommands[this._commandCount - 1] = true;
        return this;
    }

    public CommandBatch EnableCachingYes()
        => throw new NotSupportedException("Batch execution of commands does not support the EnableCachingYes method.");

    public CommandBatch AppendCommandWithCachingYes(CommandPacket command)
    {
        var cachingYesCommand = ConnectionCommands.ClientCaching(true);
        this.AddCommand(cachingYesCommand);
        this.BorrowCommand(command);
        return this;
    }

    public string GetCommandString(Encoding encoding)
        => "PIPELINING";

    public void WithFlags(CommandFlags flags)
    {
        _ = this | flags;
    }

    public void RemoveMode(CommandMode mode)
    {
        _ = this & mode;
    }

    public string CacheKey(Encoding encoding, string keyPrefix, string keySuffix)
        => throw new NotSupportedException(CommandBatch._cacheKeyExMsg);

    public void WriteToResp(Encoding encoding, ref byte[] buffer, ref int position, byte[] keyPrefix, byte[] keySuffix)
    {
        foreach (var command in this.BatchCommands)
        {
            command.WriteToResp(encoding, ref buffer, ref position, keyPrefix, keySuffix);
        }
    }

    #region Dispose
    public bool TryRetainForSend()
    {
        if (!this._lifetime.TryRetain()) return false;

        int retained = 0;
        while (retained < this._commandCount && this._commands[retained].TryRetainForSend())
        {
            retained++;
        }
        if (retained == this._commandCount) return true;

        for (int i = 0; i < retained; i++) this._commands[i].ReleaseAfterSend();
        if (this._lifetime.Release()) this.ReleaseResources();
        return false;
    }

    public void ReleaseAfterSend()
    {
        for (int i = 0; i < this._commandCount; i++) this._commands[i].ReleaseAfterSend();
        if (this._lifetime.Release()) this.ReleaseResources();
    }

    private void ReleaseResources()
    {
        for (int i = 0; i < this._commandCount; i++)
        {
            if (this._borrowedCommands == null || !this._borrowedCommands[i]) this._commands[i].Dispose();
        }
        SharedPool.CommandPacketShard.Return(this._commands, true);
        this._commands = null!;
        this._borrowedCommands = null!;
    }

    public void Dispose()
    {
        if (this._lifetime.DisposeOwner()) this.ReleaseResources();
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Operator extension
    public static CommandBatch operator |(CommandBatch command, CommandFlags flags)
    {
        if (flags == CommandFlags.None) return command;
        command._flags |= flags;
        return command;
    }

    public static CommandBatch operator |(CommandBatch command, CommandMode mode)
    {
        command._mode |= mode;
        return command;
    }

    public static CommandBatch operator &(CommandBatch command, CommandMode mode)
    {
        command._mode &= ~mode;
        return command;
    }

    public static CommandBatch operator |(CommandBatch command, int database)
    {
        if (command._database != database)
        {
            command._database = database;
        }
        return command;
    }
    #endregion

    bool ICommand.RetutnBytes(int index)
    {
        return (this._commands?[index]?.RetutnBytes(index)) ?? false;
    }
}
