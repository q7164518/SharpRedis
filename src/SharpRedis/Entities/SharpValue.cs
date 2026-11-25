using SharpRedis.Enums;
using System;
using System.Text;

namespace SharpRedis;

/// <summary>
/// Sharp-Redis Command struct
/// <para>Support implicit conversion for types such as string, span, and byte[]</para>
/// <para>SharpRedis命令结构体. 支持string, span, byte[]等隐式转换</para>
/// </summary>
/// <remarks>
/// To support asynchronous methods, the Span&lt;T&gt; type is unable to achieve zero allocation. Therefore, we recommend using the <b>Memory&lt;T&gt;</b> type.
/// <para>为了支持异步方法，Span&lt;T&gt;类型无法实现零分配。所以, 我们建议使用<b>Memory&lt;T&gt;</b>类型。</para>
/// </remarks>
public readonly struct SharpValue : IEquatable<SharpValue>
{
    private const uint _fnvOffset = 2166136261;
    private const uint _fnvPrime = 16777619;

    public static readonly SharpValue Empty = new();

    private readonly CommandItemCategory _category = CommandItemCategory.None;
#if MODERN_NET
    private readonly string? _rawString = null;
    public readonly string? RawString => this._rawString;
#else
    private readonly string _rawString = null;
    public readonly string RawString => this._rawString;
#endif
    private readonly ReadOnlyMemory<char>? _memoryChar;
    private readonly ReadOnlyMemory<byte>? _memoryByte;

    public readonly ReadOnlyMemory<char>? MemoryChar => this._memoryChar;

    public readonly ReadOnlyMemory<byte>? MemoryByte => this._memoryByte;

    internal readonly CommandItemCategory Category => this._category;

    public readonly bool HasValue
    {
        get
        {
            if (this._rawString != null
                || this._memoryChar.HasValue
                || this._memoryByte.HasValue)
            {
                return true;
            }
            return false;
        }
    }

    public readonly bool IsNullOrEmpty
    {
        get
        {
            if (!this.HasValue) return true;
            if (!string.IsNullOrEmpty(this._rawString)) return false;
            if (this._memoryChar.HasValue && this._memoryChar.Value.Length > 0) return false;
            if (this._memoryByte.HasValue && this._memoryByte.Value.Length > 0) return false;
            return true;
        }
    }

    #region Constructors
    public SharpValue()
    {
    }

    /// <summary>
    /// Create a SharpValue based on the string
    /// <para>根据string创建SharpValue</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(string value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null) throw new ArgumentNullException(nameof(value));
#endif
        this._rawString = value;
    }

    /// <summary>
    /// Create a SharpValue based on the byte[]
    /// <para>根据byte[]创建SharpValue</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(byte[] value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value);
#else
        if (value is null) throw new ArgumentNullException(nameof(value));
#endif
        this._memoryByte = value.AsMemory();
    }

    /// <summary>
    /// Create a SharpValue based on the ReadOnlyMemory&lt;char&gt;
    /// <para>根据ReadOnlyMemory&lt;char&gt;创建SharpValue</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(ReadOnlyMemory<char> value)
    {
        this._memoryChar = value;
    }

    /// <summary>
    /// Create a SharpValue based on the ReadOnlyMemory&lt;byte&gt;
    /// <para>根据ReadOnlyMemory&lt;byte&gt;创建SharpValue</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(ReadOnlyMemory<byte> value)
    {
        this._memoryByte = value;
    }

    /// <summary>
    /// Create a SharpValue based on the ReadOnlyMemory&lt;char&gt;. A new char[] allocation will occur
    /// <para>根据ReadOnlySpan&lt;char&gt;创建SharpValue。会产生新的char[]分配.</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(ReadOnlySpan<char> value)
    {
        this._memoryChar = value.ToArray().AsMemory();
    }

    /// <summary>
    /// Create a SharpValue based on the ReadOnlySpan&lt;byte&gt;. A new byte[] allocation will occur
    /// <para>根据ReadOnlySpan&lt;byte&gt;创建SharpValue。会产生新的byte[]分配.</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(ReadOnlySpan<byte> value)
    {
        this._memoryByte = value.ToArray().AsMemory();
    }

    /// <summary>
    /// Create a SharpValue based on the ArraySegment&lt;char&gt;
    /// <para>根据ArraySegment&lt;char&gt;创建SharpValue。</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(ArraySegment<char> value)
    {
        this._memoryChar = new ReadOnlyMemory<char>(value.Array, value.Offset, value.Count);
    }

    /// <summary>
    /// Create a SharpValue based on the ArraySegment&lt;byte&gt;
    /// <para>根据ArraySegment&lt;byte&gt;创建SharpValue。</para>
    /// </summary>
    /// <param name="value"></param>
    public SharpValue(ArraySegment<byte> value)
    {
        this._memoryByte = new ReadOnlyMemory<byte>(value.Array, value.Offset, value.Count);
    }

    internal SharpValue(
#if MODERN_NET
        string? rawString,
#else
        string rawString,
#endif
        ReadOnlyMemory<char>? memoryChar,
        ReadOnlyMemory<byte>? memoryByte,
        CommandItemCategory category)
    {
        this._rawString = rawString;
        this._memoryChar = memoryChar;
        this._memoryByte = memoryByte;
        this._category = category;
    }

    internal SharpValue(string arg, CommandItemCategory category)
    {
        this._rawString = arg;
        this._category = category;
    }
    #endregion

    internal SharpValue With(CommandItemCategory category)
    {
        return new(this._rawString, this._memoryChar, this._memoryByte, category);
    }

    #region Implicit convert methods
    public static implicit operator SharpValue(string value)
    {
        return new SharpValue(value);
    }

#if MODERN_NET
    public static implicit operator SharpValue(byte[]? value)
#else
    public static implicit operator SharpValue(byte[] value)
#endif
    {
        if (value is null) return SharpValue.Empty;
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(ReadOnlyMemory<char> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(Memory<char> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(ReadOnlyMemory<byte> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(Memory<byte> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(ReadOnlySpan<char> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(Span<char> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(ReadOnlySpan<byte> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(Span<byte> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(ArraySegment<char> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(ArraySegment<byte> value)
    {
        return new SharpValue(value);
    }

    public static implicit operator SharpValue(RedisResult value)
    {
        if (value.RawValue is string str)
        {
            return new SharpValue(str);
        }
        else if (value.RawValue is byte[] bytes)
        {
            return new SharpValue(bytes);
        }
        throw new FormatException("Unrecognized type conversion to SharpValue");
    }
    #endregion

    public bool Equals(SharpValue other)
    {
        if (this._category != other._category) return false;
        bool hasValue = this.HasValue;
        if (hasValue != other.HasValue) return false;
        if (!hasValue) return true;

        if (this._memoryByte.HasValue && other._memoryByte.HasValue)
        {
            return this._memoryByte.Value.Span.SequenceEqual(
                other._memoryByte.Value.Span);
        }

        if (!this._memoryByte.HasValue && !other._memoryByte.HasValue)
        {
            return this.GetChars().SequenceEqual(other.GetChars());
        }

        if (this._memoryByte.HasValue)
        {
            return EqualsUtf8(
                this._memoryByte.Value.Span,
                other.GetChars());
        }

        return EqualsUtf8(
            other._memoryByte!.Value.Span,
            this.GetChars());
    }

    private ReadOnlySpan<char> GetChars()
    {
        if (this._rawString is not null)
            return this._rawString.AsSpan();

        return this._memoryChar.GetValueOrDefault().Span;
    }

    private static bool EqualsUtf8(
        ReadOnlySpan<byte> bytes,
        ReadOnlySpan<char> chars)
    {
#if NET5_0_OR_GREATER
        int charOffset = 0;
        int byteOffset = 0;

        Span<byte> utf8 = stackalloc byte[4];

        while (charOffset < chars.Length)
        {
            var status = Rune.DecodeFromUtf16(
                chars[charOffset..],
                out Rune rune,
                out int consumed);

            if (status != System.Buffers.OperationStatus.Done)
            {
                rune = Rune.ReplacementChar;
                consumed = 1;
            }
            charOffset += consumed;
            int written = rune.EncodeToUtf8(utf8);

            if (byteOffset > bytes.Length - written)
                return false;

            if (!utf8[..written].SequenceEqual(bytes.Slice(byteOffset, written)))
                return false;

            byteOffset += written;
        }

        return byteOffset == bytes.Length;
#else
        int byteCount = GetUtf8ByteCount(chars);
        if (byteCount != bytes.Length) return false;
        if (byteCount == 0) return true;

        byte[] buffer = Extensions.SharedPool.BytesShard.Rent(byteCount);

        try
        {
            unsafe
            {
                fixed (char* pChars = chars)
                fixed (byte* pBuffer = buffer)
                {
                    int written = Encoding.UTF8.GetBytes(
                        pChars,
                        chars.Length,
                        pBuffer,
                        buffer.Length);

                    return bytes.SequenceEqual(new ReadOnlySpan<byte>(buffer, 0, written));
                }
            }
        }
        finally
        {
            Extensions.SharedPool.BytesShard.Return(buffer, true);
        }
#endif
    }

    public override int GetHashCode()
    {
        unchecked
        {
            uint hash = _fnvOffset;
            hash = (hash ^ (uint)this._category) * _fnvPrime;
            if (!this.HasValue) return (int)((hash ^ 0u) * _fnvPrime);

            hash = (hash ^ 1u) * _fnvPrime;
            if (this._memoryByte.HasValue)
            {
                ReadOnlySpan<byte> bytes = this._memoryByte.Value.Span;
                foreach (byte value in bytes)
                    hash = (hash ^ value) * _fnvPrime;
                return (int)hash;
            }
            return GetUtf8HashCode(this.GetChars(), hash);
        }
    }

    private static unsafe int GetUtf8ByteCount(ReadOnlySpan<char> chars)
    {
#if !NETSTANDARD2_0
        return Encoding.UTF8.GetByteCount(chars);
#else
        fixed (char* pChars = chars)
        {
            return Encoding.UTF8.GetByteCount(pChars, chars.Length);
        }
#endif
    }

    private static int GetUtf8HashCode(ReadOnlySpan<char> chars, uint hash)
    {
        unchecked
        {
#if NET6_0_OR_GREATER
            Span<byte> utf8 = stackalloc byte[4];
            int offset = 0;
            while (offset < chars.Length)
            {
                var status = Rune.DecodeFromUtf16(
                    chars[offset..],
                    out Rune rune,
                    out int consumed);
                if (status != System.Buffers.OperationStatus.Done)
                {
                    rune = Rune.ReplacementChar;
                    consumed = 1;
                }
                offset += consumed;
                int written = rune.EncodeToUtf8(utf8);
                for (int i = 0; i < written; i++)
                    hash = (hash ^ utf8[i]) * _fnvPrime;
            }
#else
            int byteCount = GetUtf8ByteCount(chars);

            if (byteCount == 0)
                return (int)hash;

            byte[] buffer = Extensions.SharedPool.BytesShard.Rent(byteCount);

            try
            {
                unsafe
                {
                    fixed (char* pChars = chars)
                    fixed (byte* pBuffer = buffer)
                    {
                        int written = Encoding.UTF8.GetBytes(
                            pChars,
                            chars.Length,
                            pBuffer,
                            buffer.Length);

                        for (int i = 0; i < written; i++)
                            hash = (hash ^ buffer[i]) * _fnvPrime;
                    }
                }
            }
            finally
            {
                Extensions.SharedPool.BytesShard.Return(buffer, true);
            }
#endif
            return (int)hash;
        }
    }

#if MODERN_NET
    public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
#else
    public override bool Equals(object obj)
#endif
    {
        return obj is SharpValue other && this.Equals(other);
    }

    public string ToString(Encoding encoding)
    {
        if (this._rawString is not null) return this._rawString;
        if (this._memoryChar.HasValue) return this._memoryChar.Value.ToString();
        if (this._memoryByte.HasValue) return encoding.GetString(this._memoryByte.Value.ToArray());
        return string.Empty;
    }

    public override string ToString()
    {
        return this.ToString(Encoding.UTF8);
    }

    public static bool operator ==(SharpValue left, SharpValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SharpValue left, SharpValue right)
    {
        return !left.Equals(right);
    }
}
