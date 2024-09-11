#pragma warning disable IDE0130
#if NET5_0_OR_GREATER
#pragma warning disable IDE0083
#endif
using SharpRedis.Extensions;
using System;
using System.Text;

namespace SharpRedis
{
    public sealed class XInfoConsumersValue : BaseXInfoConsumersValue
    {
        /// <summary>
        /// The number of entries in the PEL: pending messages for the consumer, which are messages that were delivered but are yet to be acknowledged
        /// <para>获得待处理的消息数量</para>
        /// </summary>
        public long Pending { get; private set; }

        /// <summary>
        /// The number of milliseconds that have passed since the consumer's last attempted interaction
        /// <para>自消费者最后一次尝试交互之后的毫秒数</para>
        /// </summary>
        public long Idle { get; private set; }

        /// <summary>
        /// The number of milliseconds that have passed since the consumer's last successful interaction
        /// <para>Available since: 7.2.0. Redis below this version remains -1</para>
        /// <para>自消费者最后一次成功交互之后的毫秒数, Redis 7.2.0+才有, 否则一直为-1</para>
        /// </summary>
        public long Inactive { get; private set; } = -1;

        internal XInfoConsumersValue(object data, Encoding encoding)
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
                        case "idle":
                            this.Idle = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                            continue;
                        case "inactive":
                            this.Inactive = ConvertExtensions.To<long>(array[i + 1], ResultType.Int64, encoding);
                            continue;
                        default: continue;
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"Name: {this.Name}, Pending: {this.Pending}, Idle: {this.Idle}, Inactive: {this.Inactive}";
        }
    }

    public abstract class BaseXInfoConsumersValue
    {
        /// <summary>
        /// The consumer's name
        /// <para>获得消费者名称</para>
        /// </summary>
        public string Name { get; private protected set; }

        private protected BaseXInfoConsumersValue(object data, Encoding encoding)
        {
            if (!(data is object[] array) || array.Length % 2 != 0)
            {
                throw new FormatException($"The data is not a valid XInfoConsumersValue, The actual type is {data.GetType().FullName}");
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
                        goto End;
                    default: continue;
                }
            }

            End:

            if (this.Name is null) throw new FormatException("Unrecognized name format");
        }
    }
}
