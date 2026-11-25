using SharpRedis.Entities.Command;
using SharpRedis.Extensions;
using System;

namespace SharpRedis.Commands;

internal static class CommandExtensions
{
    internal static CommandPacket WriteExpiry(this CommandPacket command, SharpExpiry? expiry, ExpirationFlags expiryFlags)
    {
        if (expiryFlags is ExpirationFlags.KEEPTTL)
        {
            return command.WriteArguments(nameof(ExpirationFlags.KEEPTTL));
        }
        else if (expiryFlags is ExpirationFlags.PERSIST)
        {
            return command.WriteArguments(nameof(ExpirationFlags.PERSIST));
        }
        else
        {
            if (expiry.HasValue)
            {
                return expiryFlags switch
                {
                    ExpirationFlags.EX => command
                        .WriteArguments(nameof(ExpirationFlags.EX))
                        .WriteArguments(expiry.Value.GetSeconds.ToBytes()),
                    ExpirationFlags.PX => command
                        .WriteArguments(nameof(ExpirationFlags.PX))
                        .WriteArguments(expiry.Value.GetMilliseconds.ToBytes()),
                    ExpirationFlags.EXAT => command
                        .WriteArguments(nameof(ExpirationFlags.EXAT))
                        .WriteArguments(expiry.Value.GetUnixTimeSeconds.ToBytes()),
                    ExpirationFlags.PXAT => command
                        .WriteArguments(nameof(ExpirationFlags.PXAT))
                        .WriteArguments(expiry.Value.GetUnixTimeMilliseconds.ToBytes()),
                    _ => command,
                };
            }
        }
        return command;
    }

    internal static CommandPacket CallCommand(SharpValueSpan command, SharpValueSpan keys)
    {
        int length = command.Length;
        if (length == 0) throw new ArgumentException("Cannot execute an empty command", nameof(command));
        var commandPacket = new CommandPacket(command[0], Enums.CommandMode.Call);
        try
        {
            for (int i = 1; i < length; i++)
            {
                var argument = command[i];
                int index = GetKeyIndex(in argument);
                if (index < 0) commandPacket.WriteArguments(argument);
                else if (index < keys.Length) commandPacket.WriteKeys(keys[index]);
                else throw new ArgumentOutOfRangeException(nameof(keys), "KEYS index is outside the supplied key collection.");
            }
            return commandPacket;
        }
        catch
        {
            commandPacket.Dispose();
            throw;
        }
    }

    private static int GetKeyIndex(in SharpValue argument)
    {
        var chars = argument.RawString is { } text ? text.AsSpan() : argument.MemoryChar.GetValueOrDefault().Span;
        var bytes = argument.MemoryByte.GetValueOrDefault().Span;
        bool binary = !bytes.IsEmpty;
        int length = binary ? bytes.Length : chars.Length;
        if (length < 7) return -1;
        if (binary)
        {
            if (bytes[length - 1] != ']' || !bytes.StartsWith("KEYS["u8)) return -1;
        }
        else if (chars[length - 1] != ']' || !chars.StartsWith("KEYS[".AsSpan(), StringComparison.Ordinal)) return -1;

        int index = 0;
        for (int i = 5; i < length - 1; i++)
        {
            int digit = (binary ? (int)bytes[i] : chars[i]) - '0';
            if ((uint)digit > 9) return -1;
            // Saturate so an oversized decimal index cannot wrap to a valid key.
            index = index > (int.MaxValue - digit) / 10 ? int.MaxValue : index * 10 + digit;
        }
        return index;
    }
}
