#pragma warning disable IDE1006
using SharpRedis.Entities.Command;
using System.Buffers;

namespace SharpRedis.Extensions;

internal static class SharedPool
{
    internal static readonly ArrayPool<byte> BytesShard = ArrayPool<byte>.Shared;

    internal static readonly ArrayPool<SharpValue> CommandShard = ArrayPool<SharpValue>.Shared;

    internal static readonly ArrayPool<CommandPacket> CommandPacketShard = ArrayPool<CommandPacket>.Shared;
}
