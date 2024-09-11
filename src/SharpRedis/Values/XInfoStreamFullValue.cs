#pragma warning disable IDE0130
using SharpRedis.Extensions;
using System;
using System.Text;

namespace SharpRedis
{
    public sealed class XInfoStreamFullValue<TValue> : BaseXInfoStreamValue
        where TValue : class
    {
        /// <summary>
        /// The FULL reply includes an entries array that consists of the stream entries (ID and field-value tuples) in ascending order
        /// <para>Stream内容条目, 按ID升序排序的</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public StreamValue<TValue>[]? Entries { get; private set; }
#else
        public StreamValue<TValue>[] Entries { get; private set; }
#endif

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public FullGroup[]? Groups { get; private set; }
#else
        public FullGroup[] Groups { get; private set; }
#endif

        internal XInfoStreamFullValue(object data, Encoding encoding, ResultType valueType)
            : base(data, encoding, valueType)
        {
            if (data is object[] array)
            {
                for (uint i = 0; i < array.Length; i += 2)
                {
                    var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                    switch (key)
                    {
                        case "entries":
                            this.Entries = ConvertExtensions.To<StreamValue<TValue>[]>(array[i + 1], ResultType.Array | ResultType.Stream | valueType, encoding);
                            continue;
                        case "groups":
                            if (array[i + 1] is object[] groups)
                            {
                                this.Groups = new FullGroup[groups.Length];
                                for (uint z = 0; z < groups.Length; z++)
                                {
                                    this.Groups[z] = new FullGroup(groups[z], encoding);
                                }
                            }
                            continue;
                        default: continue;
                    }
                }
            }
        }

        public sealed class FullGroup : BaseXInfoGroupsValue
        {
            /// <summary>
            /// The length of the group's pending entries list (PEL), which are messages that were delivered but are yet to be acknowledged
            /// <para>消费组待处理条目列表（PEL）的长度，这些消息已发送但尚未确认</para>
            /// </summary>
            public long PelCount { get; private set; }

            /// <summary>
            /// An array with consumers information
            /// <para>消费组中的消费者数组</para>
            /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            public FullConsumer[]? Consumers { get; private set; }
#else
            public FullConsumer[] Consumers { get; private set; }
#endif

            /// <summary>
            /// An array with pending entries information
            /// <para>当前消费组待处理的消息信息</para>
            /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            public FullGroupPending[]? Pending { get; private set; }
#else
            public FullGroupPending[] Pending { get; private set; }
#endif

            internal FullGroup(object data, Encoding encoding)
                : base(data, encoding)
            {
                if (data is object[] array)
                {
                    for (uint i = 0; i < array.Length; i++)
                    {
                        var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                        switch (key)
                        {
                            case "pel-count":
                                this.PelCount = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                                continue;
                            case "pending":
                                if (array[i + 1] is object[] pending)
                                {
                                    this.Pending = new FullGroupPending[pending.Length];
                                    for (uint z = 0; z < pending.Length; z++)
                                    {
                                        this.Pending[z] = new FullGroupPending(pending[z], encoding);
                                    }
                                }
                                continue;
                            case "consumers":
                                if (array[i + 1] is object[] consumers)
                                {
                                    this.Consumers = new FullConsumer[consumers.Length];
                                    for (uint z = 0; z < consumers.Length; z++)
                                    {
                                        this.Consumers[z] = new FullConsumer(consumers[z], encoding);
                                    }
                                }
                                continue;
                            default: continue;
                        }
                    }
                }
            }
        }

        public sealed class FullConsumer : BaseXInfoConsumersValue
        {
            /// <summary>
            /// The UNIX timestamp of the last attempted interaction
            /// <para>Note that before Redis 7.2.0, seen-time used to denote the last successful interaction</para>
            /// <para>上次尝试交互的时间戳, 毫秒级. 注意, 在Redis 7.2.0之前, SeenTime表示最后一次成功交互的时间戳</para>
            /// </summary>
            public long SeenTime { get; private set; } = -1;

            /// <summary>
            /// The UNIX timestamp of the last successful interaction. Redis 7.2.0
            /// <para>上次成功交互的时间戳, 毫秒级, Redis 7.2.0及之后才支持</para>
            /// </summary>
            public long ActiveTime { get; private set; } = -1;

            /// <summary>
            /// The number of entries in the PEL: pending messages for the consumer, which are messages that were delivered but are yet to be acknowledged
            /// <para>PEL 中的条目数：消费者的待处理消息，这些消息已发送但尚未确认</para>
            /// </summary>
            public long PelCount { get; private set; }

            /// <summary>
            /// An array with pending entries information
            /// <para>当前消费者待处理的消息信息</para>
            /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            public FullConsumerPending[]? Pending { get; private set; }
#else
            public FullConsumerPending[] Pending { get; private set; }
#endif

            internal FullConsumer(object data, Encoding encoding)
                : base(data, encoding)
            {
                if (data is object[] array)
                {
                    for (uint i = 0; i < array.Length; i++)
                    {
                        var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                        switch (key)
                        {
                            case "seen-time":
                                this.SeenTime = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                                continue;
                            case "active-time":
                                this.ActiveTime = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                                continue;
                            case "pel-count":
                                this.PelCount = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                                continue;
                            case "pending":
                                if (array[i + 1] is object[] pending)
                                {
                                    this.Pending = new FullConsumerPending[pending.Length];
                                    for (uint z = 0; z < pending.Length; z++)
                                    {
                                        this.Pending[z] = new FullConsumerPending(pending[z], encoding);
                                    }
                                }
                                continue;
                            default: continue;
                        }
                    }
                }
            }
        }

        public sealed class FullGroupPending
        {
            /// <summary>
            /// The ID of the message
            /// <para>消息ID</para>
            /// </summary>
            public string MessageID { get; private set; }

            /// <summary>
            /// The name of the consumer that fetched the message and has still to acknowledge it. We call it the current owner of the message
            /// <para>获取消息且仍需确认的消费者的名称。我们称其为消息的当前所有者。</para>
            /// </summary>
            public string Consumer { get; private set; }

            /// <summary>
            /// The UNIX timestamp of when the message was delivered to this consumer
            /// <para>消息传递给该消费者时的时间戳, 毫秒级</para>
            /// </summary>
            public long DeliveredTime { get; private set; }

            /// <summary>
            /// The number of times this message was delivered
            /// <para>该消息被传送的次数</para>
            /// </summary>
            public long DeliveredCount { get; private set; }

            internal FullGroupPending(object data, Encoding encoding)
            {
                if (data is object[] array && array.Length is 4)
                {
                    this.MessageID = ConvertExtensions.To<string>(array[0], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
                    this.Consumer = ConvertExtensions.To<string>(array[1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;

                    this.DeliveredTime = ConvertExtensions.To<long>(array[2], ResultType.Int64, encoding);
                    this.DeliveredCount = ConvertExtensions.To<long>(array[3], ResultType.Int64, encoding);
                    return;
                }
                throw new FormatException("Unrecognized pending format");
            }
        }

        public sealed class FullConsumerPending
        {
            /// <summary>
            /// The ID of the message
            /// <para>消息ID</para>
            /// </summary>
            public string MessageID { get; private set; }

            /// <summary>
            /// The UNIX timestamp of when the message was delivered to this consumer
            /// <para>消息传递给该消费者时的时间戳, 毫秒级</para>
            /// </summary>
            public long DeliveredTime { get; private set; }

            /// <summary>
            /// The number of times this message was delivered
            /// <para>该消息被传送的次数</para>
            /// </summary>
            public long DeliveredCount { get; private set; }

            internal FullConsumerPending(object data, Encoding encoding)
            {
                if (data is object[] array && array.Length is 3)
                {
                    this.MessageID = ConvertExtensions.To<string>(array[0], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;

                    this.DeliveredTime = ConvertExtensions.To<long>(array[1], ResultType.Int64, encoding);
                    this.DeliveredCount = ConvertExtensions.To<long>(array[2], ResultType.Int64, encoding);
                    return;
                }
                throw new FormatException("Unrecognized pending format");
            }
        }
    }
}
