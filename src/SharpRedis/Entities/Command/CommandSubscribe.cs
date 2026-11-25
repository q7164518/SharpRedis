using SharpRedis.Enums;
using System;
using System.Text;
using System.Threading;

namespace SharpRedis.Entities.Command;

internal sealed class CommandSubscribe : ICommand
{
    public CancellationToken CancellationToken { get; set; }

    public DateTime TimeoutAt { get; set; } = DateTime.MaxValue;

    private CommandPacket _commandPacket;
    private CommandLifetime _lifetime;
#if MODERN_NET
    private OnReceive? _onReceive;
    private POnReceive? _ponReceive;

    internal OnReceive? OnReceive => this._onReceive;

    internal POnReceive? POnReceive => this._ponReceive;
#else
    private OnReceive _onReceive;
    private POnReceive _ponReceive;

    internal OnReceive OnReceive => this._onReceive;

    internal POnReceive POnReceive => this._ponReceive;
#endif

    public ReadOnlySpan<CommandPacket> BatchCommands => throw new NotSupportedException("Non-batch pipeline command");

    public CommandMode Mode => this._commandPacket.Mode;

    public bool IsBatch => false;

    public int ResultCount => 1;

    public CommandFlags Flags => this._commandPacket.Flags;

    public int DataBaseIndex => this._commandPacket.DataBaseIndex;

    public ReadOnlySpan<SharpValue> Items => this._commandPacket.Items;

    internal CommandSubscribe(CommandPacket command, OnReceive onReceive)
    {
        this._commandPacket = command;
        this._onReceive = onReceive;
    }

    internal CommandSubscribe(CommandPacket command, POnReceive ponReceive)
    {
        this._commandPacket = command;
        this._ponReceive = ponReceive;
    }

    public string GetCommandString(Encoding encoding)
    {
        return this._commandPacket.GetCommandString(encoding);
    }

    public void WithFlags(CommandFlags flags)
    {
        this._commandPacket.WithFlags(flags);
    }

    public void RemoveMode(CommandMode mode)
    {
        this._commandPacket.RemoveMode(mode);
    }

    public void WriteToResp(Encoding encoding, ref byte[] buffer, ref int position, byte[] keyPrefix, byte[] keySuffix)
    {
        this._commandPacket.WriteToResp(encoding, ref buffer, ref position, keyPrefix, keySuffix);
    }

    public CommandBatch AddCommand(CommandPacket command)
    {
        throw new NotImplementedException();
    }

    public CommandBatch AppendCommandWithCachingYes(CommandPacket command)
    {
        throw new NotImplementedException();
    }

    public string CacheKey(Encoding encoding, string keyPrefix, string keySuffix)
    {
        throw new NotImplementedException();
    }

    public CommandBatch EnableCachingYes()
    {
        throw new NotImplementedException();
    }

    public bool TryRetainForSend() => this._lifetime.TryRetain();

    public void ReleaseAfterSend()
    {
        if (this._lifetime.Release()) this.ReleaseResources();
    }

    private void ReleaseResources()
    {
        this._commandPacket.Dispose();
        this._commandPacket = null!;
        this._onReceive = null;
        this._ponReceive = null;
    }

    public void Dispose()
    {
        if (this._lifetime.DisposeOwner()) this.ReleaseResources();
        GC.SuppressFinalize(this);
    }

    public static CommandSubscribe operator |(CommandSubscribe command, int database)
    {
        if (command._commandPacket.DataBaseIndex != database)
        {
            _ = command._commandPacket | database;
        }
        return command;
    }

    bool ICommand.RetutnBytes(int index)
    {
        return this._commandPacket.RetutnBytes(index);
    }
}
