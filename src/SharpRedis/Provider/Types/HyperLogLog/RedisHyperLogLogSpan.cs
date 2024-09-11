#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisHyperLogLog
    {
        /// <summary>
        /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument
        /// <para>Available since: 2.8.9</para>
        /// <para>添加一个元素到指定的HyperLogLog中</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="element">element</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>true: if at least one HyperLogLog internal register was altered
        /// <para>false: if no HyperLogLog internal registers were altered</para>
        /// <para>true: 至少有一个元素被更改</para>
        /// <para>false: 没有元素被更改</para>
        /// </returns>
        public bool PFAdd(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.PFAdd(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument
        /// <para>Available since: 2.8.9</para>
        /// <para>添加一个元素到指定的HyperLogLog中</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="element">element</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>true: if at least one HyperLogLog internal register was altered
        /// <para>false: if no HyperLogLog internal registers were altered</para>
        /// <para>true: 至少有一个元素被更改</para>
        /// <para>false: 没有元素被更改</para>
        /// </returns>
        public Task<bool> PFAddAsync(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.PFAddAsync(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
        }
    }
}
#endif
