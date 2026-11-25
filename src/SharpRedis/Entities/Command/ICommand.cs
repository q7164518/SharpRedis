using SharpRedis.Enums;
using System;
using System.Text;
using System.Threading;

namespace SharpRedis.Entities.Command;

internal interface ICommand : IDisposable
{
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// UTC deadline; DateTime.MaxValue means no timeout. Set before enqueueing.
    /// </summary>
    DateTime TimeoutAt { get; set; }

    bool TryRetainForSend();

    void ReleaseAfterSend();

    ReadOnlySpan<CommandPacket> BatchCommands { get; }

    ReadOnlySpan<SharpValue> Items { get; }

    CommandMode Mode { get; }

    bool IsBatch { get; }

    int ResultCount { get; }

    CommandFlags Flags { get; }

    int DataBaseIndex { get; }

    bool RetutnBytes(int index);

    CommandBatch AddCommand(CommandPacket command);

    CommandBatch EnableCachingYes();

    CommandBatch AppendCommandWithCachingYes(CommandPacket command);

    void WithFlags(CommandFlags flags);

    void RemoveMode(CommandMode mode);

    string GetCommandString(Encoding encoding);

    string CacheKey(Encoding encoding, string keyPrefix, string keySuffix);

    void WriteToResp(Encoding encoding, ref byte[] buffer, ref int position, byte[] keyPrefix, byte[] keySuffix);
}
