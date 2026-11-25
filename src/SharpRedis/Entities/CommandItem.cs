//using SharpRedis.Extensions;
//using System;
//using System.Text;

//namespace SharpRedis.Entities;

//internal readonly struct CommandItem
//{
//    internal readonly SharpValue Data { get; }

//    internal readonly CommandItemCategory Category { get; }

//    internal CommandItem(SharpValue data, CommandItemCategory category)
//    {
//        this.Data = data;
//        this.Category = category;
//    }

//    internal void WriteToResp(ref byte[] buffer, ref int pos, Encoding encoding)
//    {
//        CommandItem.WriteByte(Config.BULK_STRINGS, ref buffer, ref pos);
//        var (dataBytes, dataLength) = this.Data.GetBytes(encoding);
//        CommandItem.WriteIntAscii(dataLength, ref buffer, ref pos);
//        CommandItem.WriteByte(Config.CR, ref buffer, ref pos);
//        CommandItem.WriteByte(Config.LF, ref buffer, ref pos);
//        CommandItem.WriteBytes(dataBytes, dataLength, ref buffer, ref pos);
//        CommandItem.WriteByte(Config.CR, ref buffer, ref pos);
//        CommandItem.WriteByte(Config.LF, ref buffer, ref pos);
//        SharedPool.BytesShard.Return(dataBytes);
//    }

//#if MODERN_NET
//    internal void KeyWriteToResp(ref byte[] buffer, ref int pos, byte[]? prefix, byte[]? suffix, Encoding encoding)
//#else
//    internal void KeyWriteToResp(ref byte[] buffer, ref int pos, byte[] prefix, byte[] suffix, Encoding encoding)
//#endif
//    {
//        CommandItem.WriteByte(Config.BULK_STRINGS, ref buffer, ref pos);
//        var (dataBytes, dataLength) = this.Data.GetBytes(encoding);
//        int keyLength = dataLength;
//        if (prefix is { Length: > 0 }) keyLength += prefix.Length;
//        if (suffix is { Length: > 0 }) keyLength += suffix.Length;
//        CommandItem.WriteIntAscii(keyLength, ref buffer, ref pos);
//        CommandItem.WriteByte(Config.CR, ref buffer, ref pos);
//        CommandItem.WriteByte(Config.LF, ref buffer, ref pos);
//        if (prefix is { Length: > 0 })
//        {
//            CommandItem.WriteBytes(prefix, prefix.Length, ref buffer, ref pos);
//        }
//        CommandItem.WriteBytes(dataBytes, dataLength, ref buffer, ref pos);
//        if (suffix is { Length: > 0 })
//        {
//            CommandItem.WriteBytes(suffix, suffix.Length, ref buffer, ref pos);
//        }
//        CommandItem.WriteByte(Config.CR, ref buffer, ref pos);
//        CommandItem.WriteByte(Config.LF, ref buffer, ref pos);
//        SharedPool.BytesShard.Return(dataBytes);
//    }

//    internal static void WriteBytes(byte[] src, int count, ref byte[] buffer, ref int pos)
//    {
//        CommandItem.EnsureCapacity(count, ref buffer, ref pos);
//        Buffer.BlockCopy(src, 0, buffer, pos, count);
//        pos += count;
//    }

//    private static void EnsureCapacity(int need, ref byte[] buffer, ref int pos)
//    {
//        var maxSize = pos + need;
//        if (maxSize <= buffer.Length) return;

//        int newSize = buffer.Length + Config.DEFAULT_BUFFER_SIZE;
//        while (maxSize > newSize) newSize += Config.DEFAULT_BUFFER_SIZE;

//        var newBuf = SharedPool.BytesShard.Rent(newSize);
//        Buffer.BlockCopy(buffer, 0, newBuf, 0, pos);
//        SharedPool.BytesShard.Return(buffer);

//        buffer = newBuf;
//    }

//    internal static void WriteByte(byte value, ref byte[] buffer, ref int pos)
//    {
//        CommandItem.EnsureCapacity(1, ref buffer, ref pos);
//        Buffer.SetByte(buffer, pos, value);
//        pos++;
//    }

//    internal static void WriteIntAscii(int value, ref byte[] buffer, ref int pos)
//    {
//        if (value <= 9)
//        {
//            Buffer.SetByte(buffer, pos, (byte)(48 + value));
//            pos++;
//            return;
//        }

//        int tmp = value;
//        int digits = 0;
//        if (tmp >= 10)
//        {
//            while (tmp > 0)
//            {
//                tmp /= 10;
//                digits++;
//            }
//        }

//        CommandItem.EnsureCapacity(digits, ref buffer, ref pos);

//        int writePos = pos + digits - 1;
//        while (value > 0)
//        {
//            int digit = value % 10;
//            Buffer.SetByte(buffer, writePos--, (byte)(48 + digit));
//            value /= 10;
//        }

//        pos += digits;
//    }

//    internal static CommandItem CreateKey(SharpValue key)
//    {
//        return new(key, CommandItemCategory.Key);
//    }

//    internal static CommandItem CreateArgument(SharpValue arg)
//    {
//        return new(arg, CommandItemCategory.Argument);
//    }

//    internal enum CommandItemCategory : byte
//    {
//        Command = 0,

//        Key,

//        Argument,
//    }
//}
