#if NET5_0_OR_GREATER
#pragma warning disable IDE0090
#endif
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRedis.Models
{
    internal sealed class CommandPacket : IDisposable
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private string[]? _cacheKeys;
#else
        private string[] _cacheKeys;
#endif
        private readonly CommandMode _mode;
        private List<CommandItem> _commands;
        private bool _disposedValue;
        private ResultDataType _resultDataType = ResultDataType.Bytes;

        internal CommandMode Mode => this._mode;

        internal ResultDataType ResultDataType => this._resultDataType;

        internal CommandPacket(string command, CommandMode mode)
        {
            this._commands = new List<CommandItem>(8) { command };
            this._mode = mode;
        }

        #region Write key methods
        internal CommandPacket WriteKey(string key)
        {
            return this.WriteKey(true, key);
        }

        internal CommandPacket WriteKey(bool condition, string key)
        {
            if (!condition) return this;
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(key);
#else
            if (key is null) throw new ArgumentNullException(nameof(key));
#endif
            this._commands.Add(new CommandItem(key, true));
            return this;
        }

        internal CommandPacket WriteKeys(params string[] keys)
        {
            return this.WriteKeys(true, keys);
        }

        internal CommandPacket WriteKeys(bool condition, params string[] keys)
        {
            if (!condition || keys is null || keys.Length <= 0) return this;
            var keyArray = new CommandItem[keys.Length];
            for (uint i = 0; i < keys.Length; i++)
            {
                if (keys[i] is null) throw new ArgumentNullException(nameof(keys));
                keyArray[i] = new CommandItem(keys[i], true);
            }
            this._commands.AddRange(keyArray);
            return this;
        }

        internal CommandPacket WriteArgKey(string argName, string key)
        {
            return this.WriteArgKey(true, argName, key);
        }

        internal CommandPacket WriteArgKey(bool condition, string argName, string key)
        {
            if (!condition) return this;
            return this.WriteArg(true, argName)
                .WriteKey(key);
        }
        #endregion

        #region Write arg methods
        internal CommandPacket WriteArg(string argName, string argValue)
        {
            return this.WriteArg(true, argName, argValue);
        }

        internal CommandPacket WriteArg(bool condition, string argName, string argValue)
        {
            if (!condition) return this;
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(argValue);
#else
            if (argValue is null) throw new ArgumentNullException(nameof(argValue));
#endif
            this._commands.Add(argName);
            this._commands.Add(argValue);
            return this;
        }

        internal CommandPacket WriteArg<TArg>(string argName, TArg argValue) where TArg : struct
        {
            return this.WriteArg(true, argName, argValue);
        }

        internal CommandPacket WriteArg<TArg>(bool condition, string argName, TArg argValue) where TArg : struct
        {
            if (!condition) return this;
            var stringValue = argValue.ToString() ?? throw new ArgumentNullException(nameof(argValue));
            this._commands.Add(argName);
            this._commands.Add(stringValue);
            return this;
        }

        internal CommandPacket WriteArg<TArg>(bool condition, string argName, TArg argValue1, TArg argValue2) where TArg : struct
        {
            if (!condition) return this;
            var stringValue1 = argValue1.ToString() ?? throw new ArgumentNullException(nameof(argValue1));
            var stringValue2 = argValue2.ToString() ?? throw new ArgumentNullException(nameof(argValue2));
            this._commands.Add(argName);
            this._commands.Add(stringValue1);
            this._commands.Add(stringValue2);
            return this;
        }

        internal CommandPacket WriteArg(string argName)
        {
            return this.WriteArg(true, argName);
        }

        internal CommandPacket WriteArg(bool condition, string argName)
        {
            if (!condition) return this;
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(argName);
#else
            if (argName is null) throw new ArgumentNullException(nameof(argName));
#endif
            this._commands.Add(argName);
            return this;
        }

        internal CommandPacket WriteArg<TArg>(TArg argValue) where TArg : struct
        {
            return this.WriteArg(true, argValue);
        }

        internal CommandPacket WriteArg<TArg>(bool condition, TArg argValue) where TArg : struct
        {
            if (!condition) return this;
            var stringValue = argValue.ToString() ?? throw new ArgumentNullException(nameof(argValue));
            this._commands.Add(stringValue);
            return this;
        }

        internal CommandPacket WriteArgs(params string[] args)
        {
            return this.WriteArgs(true, args);
        }

        internal CommandPacket WriteArgs(bool condition, params string[] args)
        {
            if(!condition) return this;
            if (args is null || args.Length == 0) return this;
            for (uint i = 0; i < args.Length; i++)
            {
                if (args[i] == null) throw new ArgumentNullException(nameof(args));
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                this._commands.Add(new CommandItem(args[i]!, false));
#else
                this._commands.Add(new CommandItem(args[i], false));
#endif
            }
            return this;
        }

        internal CommandPacket WriteArgs<TArg>(string argName, params TArg[] args) where TArg : struct
        {
            return this.WriteArgs(true, argName, args);
        }

        internal CommandPacket WriteArgs<TArg>(bool condition, string argName, params TArg[] args) where TArg : struct
        {
            if (!condition || args is null || args.Length <= 0) return this;
            this._commands.Add(new CommandItem(argName, false));
            for (int i = 0; i < args.Length; i++)
            {
                var stringValue = args[i].ToString() ?? throw new ArgumentNullException(nameof(args));
                this._commands.Add(stringValue);
            }
            return this;
        }
        #endregion

        #region Write value methods
        internal CommandPacket WriteValue<T>(T value) where T : class
        {
            return this.WriteValue(true, value);
        }

        internal CommandPacket WriteValue<T>(bool condition, T value) where T : class
        {
            if (!condition) return this;
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(value);
#else
            if (value is null) throw new ArgumentNullException(nameof(value));
#endif
            this._commands.Add(new CommandItem(value, false));
            return this;
        }

        internal CommandPacket WriteValue<T>(bool condition, string argName, T value) where T : class
        {
            if (!condition) return this;
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(value);
#else
            if (value is null) throw new ArgumentNullException(nameof(value));
#endif
            this.WriteArg(argName);
            this._commands.Add(new CommandItem(value, false));
            return this;
        }

        internal CommandPacket WriteValues<T>(params T[] values) where T : class
        {
            return this.WriteValues(true, values);
        }

        internal CommandPacket WriteValues<T>(bool condition, params T[] values) where T : class
        {
            if (!condition) return this;
            if (values is null || values.Length == 0) return this;
            for (uint i = 0; i < values.Length; i++)
            {
                if (values[i] == null) throw new ArgumentNullException(nameof(values));
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                this._commands.Add(new CommandItem(values[i]!, false));
#else
                this._commands.Add(new CommandItem(values[i], false));
#endif
            }
            return this;
        }
        #endregion

        internal CommandPacket SetResultDataType(ResultDataType dataType)
        {
            this._resultDataType = dataType;
            return this;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        internal byte[] ToResp(Encoding encoding, byte[]? prefix)
#else
        internal byte[] ToResp(Encoding encoding, byte[] prefix)
#endif
        {
            List<byte> commandByte = new List<byte>(16) { 42 };
            commandByte.AddRange(encoding.GetBytes(this._commands.Count.ToString()));
            commandByte.Add(13); //\r
            commandByte.Add(10); //\n

            for (int i = 0; i < this._commands.Count; i++)
            {
                commandByte.Add(36); //$
                var argBytes = this._commands[i].ToByteArray(encoding, prefix);
                commandByte.AddRange(encoding.GetBytes(argBytes.Length.ToString()));
                commandByte.Add(13); //\r
                commandByte.Add(10); //\n
                commandByte.AddRange(argBytes);
                commandByte.Add(13); //\r
                commandByte.Add(10); //\n
            }
            var result = commandByte.ToArray();
            commandByte.Clear();
            commandByte.Capacity = 0;
            return result;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        internal string[] GetKeys(string? keyPrefix)
#else
        internal string[] GetKeys(string keyPrefix)
#endif
        {
            if (this._cacheKeys != null) return this._cacheKeys;

            var keys = new List<string>();
            for (int i = 0; i < this._commands.Count; i++)
            {
                var item = this._commands[i];
                if (item.IsKey && item.Item is string key)
                {
                    if (!string.IsNullOrEmpty(keyPrefix)) keys.Add($"{keyPrefix}{key}");
                    else keys.Add(key);
                }
            }
            if (keys.Count > 0)
            {
                var result = keys.ToArray();
                keys.Clear();
                keys.Capacity = 0;
                this._cacheKeys = result;
                return this._cacheKeys;
            }
#if NET8_0_OR_GREATER
            this._cacheKeys = [];
            return this._cacheKeys;
#else
            this._cacheKeys = Array.Empty<string>();
            return this._cacheKeys;
#endif
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        internal ClientSideCacheKey ToClientSideCacheKey(string? keyPrefix)
#else
        internal ClientSideCacheKey ToClientSideCacheKey(string keyPrefix)
#endif
        {
            long hashKey = 17;
            for (int i = 0; i < this._commands.Count; i++)
            {
                var item = this._commands[i].Item;
                if (item is string str)
                {
                    hashKey = hashKey * 31 + Extend.GetStringHashCode(str);
                }
                else if (item is byte[] byteArray)
                {
                    hashKey = hashKey * 31 + Extend.GetByteArrayHashCode(byteArray);
                }
                else
                {
                    continue;
                }
            }

            return new ClientSideCacheKey(this.GetKeys(keyPrefix), hashKey);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < this._commands.Count; i++)
            {
                sb.Append(this._commands[i])
                    .Append(' ');
            }
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < this._commands.Count; i++)
            {
                hashCode ^= this._commands[i].GetHashCode();
            }
            hashCode ^= this._mode.GetHashCode();
            return hashCode;
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                    if (this._commands != null)
                    {
                        this._commands.Clear();
                        this._commands.Capacity = 0;
                    }
                }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
                this._commands = null;
                this._cacheKeys = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~CommandPacket()
        {
            this.Dispose(disposing: true);
        }

        private readonly struct CommandItem
        {
            private readonly object _item;
            private readonly bool _isKey;

            internal bool IsKey => this._isKey;

            internal object Item => this._item;

            internal CommandItem(object item, bool isKey)
            {
                this._item = item;
                this._isKey = isKey;
            }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            internal byte[] ToByteArray(Encoding encoding, byte[]? prefix)
#else
            internal byte[] ToByteArray(Encoding encoding, byte[] prefix)
#endif
            {
                byte[] itemByteArray;
                if (this._item is string str)
                {
                    itemByteArray = encoding.GetBytes(str);
                }
                else if (this._item is byte[] bytes)
                {
                    itemByteArray = bytes;
                }
                else
                {
                    throw new NotSupportedException($"The {this._item.GetType().FullName} type is not supported");
                }

                if (!this._isKey || prefix is null) return itemByteArray;
#if NET8_0_OR_GREATER
                return [.. prefix, .. itemByteArray];
#else
                var result = new byte[prefix.Length + itemByteArray.Length];
                Buffer.BlockCopy(prefix, 0, result, 0, prefix.Length);
                Buffer.BlockCopy(itemByteArray, 0, result, prefix.Length, itemByteArray.Length);
                return result;
#endif
            }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            public override string? ToString()
#else
            public override string ToString()
#endif
            {
                return this._item.ToString();
            }

            public static implicit operator CommandItem(string str)
            {
                return new CommandItem(str, false);
            }

            public static implicit operator CommandItem(byte[] bytes)
            {
                return new CommandItem(bytes, false);
            }
        }
    }
}
