#pragma warning disable IDE0130
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRedis
{
    public sealed class StreamValue<TValue>
        where TValue : class
    {
        private readonly string _id;

        /// <summary>
        /// Gets ID
        /// <para>获得内容条目ID</para>
        /// </summary>
        public string ID => this._id;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private readonly KeyValuePair<string, TValue>[]? _fvArray;

        /// <summary>
        /// Gets the entry, which is a key-value array, where Key represents field and value represents Value
        /// <para>获得内容条目, 是一个键值对数组, 键表示field, 值表示value</para>
        /// </summary>
        public KeyValuePair<string, TValue>[]? FieldValue => this._fvArray;
#else
        private readonly KeyValuePair<string, TValue>[] _fvArray;

        /// <summary>
        /// Gets the entry, which is a key-value array, where Key represents field and value represents Value
        /// <para>获得内容条目, 是一个键值对数组, 键表示field, 值表示value</para>
        /// </summary>
        public KeyValuePair<string, TValue>[] FieldValue => this._fvArray;
#endif

        internal StreamValue(object data, Encoding encoding, ResultType valueType)
        {
            if (data is object[] array && array.Length is 2)
            {
                var id = ConvertExtensions.To<string>(array[0], ResultType.String, encoding)
                    ?? throw new FormatException($"The data is not a valid StreamValue, The actual type is {data.GetType().FullName}");
                this._id = id;

                var fvs = ConvertExtensions.To<KeyValuePair<string, TValue>[]>(array[1], ResultType.KeyValuePairArray | valueType, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        !
#endif
                        ;
                this._fvArray = fvs;
                return;
            }

            if (data is byte[])
            {
                var id = ConvertExtensions.To<string>(data, ResultType.String, encoding)
                    ?? throw new FormatException($"The data is not a valid StreamValue, The actual type is {data.GetType().FullName}");
                this._id = id;
                return;
            }
            throw new FormatException($"The data is not a valid StreamValue, The actual type is {data.GetType().FullName}");
        }

        public override int GetHashCode()
        {
            if (this._fvArray != null)
            {
                return this._id.GetHashCode() ^ this._fvArray.GetHashCode();
            }
            return this._id.GetHashCode();
        }

        /// <summary>
        /// Deconstruct, var (id, fieldValue) = this
        /// <para>解构函数, var (id, fieldValue) = this</para>
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="fieldValue">fieldValue</param>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public void Deconstruct(out string id, out KeyValuePair<string, TValue>[]? fieldValue)
#else
        public void Deconstruct(out string id, out KeyValuePair<string, TValue>[] fieldValue)
#endif
        {
            id = this._id;
            fieldValue = this._fvArray;
        }
    }
}
