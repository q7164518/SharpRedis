#pragma warning disable IDE0130
#if NET5_0_OR_GREATER
#pragma warning disable IDE0083
#endif
using SharpRedis.Extensions;
using System;
using System.Text;

namespace SharpRedis
{
    public sealed class XInfoStreamValue<TValue> : BaseXInfoStreamValue
        where TValue : class
    {
        /// <summary>
        /// The number of consumer groups defined for the stream
        /// <para>当前Stream上存在的消费者组（consumer group）数量</para>
        /// </summary>
        public long Groups { get; private set; }

        /// <summary>
        /// The ID and field-value tuples of the first entry in the stream
        /// <para>当前Stream中第一个内容条目的ID及其数据</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<TValue>? FirstEntry { get; private set; }
#else
        public StreamValue<TValue> FirstEntry { get; private set; }
#endif

        /// <summary>
        /// The ID and field-value tuples of the last entry in the stream
        /// <para>当前Stream中最后一个内容条目的ID及其数据</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<TValue>? LastEntry { get; private set; }
#else
        public StreamValue<TValue> LastEntry { get; private set; }
#endif

        internal XInfoStreamValue(object data, Encoding encoding, ResultType valueType)
            : base(data, encoding, valueType)
        {
            if (data is object[] array)
            {
                for (uint i = 0; i < array.Length; i += 2)
                {
                    var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                    switch (key)
                    {
                        case "groups":
                            this.Groups = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                            continue;
                        case "first-entry":
                            this.FirstEntry = ConvertExtensions.To<StreamValue<TValue>>(array[i + 1], ResultType.Stream | valueType, encoding);
                            continue;
                        case "last-entry":
                            this.LastEntry = ConvertExtensions.To<StreamValue<TValue>>(array[i + 1], ResultType.Stream | valueType, encoding);
                            continue;
                        default: continue;
                    }
                }
            }
        }
    }

    public abstract class BaseXInfoStreamValue
    {
        /// <summary>
        /// The number of entries in the stream
        /// <para>Stream中的内容条目数量</para>
        /// </summary>
        public long Length { get; private protected set; }

        /// <summary>
        /// The number of keys in the underlying radix data structure
        /// <para>内部 Radix 树的节点数量（键数）。Redis 使用 Radix 树存储流中的条目。这个值表示用于存储流的 Radix 树中的键的数量</para>
        /// </summary>
        public long RadixTreeKeys { get; private protected set; }

        /// <summary>
        /// The number of nodes in the underlying radix data structure
        /// <para>内部 Radix 树的总节点数量。Redis 使用 Radix 树来高效地存储和查找条目，这个值表示 Radix 树中总共的节点数（包括内部节点和叶节点）</para>
        /// </summary>
        public long RadixTreeNodes { get; private protected set; }

        /// <summary>
        /// The ID of the least-recently entry that was added to the stream
        /// <para>当前Stream中最后生成的内容条目的ID。每次使用 XADD 向流中添加内容条目时，Redis 会生成一个唯一的ID，这个字段表示流中最新的那个内容条目的ID</para>
        /// </summary>
        public string LastGeneratedID { get; private protected set; }

        /// <summary>
        /// The maximal entry ID that was deleted from the stream
        /// <para>当前Stream中已删除内容条目的最大ID。这个字段表示Stream中所有已删除内容条目中ID的最大值。如果没有删除条目，通常为0-0</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? MaxDeletedEntryID { get; private protected set; }
#else
        public string MaxDeletedEntryID { get; private protected set; }
#endif

        /// <summary>
        /// The count of all entries added to the stream during its lifetime
        /// <para>自Stream创建以来添加的总内容条目数。包括当前存在的和已被删除的内容条目</para>
        /// </summary>
        public long EntriesAdded { get; private protected set; }

        /// <summary>
        /// The minimum entry ID currently present in the stream (that is, the "first entry of record" ID)
        /// <para>Available since: 5.0.0</para>
        /// <para>当前Stream中存在的最小内容条目ID（即“Stream中的第一个条目”ID）</para>
        /// <para>Redis 7.0.0+以后才会返回, 否则一直为null</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? RecordedFirstEntryID { get; private protected set; }
#else
        public string RecordedFirstEntryID { get; private protected set; }
#endif

        private protected BaseXInfoStreamValue(object data, Encoding encoding, ResultType valueType)
        {
            if (!(data is object[] array) || array.Length % 2 != 0)
            {
                throw new FormatException($"The data is not a valid XInfoStreamValue, The actual type is {data.GetType().FullName}");
            }

            for (uint i = 0; i < array.Length; i += 2)
            {
                var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                switch (key)
                {
                    case "length":
                        this.Length = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                        continue;
                    case "radix-tree-keys":
                        this.RadixTreeKeys = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                        continue;
                    case "radix-tree-nodes":
                        this.RadixTreeNodes = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                        continue;
                    case "last-generated-id":
                        this.LastGeneratedID = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
                        continue;
                    case "max-deleted-entry-id":
                        this.MaxDeletedEntryID = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
                        continue;
                    case "entries-added":
                        this.EntriesAdded = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                        continue;
                    case "recorded-first-entry-id":
                        this.RecordedFirstEntryID = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding);
                        continue;
                    default: continue;
                }
            }

            if (this.LastGeneratedID is null)
            {
                throw new FormatException("Unrecognized last-generated-id format");
            }
        }
    }
}
