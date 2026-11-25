using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpRedis;

public sealed partial class RedisList
{
    /// <summary>
    /// Multiple List pop return value struct
    /// <para>多个List弹出的返回值结构体</para>
    /// </summary>
    public readonly struct ListBlockPopResult
    {
        private readonly RedisResult _key;
        private readonly RedisResult _element;

        /// <summary>
        /// Get the List key for the popped elements
        /// <para>获得弹出元素的List key</para>
        /// </summary>
        public readonly RedisResult Key => this._key;

        /// <summary>
        /// Get the popped element
        /// <para>获得已弹出的元素</para>
        /// </summary>
        public readonly RedisResult Element => this._element;

        /// <summary>
        /// Check if no results were returned
        /// <para>是否没有返回任何结果</para>
        /// </summary>
        public readonly bool IsNull => this._key.IsNull && this._element.IsNull;

        internal ListBlockPopResult(RedisResult result)
        {
            if (result.IsNull)
            {
                this._key = RedisResult.Empty;
                this._element = RedisResult.Empty;
                return;
            }

            if (result.Type != Enums.RespType.Array
                || result.RawValue is not RedisResult[] { Length: 2 } array)
            {
                throw new FormatException($"Invalid BLPOP/BRPOP response format, The actual type is {result.RawValue.GetType().FullName}");
            }

            this._key = array[0];
            this._element = array[1];
        }

        public void Deconstruct(out RedisResult key, out RedisResult element)
        {
            key = this._key;
            element = this._element;
        }
    }

    /// <summary>
    /// Multiple List pop return value struct
    /// <para>多个List弹出的返回值结构体</para>
    /// </summary>
    public readonly struct ListPopResult : IEnumerable<RedisResult>
    {
        internal static readonly ListPopResult _empty = new();
        private readonly RedisResult _key;
#if MODERN_NET
        private readonly RedisResult[]? _elements;
#else
    private readonly RedisResult[] _elements;
#endif

        /// <summary>
        /// Check if no results were returned
        /// <para>是否没有返回任何结果</para>
        /// </summary>
#if !NETSTANDARD
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(Elements))]
        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(_elements))]
#endif
        public readonly bool IsNull => this._key.IsNull && this._elements is null;

        /// <summary>
        /// Get the List key for the popped elements
        /// <para>获得弹出元素的List key</para>
        /// </summary>
        public readonly RedisResult Key => this._key;

        /// <summary>
        /// Get the number of returned elements.
        /// <para>获取返回的元素数量</para>
        /// </summary>
        public readonly int Count => this._elements?.Length ?? -1;

        /// <summary>
        /// Get the popped elements
        /// <para>获得已弹出的元素</para>
        /// </summary>
#if MODERN_NET
        public readonly RedisResult[]? Elements => this._elements;
#else
        public readonly RedisResult[] Elements => this._elements;
#endif

        public ListPopResult() { }

        internal ListPopResult(RedisResult result)
        {
            if (result.IsNull)
            {
                this._key = RedisResult.Empty;
                return;
            }

            if (result.Type != Enums.RespType.Array
                || result.RawValue is not RedisResult[] { Length: 2 } array
                || array[1].Type != Enums.RespType.Array
                || array[1].RawValue is not RedisResult[] elements)
            {
                throw new FormatException($"Invalid LMPOP/BLMPOP response format, The actual type is {result.RawValue.GetType().FullName}");
            }

            this._key = array[0];
            this._elements = elements;
        }

        /// <summary>
        /// Retrieve the element based on the subscript
        /// <para>根据下标获取元素</para>
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>element</returns>
        public readonly RedisResult this[int index]
        {
            get
            {
                if (this.IsNull) throw new NullReferenceException("No elements were returned.");
                return this._elements![index];
            }
        }

        public IEnumerator<RedisResult> GetEnumerator()
        {
            if (this.IsNull) throw new NullReferenceException("No elements were returned.");
            return new SharpEnumerator<RedisResult>(this._elements!);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

#if MODERN_NET
        public void Deconstruct(out RedisResult key, out RedisResult[]? elements)
#else
        public void Deconstruct(out RedisResult key, out RedisResult[] elements)
#endif
        {
            key = this._key;
            elements = this._elements;
        }
    }
}
