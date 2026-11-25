using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpRedis.Modules;

/// <summary>
/// Count-min sketch Operation Class
/// <para><b>ℹ️Note: The RedisBloom 2.0.0 extension module is required, or a version of Redis 8.0 or above must be used. Otherwise, any command operation under this attribute will result in an exception.</b></para>
/// <para>Count-min sketch操作类. (Count-min 草图是一种概率数据结构，用于估计数据流中元素的频率。)</para>
/// <para><b>ℹ️提示: 需要有RedisBloom 2.0.0+ 扩展模块, 或者Redis8.0以上版本, 否则该属性下的命令操作都会引发异常</b></para>
/// <para>If you are new to Count-min sketch, see <see href="https://redis.io/docs/develop/data-types/probabilistic/count-min-sketch">Introduction to Redis Count-min sketch</see></para>
/// <para>如果你不太懂Count-min sketch, 请<see href="https://redis.io/docs/develop/data-types/probabilistic/count-min-sketch">查看Count-min sketch文档</see></para>
/// </summary>
public sealed partial class RedisCountMinSketch : RedisTypeBase
{
    internal RedisCountMinSketch(BaseRedis redis) : base(redis)
    { }

    /// <summary>
    /// Increases the count of item by increment. Multiple items can be increased with one call.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将 Count-Min Sketch 中指定元素的计数增加指定增量</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="item">To add item for increment
    /// <para>要增加计数的元素</para>
    /// </param>
    /// <param name="increment">The increase
    /// <para>增加的量</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The increment minimum count value
    /// <para>更新后的最小计数值（min-count）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.incrby">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.incrby">命令文档</see></para>
    /// </remarks>
    public IncrByResult CMS_IncrBy(
        SharpValue key,
        SharpValue item,
        long increment,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_IncrBy(
            key: key,
            item: item,
            increment: increment) | flags, cancellationToken);
    }

    /// <summary>
    /// Increases the count of item by increment. Multiple items can be increased with one call.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将 Count-Min Sketch 中指定元素的计数增加指定增量</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="itemIncrement">One item-increment tuple.
    /// <para>(元素, 增量值) 的键值元组</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The increment minimum count value
    /// <para>更新后的最小计数值（min-count）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.incrby">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.incrby">命令文档</see></para>
    /// </remarks>
    public IncrByResult CMS_IncrBy(
        SharpValue key,
        (SharpValue item, long increment) itemIncrement,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_IncrBy(
            key: key,
            itemIncrement: itemIncrement) | flags, cancellationToken);
    }

    /// <summary>
    /// Increases the count of item by increment. Multiple items can be increased with one call.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将 Count-Min Sketch 中指定元素的计数增加指定增量</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="itemIncrement">One item-increment pairs.
    /// <para>元素-增量值 的键值对</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The increment minimum count value
    /// <para>更新后的最小计数值（min-count）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.incrby">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.incrby">命令文档</see></para>
    /// </remarks>
    public IncrByResult CMS_IncrBy(
        SharpValue key,
        KeyValuePair<SharpValue, long> itemIncrement,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_IncrBy(
            key: key,
            itemIncrement: itemIncrement) | flags, cancellationToken);
    }

    /// <summary>
    /// Increases the count of item by increment. Multiple items can be increased with one call.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将 Count-Min Sketch 中指定元素的计数增加指定增量</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="itemIncrements">One or more item-increment tuple. Each item's counter is increased by its corresponding increment.
    /// <para>一个或多个增量元组。每项的元素计数器将增加其对应的增量。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of integer replies representing updated min-counts of each of the provided items in the sketch.
    /// <para>指定元素递增后的最小计数值数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.incrby">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.incrby">命令文档</see></para>
    /// </remarks>
    public IncrByResult[] CMS_IncrBy(
        SharpValue key,
        ICollection<(SharpValue item, long increment)> itemIncrements,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_IncrBy(
            key: key,
            itemIncrements: itemIncrements) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Increases the count of item by increment. Multiple items can be increased with one call.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将 Count-Min Sketch 中指定元素的计数增加指定增量</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="itemIncrements">One or more item-increment pairs. Each item's counter is increased by its corresponding increment.
    /// <para>一个或多个增量对。每项的元素计数器将增加其对应的增量。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of integer replies representing updated min-counts of each of the provided items in the sketch.
    /// <para>指定元素递增后的最小计数值数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.incrby">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.incrby">命令文档</see></para>
    /// </remarks>
    public IncrByResult[] CMS_IncrBy(
        SharpValue key,
        ICollection<KeyValuePair<SharpValue, long>> itemIncrements,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_IncrBy(
            key: key,
            itemIncrements: itemIncrements) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Increases the count of item by increment. Multiple items can be increased with one call.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将 Count-Min Sketch 中指定元素的计数增加指定增量</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="itemIncrements">One or more item-increment tuple. Each item's counter is increased by its corresponding increment.
    /// <para>一个或多个增量元组。每项的元素计数器将增加其对应的增量。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of integer replies representing updated min-counts of each of the provided items in the sketch.
    /// <para>指定元素递增后的最小计数值数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.incrby">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.incrby">命令文档</see></para>
    /// </remarks>
    public IncrByResult[] CMS_IncrBy(
        SharpValue key,
        ReadOnlySpan<(SharpValue item, long increment)> itemIncrements,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_IncrBy(
            key: key,
            itemIncrements: itemIncrements) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Increases the count of item by increment. Multiple items can be increased with one call.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将 Count-Min Sketch 中指定元素的计数增加指定增量</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="itemIncrements">One or more item-increment pairs. Each item's counter is increased by its corresponding increment.
    /// <para>一个或多个增量对。每项的元素计数器将增加其对应的增量。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of integer replies representing updated min-counts of each of the provided items in the sketch.
    /// <para>指定元素递增后的最小计数值数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.incrby">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.incrby">命令文档</see></para>
    /// </remarks>
    public IncrByResult[] CMS_IncrBy(
        SharpValue key,
        ReadOnlySpan<KeyValuePair<SharpValue, long>> itemIncrements,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_IncrBy(
            key: key,
            itemIncrements: itemIncrements) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Returns width, depth and total count of the sketch.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>返回 Count-Min Sketch 的元数据信息（宽度、深度、总计数）</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.
    /// <para>要获得元数据信息的sketch key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.info">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.info">命令文档</see></para>
    /// </remarks>
    public SketchInfo CMS_Info(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Info(key) | flags, cancellationToken);
    }

    /// <summary>
    /// nitializes a Count-Min Sketch to dimensions specified by user.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>使用用户指定的宽度（width）和深度（depth）创建一个 Count-Min Sketch</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.
    /// <para>要创建的Count-Min Sketch key</para>
    /// </param>
    /// <param name="width">Number of counters in each array. Reduces the error size.
    /// <para>每个计数器数组的长度。width 越大，误差越小</para>
    /// </param>
    /// <param name="depth">Number of counter-arrays. Reduces the probability for an error of a certain size (percentage of total count).
    /// <para>计数器数组的数量。depth 越大，误差概率越低</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if executed correctly.
    /// <para>创建成功返回true, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.initbydim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.initbydim">命令文档</see></para>
    /// </remarks>
    public bool CMS_InitByDim(
        SharpValue key,
        long width,
        long depth,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_InitByDim(
            key: key,
            width: width,
            depth: depth) | flags, cancellationToken);
    }

    /// <summary>
    /// Initializes a Count-Min Sketch to accommodate requested tolerances.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>根据目标误差率（error）和误差概率（probability）初始化一个 Count-Min Sketch</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.
    /// <para>要创建的Count-Min Sketch key</para>
    /// </param>
    /// <param name="error">Estimate size of error. The error is a percent of total counted items. This effects the width of the sketch.
    /// <para>误差估计大小，占总计数的百分比。影响 sketch 的宽度（width）</para>
    /// </param>
    /// <param name="probability">The desired probability for inflated count. This should be a decimal value between 0 and 1. This effects the depth of the sketch. For example, for a desired false positive rate of 0.1% (1 in 1000), error_rate should be set to 0.001. The closer this number is to zero, the greater the memory consumption per item and the more CPU usage per operation.
    /// <para>误差概率（期望的误判概率），介于 0 和 1 之间的十进制值。影响 sketch 的深度（depth）。例如，期望误判率为 0.1%（千分之一）时，设置为 0.001。该值越接近 0，每项内存占用和 CPU 开销越高</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if executed correctly.
    /// <para>创建成功返回true, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.initbyprob">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.initbyprob">命令文档</see></para>
    /// </remarks>
    public bool CMS_InitByProb(
        SharpValue key,
        double error,
        double probability,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_InitByProb(
            key: key,
            error: error,
            probability: probability) | flags, cancellationToken);
    }

    /// <summary>
    /// Merges several sketches into one sketch. All sketches must have identical width and depth. Weights can be used to multiply certain sketches. Default weight is 1.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将一个 Count-Min Sketch 合并到一个目标 sketch 中</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="destination">The name of the destination sketch. Must be initialized
    /// <para>目标 sketch 的键名，必须已初始化 (已创建)</para>
    /// </param>
    /// <param name="source">Names of source sketches to be merged.
    /// <para>待合并的源Count-Min Sketch key</para>
    /// </param>
    /// <param name="weight">Multiple of each sketch. Default is 1.
    /// <para>源 sketch 的乘数权重，默认值为 1</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if merged successfully
    /// <para>True合并成功, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.merge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.merge">命令文档</see></para>
    /// </remarks>
    public bool CMS_Merge(
        SharpValue destination,
        SharpValue source,
        long? weight = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Merge(
            destination: destination,
            source: source,
            weight: weight) | flags, cancellationToken);
    }

    /// <summary>
    /// Merges several sketches into one sketch. All sketches must have identical width and depth. Weights can be used to multiply certain sketches. Default weight is 1.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将一个 Count-Min Sketch 合并到一个目标 sketch 中</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="destination">The name of the destination sketch. Must be initialized
    /// <para>目标 sketch 的键名，必须已初始化 (已创建)</para>
    /// </param>
    /// <param name="sources">Names of source sketches to be merged.
    /// <para>待合并的源Count-Min Sketch key, 支持多个</para>
    /// </param>
    /// <param name="weights">Multiple of each sketch. Default is 1.
    /// <para>每个源 sketch 的乘数权重，默认值为 1</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if merged successfully
    /// <para>True合并成功, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.merge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.merge">命令文档</see></para>
    /// </remarks>
    public bool CMS_Merge(
        SharpValue destination,
        SharpValueSpan sources,
#if MODERN_NET
        long[]? weights = null,
#else
        long[] weights = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Merge(
            destination: destination,
            sourceSpan: sources,
            weights: weights) | flags, cancellationToken);
    }

    /// <summary>
    /// Merges several sketches into one sketch. All sketches must have identical width and depth. Weights can be used to multiply certain sketches. Default weight is 1.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将一个 Count-Min Sketch 合并到一个目标 sketch 中</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="destination">The name of the destination sketch. Must be initialized
    /// <para>目标 sketch 的键名，必须已初始化 (已创建)</para>
    /// </param>
    /// <param name="sourceWeights">The source sketch Key-weight tuple to be merged
    /// <para>要合并的 Count-Min Sketch key-weight 元组集合, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if merged successfully
    /// <para>True合并成功, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.merge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.merge">命令文档</see></para>
    /// </remarks>
    public bool CMS_Merge(
        SharpValue destination,
        ICollection<(SharpValue source, long weight)> sourceWeights,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Merge(
            destination: destination,
            sourceWeights: sourceWeights) | flags, cancellationToken);
    }

    /// <summary>
    /// Merges several sketches into one sketch. All sketches must have identical width and depth. Weights can be used to multiply certain sketches. Default weight is 1.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将一个 Count-Min Sketch 合并到一个目标 sketch 中</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="destination">The name of the destination sketch. Must be initialized
    /// <para>目标 sketch 的键名，必须已初始化 (已创建)</para>
    /// </param>
    /// <param name="sourceWeights">The source sketch Key-weight tuple to be merged
    /// <para>要合并的 Count-Min Sketch key-weight 元组集合, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if merged successfully
    /// <para>True合并成功, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.merge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.merge">命令文档</see></para>
    /// </remarks>
    public bool CMS_Merge(
        SharpValue destination,
        ReadOnlySpan<(SharpValue source, long weight)> sourceWeights,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Merge(
            destination: destination,
            sourceWeights: sourceWeights) | flags, cancellationToken);
    }

    /// <summary>
    /// Merges several sketches into one sketch. All sketches must have identical width and depth. Weights can be used to multiply certain sketches. Default weight is 1.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将一个 Count-Min Sketch 合并到一个目标 sketch 中</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="destination">The name of the destination sketch. Must be initialized
    /// <para>目标 sketch 的键名，必须已初始化 (已创建)</para>
    /// </param>
    /// <param name="sourceWeights">The source sketch Key-weight pairs to be merged
    /// <para>要合并的 Count-Min Sketch key-weight 键值集合, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if merged successfully
    /// <para>True合并成功, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.merge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.merge">命令文档</see></para>
    /// </remarks>
    public bool CMS_Merge(
        SharpValue destination,
        ICollection<KeyValuePair<SharpValue, long>> sourceWeights,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Merge(
            destination: destination,
            sourceWeights: sourceWeights) | flags, cancellationToken);
    }

    /// <summary>
    /// Merges several sketches into one sketch. All sketches must have identical width and depth. Weights can be used to multiply certain sketches. Default weight is 1.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>将一个 Count-Min Sketch 合并到一个目标 sketch 中</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="destination">The name of the destination sketch. Must be initialized
    /// <para>目标 sketch 的键名，必须已初始化 (已创建)</para>
    /// </param>
    /// <param name="sourceWeights">The source sketch Key-weight pairs to be merged
    /// <para>要合并的 Count-Min Sketch key-weight 键值集合, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if merged successfully
    /// <para>True合并成功, 否则引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.merge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.merge">命令文档</see></para>
    /// </remarks>
    public bool CMS_Merge(
        SharpValue destination,
        ReadOnlySpan<KeyValuePair<SharpValue, long>> sourceWeights,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Merge(
            destination: destination,
            sourceWeights: sourceWeights) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the count for one item in a sketch.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>返回 Count-Min Sketch 中一个元素的计数值</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="item">One item for which to return the count.
    /// <para>要查询计数的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>integer replies representing the min-counts of each of the provided items in the sketch.
    /// <para>一个元素的计数值（min-count）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.query">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.query">命令文档</see></para>
    /// </remarks>
    public long CMS_Query(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Query(
            key: key,
            item: item) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the count for one item in a sketch.
    /// <para>Available in: Bloom 2.0.0</para>
    /// <para>返回 Count-Min Sketch 中一个元素的计数值</para>
    /// <para>支持此命令的版本: Bloom 2.0.0</para>
    /// </summary>
    /// <param name="key">The name of the sketch.</param>
    /// <param name="items">One or more items for which to return the count.
    /// <para>要查询计数的元素, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>integer replies representing the min-counts of each of the provided items in the sketch.
    /// <para>一个元素的计数值（min-count）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cms.query">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cms.query">命令文档</see></para>
    /// </remarks>
    public long[] CMS_Query(SharpValue key, SharpValueSpan items, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(CountMinSketchCommands.CMS_Query(
            key: key,
            items: items) | flags, cancellationToken)!;
    }
}
