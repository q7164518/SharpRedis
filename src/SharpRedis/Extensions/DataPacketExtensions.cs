#if NET8_0_OR_GREATER
#pragma warning disable IDE0301
#endif
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpRedis.Extensions
{
    internal static class DataPacketExtensions
    {
        private const byte _r = 13;
        private const byte _n = 10;
        private const string _unknownErrorMsg = "Unknown error, error message cannot be obtained";

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        internal static bool GetNextValue(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, [NotNullWhen(true)] out object? data)
#else
        internal static bool GetNextValue(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, out object data)
#endif
        {
            if (packetData is null || packetData.Length <= 0)
            {
                data = null;
                return false;
            }
            var type = packetData.ReadByte();
            switch (type)
            {
                case 33: // ! Bulk errors
                    {
                        if (DataPacketExtensions.ParseBulkErrors(packetData, encoding, dataType, out var ex))
                        {
                            data = ex;
                            return true;
                        }
                        goto case -1;
                    }
                case 35: // # boolean
                    {
                        if (DataPacketExtensions.ParseLine(packetData, out var boolean))
                        {
                            data = new BooleanValue(encoding.GetString(boolean));
                            return true;
                        }
                        goto case -1;
                    }
                case 36: //$ Bulk strings
                    {
                        if (DataPacketExtensions.ParseBulkStrings(packetData, encoding, dataType, out var stringData))
                        {
                            data = stringData;
                            return true;
                        }
                        goto case -1;
                    }
                case 37: // % Hash
                    {
                        if (DataPacketExtensions.ParseHash(packetData, encoding, dataType, out var hash))
                        {
                            data = hash;
                            return true;
                        }
                        goto case -1;
                    }
                case 43: //+ Simple strings
                    {
                        if (DataPacketExtensions.ParseLine(packetData, out var simple))
                        {
                            if (dataType is ResultDataType.Bytes) data = simple;
                            else data = encoding.GetString(simple);
                            return true;
                        }
                        goto case -1;
                    }
                case 40: //( Big numbers
                case 44: //, double
                case 58: //: int long
                    {
                        if (DataPacketExtensions.ParseLine(packetData, out var number))
                        {
                            data = new NumberValue(encoding.GetString(number), type);
                            return true;
                        }
                        goto case -1;
                    }
                case 45: //- Simple Errors
                    {
                        if (DataPacketExtensions.ParseSimpleErrors(packetData, encoding, out var ex))
                        {
                            data = ex;
                            return true;
                        }
                        goto case -1;
                    }
                case 42: //* Array
                case 62: // > Pushes
                case 126: // ~ Sets
                    {
                        if (DataPacketExtensions.ParseArray(packetData, encoding, dataType, out var array))
                        {
                            data = array;
                            return true;
                        }
                        goto case -1;
                    }
                case 95: // _ null
                    {
                        packetData.Position += 2; // \r\n
                        data = DBNull.Value;
                        return true;
                    }
                default:
                    {
                        data = new FormatException($"Unsupported data type: {(char)type}");
                        return true;
                    }
                case -1:
                    {
                        data = null;
                        return false;
                    }
            }
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static bool ParseBulkErrors(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, [NotNullWhen(true)] out RedisException? ex)
#else
        private static bool ParseBulkErrors(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, out RedisException ex)
#endif
        {
            if (DataPacketExtensions.ParseBulkStrings(packetData, encoding, dataType, out var errorMsgObject))
            {
                if (errorMsgObject is byte[] errorMsgBytes && errorMsgBytes.Length > 0)
                {
                    ex = new RedisException(encoding.GetString(errorMsgBytes));
                    return true;
                }
                if (errorMsgObject is string errorMsg && !string.IsNullOrEmpty(errorMsg))
                {
                    ex = new RedisException(errorMsg);
                    return true;
                }

                ex = new RedisException(DataPacketExtensions._unknownErrorMsg);
                return true;
            }
            ex = null;
            return false;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static bool ParseSimpleErrors(MemoryStream packetData, Encoding encoding, [NotNullWhen(true)] out RedisException? ex)
#else
        private static bool ParseSimpleErrors(MemoryStream packetData, Encoding encoding, out RedisException ex)
#endif
        {
            if (DataPacketExtensions.ParseLine(packetData, out var errorMsg))
            {
                if (errorMsg?.Length > 0)
                {
                    ex = new RedisException(encoding.GetString(errorMsg));
                }
                else
                {
                    ex = new RedisException(DataPacketExtensions._unknownErrorMsg);
                }
                return true;
            }
            ex = null;
            return false;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static bool ParseBulkStrings(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, [NotNullWhen(true)] out object? data)
#else
        private static bool ParseBulkStrings(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, out object data)
#endif
        {
            if (!DataPacketExtensions.GetCount(packetData, out var length))
            {
                data = null;
                return false;
            }
            if (length is 0)
            {
                if (dataType is ResultDataType.Bytes)
                {
                    data = Array.Empty<byte>();
                }
                else
                {
                    data = string.Empty;
                }
                packetData.Position += 2;  // \r\n
                return true;
            }
            if (length is -1)
            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                data = null!;
#else
                data = null;
#endif
                return true;
            }
            if (packetData.Length - packetData.Position < length + 2)
            {
                data = null;
                return false;
            }

            var buffer = new byte[length];
            packetData.Read(buffer, 0, length);
            packetData.Position += 2;  // \r\n
            if (dataType is ResultDataType.Bytes)
            {
                data = buffer;
            }
            else
            {
                data = encoding.GetString(buffer);
            }
            return true;
        }

        private static bool GetCount(MemoryStream packetData, out int count)
        {
            if (DataPacketExtensions.ParseLine(packetData, out var line) && Extend.ConvertByteArrayToInt(line, out count))
            {
                return true;
            }
            count = 0;
            return false;
        }

        #region ParseArray
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static bool ParseArray(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, [NotNullWhen(true)] out object?[]? array)
#else
        private static bool ParseArray(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, out object[] array)
#endif
        {
            if (!DataPacketExtensions.GetCount(packetData, out var count))
            {
                array = null;
                return false;
            }
            if (count <= 0)
            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                array = null!;
#else
                array = null;
#endif
                return true;
            }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            array = new object?[count];
#else
            array = new object[count];
#endif
            for (uint i = 0; i < count; i++)
            {
                if (DataPacketExtensions.GetNextValue(packetData, encoding, dataType, out var value))
                {
                    array[i] = value;
                    continue;
                }
                array = null;
                return false;
            }
            return true;
        }
        #endregion

        #region ParseHash
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static bool ParseHash(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, [NotNullWhen(true)] out Dictionary<string, object?>? dict)
#else
        private static bool ParseHash(MemoryStream packetData, Encoding encoding, in ResultDataType dataType, out Dictionary<string, object> dict)
#endif
        {
            if (!DataPacketExtensions.GetCount(packetData, out var count))
            {
                dict = null;
                return false;
            }
            if (count <= 0)
            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                dict = null!;
#else
                dict = null;
#endif
                return true;
            }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            dict = new Dictionary<string, object?>(count);
#else
            dict = new Dictionary<string, object>(count);
#endif
            for (uint i = 0; i < count; i++)
            {
                if (!DataPacketExtensions.GetNextValue(packetData, encoding, dataType, out var key))
                {
                    return false;
                }
                if (!DataPacketExtensions.GetNextValue(packetData, encoding, dataType, out var value))
                {
                    return false;
                }

                if (key is string sKey)
                {
                    dict[sKey] = value;
                }
                else if (key is byte[] keyBytes)
                {
                    dict[encoding.GetString(keyBytes)] = value;
                }
            }
            return true;
        }
        #endregion

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private static bool ParseLine(MemoryStream packetData, [NotNullWhen(true)] out byte[]? line)
#else
        private static bool ParseLine(MemoryStream packetData, out byte[] line)
#endif
        {
            var startPosition = packetData.Position;
            var remaining = (int)(packetData.Length - startPosition);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            Span<byte> buffer = stackalloc byte[remaining];
            int bytesRead = packetData.Read(buffer);
            int newlineIndex = buffer.IndexOf(DataPacketExtensions._n);
            if (newlineIndex > 0 && buffer[newlineIndex - 1] == DataPacketExtensions._r)
            {
                line = buffer[..(newlineIndex - 1)].ToArray();
                packetData.Position = startPosition + newlineIndex + 1;
                return true;
            }
#else
            var buffer = new byte[remaining];
            int bytesRead = packetData.Read(buffer, 0, remaining);
            int newlineIndex = System.Array.IndexOf(buffer, DataPacketExtensions._n);
            if (newlineIndex > 0 && buffer[newlineIndex - 1] == DataPacketExtensions._r)
            {
                line = new byte[newlineIndex - 1];
#if NET46_OR_GREATER || NETSTANDARD2_0_OR_GREATER
                Array.Copy(buffer, 0, line, 0, line.Length);
#else
                Buffer.BlockCopy(buffer, 0, line, 0, line.Length);
#endif
                packetData.Position = startPosition + newlineIndex + 1;
                return true;
            }
#endif
            packetData.Position = startPosition;
            line = null;
            return false;
        }
    }
}
