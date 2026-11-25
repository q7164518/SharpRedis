using SharpRedis.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisHash
{
    /// <summary>
    /// The return value structure of the HSCAN command
    /// <para>HSCAN命令返回值结构体</para>
    /// </summary>
    public readonly struct HashScanNoValuesResult : IEnumerable<RedisResult>
    {
        internal static readonly HashScanNoValuesResult _empty = new();
        private readonly ulong _cursor;
        private readonly RedisResult[] _elements;

        /// <summary>
        /// Get the next available cursor. If it is 0, it indicates that the current reading has been completed.
        /// <para>获得下次可用的游标值, 如果返回0, 表示读取已经结束</para>
        /// </summary>
        public readonly ulong NextCursor => this._cursor;

        /// <summary>
        /// Get the number of returned elements.
        /// <para>获取返回的元素数量</para>
        /// </summary>
        public readonly int Count => this._elements.Length;

        /// <summary>
        /// Get the number of returned elements.
        /// <para>获取返回的元素数量</para>
        /// </summary>
        public readonly int Length => this._elements.Length;

        /// <summary>
        /// Gets the collection of fields contained in the result.
        /// <para>获得结果集, 字段数组</para>
        /// </summary>
        public readonly RedisResult[] Elements => this._elements;

        public HashScanNoValuesResult()
        {
            this._elements = null!;
        }

        internal HashScanNoValuesResult(RedisResult value)
        {
            if (value.Type != RespType.Array
                || value.RawValue is not RedisResult[] { Length: 2 } array
                || array[1].Type != RespType.Array)
            {
                throw new InvalidCastException("The return value is not an array and cannot be converted to HashScanResult.");
            }

            this._cursor = array[0];
            if (array[1].IsNull || !array[1].HasValue)
            {
                this._elements = [];
                return;
            }
            if (array[1].RawValue is RedisResult[] elements)
            {
                var result = new RedisResult[elements.Length];
                for (int i = 0; i < elements.Length; i++)
                {
                    result[i] = elements[i];
                }
                this._elements = result;
                return;
            }
            throw new InvalidCastException("The return value is not an array and cannot be converted to HashScanResult.");
        }

        /// <summary>
        /// Retrieve the element based on the subscript
        /// <para>根据下标获取元素</para>
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>element</returns>
        public readonly RedisResult this[int index]
            => this._elements[index];

        public IEnumerator<RedisResult> GetEnumerator()
            => new SharpEnumerator<RedisResult>(this._elements);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public void Deconstruct(out ulong cursor, out RedisResult[] elements)
        {
            cursor = this._cursor;
            elements = this._elements;
        }
    }

    /// <summary>
    /// The return value structure of the HSCAN command
    /// <para>HSCAN命令返回值结构体</para>
    /// </summary>
    public readonly struct HashScanResult : IEnumerable<KeyValuePair<RedisResult, RedisResult>>
    {
        internal static readonly HashScanResult _empty = new();
        private readonly ulong _cursor;
        private readonly KeyValuePair<RedisResult, RedisResult>[] _elements;

        /// <summary>
        /// Get the next available cursor. If it is 0, it indicates that the current reading has been completed.
        /// <para>获得下次可用的游标值, 如果返回0, 表示读取已经结束</para>
        /// </summary>
        public readonly ulong NextCursor => this._cursor;

        /// <summary>
        /// Get the number of returned elements.
        /// <para>获取返回的元素数量</para>
        /// </summary>
        public readonly int Count => this._elements.Length;

        /// <summary>
        /// Get the number of returned elements.
        /// <para>获取返回的元素数量</para>
        /// </summary>
        public readonly int Length => this._elements.Length;

        /// <summary>
        /// Gets the collection of elements contained in the result as key/value pairs.
        /// <para>获得结果集, 键值对数组</para>
        /// </summary>
        public readonly KeyValuePair<RedisResult, RedisResult>[] Elements => this._elements;

        public HashScanResult()
        {
            this._elements = null!;
        }

        internal HashScanResult(RedisResult value)
        {
            if (value.Type != RespType.Array
                || value.RawValue is not RedisResult[] { Length: 2 } array
                || array[1].Type != RespType.Array)
            {
                throw new InvalidCastException("The return value is not an array and cannot be converted to HashScanResult.");
            }

            this._cursor = array[0];
            if (array[1].IsNull || !array[1].HasValue)
            {
                this._elements = [];
                return;
            }

            if (array[1].RawValue is RedisResult[] elements)
            {
                var result = new KeyValuePair<RedisResult, RedisResult>[elements.Length / 2];
                for (int i = 0, index = 0; i < elements.Length; i += 2, index++)
                {
                    result[index] = new KeyValuePair<RedisResult, RedisResult>(elements[i], elements[i + 1]);
                }
                this._elements = result;
                return;
            }
            throw new InvalidCastException("The return value is not an array and cannot be converted to HashScanResult.");
        }

        /// <summary>
        /// Retrieve the element based on the subscript
        /// <para>根据下标获取元素</para>
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>element</returns>
        public readonly KeyValuePair<RedisResult, RedisResult> this[int index]
            => this._elements[index];

        public IEnumerator<KeyValuePair<RedisResult, RedisResult>> GetEnumerator()
            => new Enumerator(this._elements);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public struct Enumerator : IEnumerator<KeyValuePair<RedisResult, RedisResult>>
        {
            private readonly KeyValuePair<RedisResult, RedisResult>[] _items;
            private int _index;

            public Enumerator()
            {
                throw new NotImplementedException();
            }

            internal Enumerator(KeyValuePair<RedisResult, RedisResult>[] items)
            {
                this._items = items;
                this._index = -1;
            }

            public readonly KeyValuePair<RedisResult, RedisResult> Current => this._items[_index];

            readonly object IEnumerator.Current => this.Current;

            public bool MoveNext() => ++this._index < this._items.Length;

            public void Reset() => this._index = -1;

            public readonly void Dispose() { }
        }
    }
}
