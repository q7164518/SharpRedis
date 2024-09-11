#pragma warning disable IDE0130
#if NET5_0_OR_GREATER
#pragma warning disable IDE0083
#endif
using SharpRedis.Extensions;
using System;
using System.Text;

namespace SharpRedis
{
    public sealed class XInfoGroupsValue : BaseXInfoGroupsValue
    {
        /// <summary>
        /// The number of consumers in the group
        /// <para>消费组中的消费者数量</para>
        /// </summary>
        public long Consumers { get; private set; }

        /// <summary>
        /// The length of the group's pending entries list (PEL), which are messages that were delivered but are yet to be acknowledged
        /// <para>消费组中待处理消息数量(未确认的消息数量)</para>
        /// </summary>
        public long Pending { get; private set; }

        internal XInfoGroupsValue(object data, Encoding encoding)
            : base(data, encoding)
        {
            if (data is object[] array)
            {
                for (uint i = 0; i < array.Length; i += 2)
                {
                    var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                    switch (key)
                    {
                        case "pending":
                            this.Pending = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                            continue;
                        case "consumers":
                            this.Consumers = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                            continue;
                        default: continue;
                    }
                }
            }
        }
    }

    public abstract class BaseXInfoGroupsValue
    {
        /// <summary>
        /// The consumer group's name
        /// <para>消费组名称</para>
        /// </summary>
        public string Name { get; private protected set; }

        /// <summary>
        /// The ID of the last entry delivered to the group's consumers
        /// <para>最后交付给该组中消费者的条目的 ID</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? LastDeliveredID { get; private protected set; }
#else
        public string LastDeliveredID { get; private protected set; }
#endif

        /// <summary>
        /// The logical "read counter" of the last entry delivered to the group's consumers
        /// <para>Available since: 7.0.0. Redis below this version remains -1</para>
        /// <para>传递给组消费者的最后一个条目的逻辑“读计数器”. Redis 7.0.0+才有该返回值, 否则一直为-1</para>
        /// </summary>
        public long EntriesRead { get; private protected set; } = -1;

        /// <summary>
        /// The number of entries in the stream that are still waiting to be delivered to the group's consumers, or a NULL when that number can't be determined
        /// <para>Available since: 7.0.0</para>
        /// <para>Stream中仍在等待传递给组消费者的条目数，或者当无法确定该数字时为 NULL. Redis 7.0.0+才有该返回值</para>
        /// </summary>
        public long? Lag { get; private protected set; }

        private protected BaseXInfoGroupsValue(object data, Encoding encoding)
        {
            if (!(data is object[] array) || array.Length % 2 != 0)
            {
                throw new FormatException($"The data is not a valid XInfoGroupsValue, The actual type is {data.GetType().FullName}");
            }

            for (uint i = 0; i < array.Length; i += 2)
            {
                var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding);
                switch (key)
                {
                    case "name":
                        this.Name = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            !
#endif
                            ;
                        continue;
                    case "entries-read":
                        this.EntriesRead = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                        continue;
                    case "lag":
                        var lag = ConvertExtensions.To<NumberValue>(array[i + 1], ResultType.Number, encoding);
                        if (lag != null && lag.HasValue) this.Lag = lag;
                        continue;
                    case "last-delivered-id":
                        this.LastDeliveredID = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    !
#endif
    ;
                        continue;
                    default: continue;
                }
            }

            if (this.Name is null) throw new FormatException("Unrecognized name format");
        }
    }
}
