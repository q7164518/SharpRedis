#if !LOW_NET
using SharpRedis.Commands;
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
        public Task<bool> PFAddAsync(string key, string element, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(HyperLogLogCommands.PFAdd(key, element), "1", cancellationToken);
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
        public Task<bool> PFAddAsync(string key, byte[] element, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(HyperLogLogCommands.PFAdd<byte[]>(key, element), "1", cancellationToken);
        }

        /// <summary>
        /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument
        /// <para>Available since: 2.8.9</para>
        /// <para>添加多个元素到指定的HyperLogLog中</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="elements">elements</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>true: if at least one HyperLogLog internal register was altered
        /// <para>false: if no HyperLogLog internal registers were altered</para>
        /// <para>true: 至少有一个元素被更改</para>
        /// <para>false: 没有元素被更改</para>
        /// </returns>
        public Task<bool> PFAddAsync(string key, string[] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(HyperLogLogCommands.PFAdd(key, elements).WriteValues(elements), "1", cancellationToken);
        }

        /// <summary>
        /// Adds all the element arguments to the HyperLogLog data structure stored at the variable name specified as first argument
        /// <para>Available since: 2.8.9</para>
        /// <para>添加多个元素到指定的HyperLogLog中</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="elements">elements</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>true: if at least one HyperLogLog internal register was altered
        /// <para>false: if no HyperLogLog internal registers were altered</para>
        /// <para>true: 至少有一个元素被更改</para>
        /// <para>false: 没有元素被更改</para>
        /// </returns>
        public Task<bool> PFAddAsync(string key, byte[][] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallConditionAsync(HyperLogLogCommands.PFAdd(key, elements), "1", cancellationToken);
        }

        /// <summary>
        /// When called with a single key, returns the approximated cardinality computed by the HyperLogLog data structure stored at the specified variable, which is 0 if the variable does not exist
        /// <para>Available since: 2.8.9</para>
        /// <para>当使用单个键调用时，返回由存储在指定变量中的 HyperLogLog 数据结构计算得出的近似基数，如果该变量不存在，则返回 0</para>
        /// <para>支持此命令的Redis版本, 2.8.9+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="keys">keys</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The approximated number of unique elements observed via PFADD
        /// <para>通过PFADD观察到的唯一元素的近似数量</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<long> PFCountAsync(string key, string[]? keys = null, CancellationToken cancellationToken = default)
#else
        public Task<long> PFCountAsync(string key, string[] keys = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallNumberAsync<long>(HyperLogLogCommands.PFCount(key, keys), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Merge multiple HyperLogLog values into a unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures
        /// <para>Available since: 2.8.9</para>
        /// <para>将多个 HyperLogLog 值合并为一个唯一值，该值将近似观察到的源 HyperLogLog 结构集的并集基数</para>
        /// </summary>
        /// <param name="destkey">destkey
        /// <para>存储结果的Key</para>
        /// </param>
        /// <param name="sourceKey">source key</param>
        /// <param name="sourcekeys">sourcekey array
        /// <para>指定合并的Key数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<bool> PFMergeAsync(string destkey, string? sourceKey = null, string[]? sourcekeys = null, CancellationToken cancellationToken = default)
#else
        public Task<bool> PFMergeAsync(string destkey, string sourceKey = null, string[] sourcekeys = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallConditionAsync(HyperLogLogCommands.PFMerge(destkey, sourceKey, sourcekeys), "OK", cancellationToken);
        }
    }
}
#endif
