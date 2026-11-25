using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SharpRedis;

/// <summary>
/// Read-only SharpValue collection. Support any collection type of string and byte[]
/// <para>只读的SharpValue集合, 支持string和byte[]的任意集合类型转换</para>
/// </summary>
/// <example>
/// <code>
/// SharpValueSpan keys = ["key1", "key2", byte[]] //Support is available only in .NET 8 and above.
/// 
/// string[] array = ["k1", "k2"];
/// SharpValueSpan keys = array;
/// 
/// List&lt;string&gt; list = ["k1", "k2"];
/// SharpValueSpan keys = list;
/// </code>
/// </example>
#if NET8_0_OR_GREATER
[global::System.Runtime.CompilerServices.CollectionBuilder(typeof(SharpValueSpanBuilder), nameof(SharpValueSpanBuilder.Create))]
#endif
public readonly ref struct SharpValueSpan
{
    private readonly ReadOnlySpan<SharpValue> _values;
#if MODERN_NET
    private readonly IEnumerable? _collection;
#else
    private readonly IEnumerable _collection;
#endif

    /// <summary>
    /// Returns an empty SharpValueSpan.
    /// <para>获得一个空的SharpValueSpan</para>
    /// </summary>
    public static SharpValueSpan Empty => default;

    /// <summary>
    /// Gets an item from the read-only span at the specified zero-based index.
    /// <para>根据下标获取元素</para>
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.
    /// <para>要获取的元素的下标</para>
    /// </param>
    /// <returns>The element from the read-only span.
    /// <para>获得到的元素值</para>
    /// </returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public readonly SharpValue this[int index]
    {
        get
        {
            if (this._collection is not null)
            {
                if (index < 0) throw new IndexOutOfRangeException();

                if (this._collection is IList<SharpValue> listValue)
                {
                    return listValue[index];
                }
                else if (this._collection is ICollection<SharpValue> collectionValue)
                {
                    if (index >= collectionValue.Count) throw new IndexOutOfRangeException();
                    using var enumerator = collectionValue.GetEnumerator();
                    for (int cIndex = 0; cIndex <= index; cIndex++)
                    {
                        if (!enumerator.MoveNext()) throw new IndexOutOfRangeException();
                        if (cIndex == index) return enumerator.Current;
                    }
                }
                else if (this._collection is IList list)
                {
                    return SharpValueSpan.ObjectToSharpValue(list[index]);
                }
                else if (this._collection is ICollection collection)
                {
                    if (index >= collection.Count) throw new IndexOutOfRangeException();
                }

                int i = 0;
                foreach (var item in this._collection)
                {
                    if (index == i)
                    {
                        return SharpValueSpan.ObjectToSharpValue(item);
                    }
                    i++;
                }
                throw new IndexOutOfRangeException();
            }
            else
            {
                return this._values[index];
            }
        }
    }

    /// <summary>
    /// The number of items in the read-only span.
    /// <para>获得元素个数</para>
    /// </summary>
    public readonly int Length
    {
        get
        {
            if (this._collection is not null)
            {
                if (this._collection is ICollection c)
                {
                    return c.Count;
                }
                else
                {
                    int count = 0;
                    foreach (var item in this._collection)
                    {
                        count++;
                    }
                    return count;
                }
            }
            else
            {
                return this._values.Length;
            }
        }
    }

    /// <summary>
    /// The number of items in the read-only span.
    /// <para>获得元素个数</para>
    /// </summary>
    public readonly int Count => this.Length;

    /// <summary>
    /// Returns a value that indicates the current read-only span is empty.
    /// <para>是否为空</para>
    /// </summary>
    public readonly bool IsEmpty => this.Length == 0;

    #region Ctors
    /// <summary>
    /// Create a SharpValueSpan based on ReadOnlySpan&lt;SharpValue&gt;
    /// <para>根据ReadOnlySpan&lt;SharpValue&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(ReadOnlySpan<SharpValue> values)
    {
        this._values = values;
    }

    /// <summary>
    /// Create a SharpValueSpan based on IEnumerable&lt;Memory&lt;char&gt;&gt;
    /// <para>根据IEnumerable&lt;Memory&lt;char&gt;&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(IEnumerable<Memory<char>> values)
    {
        this._collection = values;
    }

    /// <summary>
    /// Create a SharpValueSpan based on IEnumerable&lt;Memory&lt;byte&gt;&gt;
    /// <para>根据IEnumerable&lt;Memory&lt;byte&gt;&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(IEnumerable<Memory<byte>> values)
    {
        this._collection = values;
    }

    /// <summary>
    /// Create a SharpValueSpan based on IEnumerable&lt;SharpValue&gt;
    /// <para>根据IEnumerable&lt;SharpValue&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(IEnumerable<SharpValue> values)
    {
        this._collection = values;
    }

    /// <summary>
    /// Create a SharpValueSpan based on IEnumerable&lt;string&gt;
    /// <para>根据IEnumerable&lt;string&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(IEnumerable<string> values)
    {
        this._collection = values;
    }

    /// <summary>
    /// Create a SharpValueSpan based on IEnumerable&lt;byte[]&gt;
    /// <para>根据IEnumerable&lt;byte[]&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(IEnumerable<byte[]> values)
    {
        this._collection = values;
    }

    /// <summary>
    /// Create a SharpValueSpan based on IEnumerable&lt;ArraySegment&lt;char&gt;&gt;
    /// <para>根据IEnumerable&lt;ArraySegment&lt;char&gt;&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(IEnumerable<ArraySegment<char>> values)
    {
        this._collection = values;
    }

    /// <summary>
    /// Create a SharpValueSpan based on IEnumerable&lt;ArraySegment&lt;byte&gt;&gt;
    /// <para>根据IEnumerable&lt;ArraySegment&lt;byte&gt;&gt;创建SharpValueSpan</para>
    /// </summary>
    /// <param name="values"></param>
    public SharpValueSpan(IEnumerable<ArraySegment<byte>> values)
    {
        this._collection = values;
    }
#endregion

    /// <summary>
    /// Copies the contents of this read-only span into a new array.
    /// <para>将里面的值, 复制到一个新的数组中, 并返回</para>
    /// </summary>
    /// <returns>An array containing the data in the current span.
    /// <para>元素数组</para>
    /// </returns>
    public SharpValue[] ToArray()
    {
        if (this._collection is not null)
        {
            SharpValue[] array = new SharpValue[this.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this[i];
            }
            return array;
        }
        else
        {
            return this._values.ToArray();
        }
    }

    public override string ToString() => $"SharpRedis.SharpValueSpan. Length: {this.Length}";

    public SharpValueSpan.Enumerator GetEnumerator()
        => new(this);

    public static implicit operator SharpValueSpan(List<SharpValue> values) => new(values);
    public static implicit operator SharpValueSpan(string[] values) => new(values);
    public static implicit operator SharpValueSpan(List<string> values) => new(values);
    public static implicit operator SharpValueSpan(SharpValue[] values) => new(values);
    public static implicit operator SharpValueSpan(List<byte[]> values) => new(values);
    public static implicit operator SharpValueSpan(byte[][] values) => new(values);
    public static implicit operator SharpValueSpan(ArraySegment<byte>[] values) => new(values);
    public static implicit operator SharpValueSpan(ArraySegment<char>[] values) => new(values);
    public static implicit operator SharpValueSpan(List<ArraySegment<byte>> values) => new(values);
    public static implicit operator SharpValueSpan(List<ArraySegment<char>> values) => new(values);
    public static implicit operator SharpValueSpan(ReadOnlySpan<SharpValue> values) => new(values);
    public static implicit operator SharpValueSpan(Memory<char>[] values) => new(values);
    public static implicit operator SharpValueSpan(Memory<byte>[] values) => new(values);
    public static implicit operator SharpValueSpan(List<Memory<byte>> values) => new(values);
    public static implicit operator SharpValueSpan(List<Memory<char>> values) => new(values);

#pragma warning disable 0809
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on SharpValueSpan will always throw an exception.")]
    public override int GetHashCode()
        => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals() on SharpValueSpan will always throw an exception. Use the equality operator instead.")]
#if MODERN_NET
    public override bool Equals(object? obj)
#else
    public override bool Equals(object obj)
#endif
        => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if MODERN_NET
    private static SharpValue ObjectToSharpValue(object? obj)
#else
    private static SharpValue ObjectToSharpValue(object obj)
#endif
    {
        return obj switch
        {
            string str => str,
            SharpValue sv => sv,
            byte[] bytes => bytes,
            ArraySegment<char> asc => asc,
            ArraySegment<byte> asb => asb,
            Memory<char> mc => mc,
            Memory<byte> mb => mb,
            null => throw new NullReferenceException("Cannot convert null to SharpValue."),
            _ => throw new NotSupportedException($"Unsupported type {obj.GetType().FullName}"),
        };
    }

    public ref struct Enumerator
    {
        private int _index;
        private readonly int _length;
        private readonly SharpValueSpan _span;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(SharpValueSpan span)
        {
            this._span = span;
            this._length = span.Length;
            this._index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int index = this._index + 1;
            if (index < this._length)
            {
                this._index = index;
                return true;
            }
            return false;
        }

        public readonly SharpValue Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._span[this._index];
        }
    }

#if NET8_0_OR_GREATER
    public static class SharpValueSpanBuilder
    {
        public static SharpValueSpan Create(ReadOnlySpan<SharpValue> values)
        {
            if (values.IsEmpty) return SharpValueSpan.Empty;
            return new(values);
        }
    }
#endif
}
