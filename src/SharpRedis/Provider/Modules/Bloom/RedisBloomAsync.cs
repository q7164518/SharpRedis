using SharpRedis.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Modules;

public sealed partial class RedisBloom
{
    /// <summary>
    /// Adds an item to a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>添加一个元素到过布隆滤器中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter to add the item to.
    /// <para>布隆过滤器的Key</para>
    /// </param>
    /// <param name="item">Is an item to add.
    /// <para>要添加的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True for successfully adding an item, or false if there's a probability that the item was already added to the filter.
    /// <para>true表示添加成功, false表示该元素可能已经在过滤器中存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.add">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.add">命令文档</see></para>
    /// </remarks>
    public Task<bool> BF_AddAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Add(key, item) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the cardinality of a Bloom filter - number of items that were added to a Bloom filter and detected as unique (items that caused at least one bit to be set in at least one sub-filter)
    /// <para>Available in: Bloom 2.4.4</para>
    /// <para>返回 Bloom Filter 中已添加元素的估计数量（基数）</para>
    /// <para>支持此命令的版本: Bloom 2.4.4</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter.
    /// <para>布隆过滤器的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of items detected as unique that were added to the Bloom filter (items that caused at least one bit to be set in at least one sub-filter), or 0 when the given key does not exist.
    /// <para>元素的估计数量（基数）, 如果Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.card">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.card">命令文档</see></para>
    /// </remarks>
    public Task<long> BF_CardAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Card(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Determines whether a given item was added to a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>检查单个元素是否可能存在于 Bloom Filter 中。</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter.
    /// <para>布隆过滤器的Key</para>
    /// </param>
    /// <param name="item">Is an item to check.
    /// <para>要检查的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>where a value of true means the item was probably already added to the filter. A value of false means the item was definitely not added, key does not exist, or key contains a value of the wrong type.
    /// <para>true: 表示该元素可能已经存在, false: 表示该元素一定不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.exists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.exists">命令文档</see></para>
    /// </remarks>
    public Task<bool> BF_ExistsAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Exists(key, item) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns information about a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>返回指定 Bloom Filter 的配置信息和统计信息</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter.
    /// <para>要返回信息的布隆过滤器的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.info">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.info">命令文档</see></para>
    /// </remarks>
    public Task<BloomInfo> BF_InfoAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Info(key, null) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns information about a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>返回指定 Bloom Filter 的配置信息和统计信息</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter.
    /// <para>要返回信息的布隆过滤器的Key</para>
    /// </param>
    /// <param name="type">Specifies the information type returned by the BF.INFO command.
    /// <para>指定 BF.INFO 命令返回的布隆过滤器统计信息类型。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>integer representing the value of the requested property
    /// <para>指定属性的数值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.info">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.info">命令文档</see></para>
    /// </remarks>
    public Task<long?> BF_InfoAsync(SharpValue key, InfoType type, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Info(key, type) | flags, cancellationToken);
    }

    /// <summary>
    /// Creates an empty Bloom filter with a single sub-filter for the initial specified capacity and with an upper bound error_rate.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>创建一个新的布隆过滤器，指定预期容量和可接受的假阳性错误率。</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for the Bloom filter to be created.
    /// <para>要创建的布隆过滤器Key</para>
    /// </param>
    /// <param name="errorRate">The desired probability for false positives. The rate is a decimal value between 0 and 1. For example, for a desired false positive rate of 0.1% (1 in 1000), error_rate should be set to 0.001.
    /// <para>期望的假阳性错误率（范围 0 - 1）例如，如果期望的误报率为 0.1%（千分之一），则 error_rate 应设置为 0.001</para>
    /// </param>
    /// <param name="capacity">The number of entries intended to be added to the filter. If your filter allows scaling, performance will begin to degrade after adding more items than this number.
    /// <para>预期插入的元素数量。如果您的筛选器支持扩展，则添加的条目超过此数量后，性能将开始下降</para>
    /// </param>
    /// <param name="expansion">When capacity is reached, an additional sub-filter is created. The size of the new sub-filter is the size of the last sub-filter multiplied by expansion, specified as a positive integer.
    /// <para>扩容因子，满时新过滤器容量倍数（默认 2）</para>
    /// </param>
    /// <param name="nonscaling">Prevents the filter from creating additional sub-filters if initial capacity is reached. Non-scaling filters requires slightly less memory than their scaling counterparts. The filter returns an error when capacity is reached.
    /// <para>禁止自动扩容，超过 capacity 后报错</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.reserve">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.reserve">命令文档</see></para>
    /// </remarks>
    public Task<bool> BF_ReserveAsync(
        SharpValue key,
        double errorRate,
        long capacity,
        uint? expansion = null,
        bool nonscaling = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Reserve(
            key: key,
            errorRate: errorRate,
            capacity: capacity,
            expansion: expansion,
            nonscaling: nonscaling) | flags, cancellationToken);
    }

    /// <summary>
    /// Creates a new Bloom filter if the key does not exist using the specified error rate, capacity, and expansion, then adds all specified items to the Bloom Filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将单个元素添加到 Bloom Filter 中，支持在 key 不存在时自动创建过滤器</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter to add items to.
    /// <para>布隆过滤器的Key</para>
    /// </param>
    /// <param name="item">One item to add.
    /// <para>要添加到布隆过滤器的元素</para>
    /// </param>
    /// <param name="capacity">Specifies the desired capacity for the filter to be created. This parameter is ignored if the filter already exists. If the filter is automatically created and this parameter is absent, then the module-level capacity is used.
    /// <para>预期插入的元素数量</para>
    /// </param>
    /// <param name="error">Specifies the error ratio of the newly created filter if it does not yet exist.
    /// <para>期望的假阳性错误率</para>
    /// </param>
    /// <param name="expansion">When capacity is reached, an additional sub-filter is created. The size of the new sub-filter is the size of the last sub-filter multiplied by expansion, specified as a positive integer.
    /// <para>扩容因子，当容量满时新过滤器容量倍数</para>
    /// </param>
    /// <param name="nocreate">Indicates that the filter should not be created if it does not already exist
    /// <para>若 key 不存在，不自动创建，返回错误</para>
    /// </param>
    /// <param name="nonscaling">Prevents the filter from creating additional sub-filters if initial capacity is reached. Non-scaling filters require slightly less memory than their scaling counterparts. The filter returns an error when capacity is reached.
    /// <para>禁止自动扩容，超过容量后返回错误</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.insert">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.insert">命令文档</see></para>
    /// </remarks>
    public Task<AddStatus> BF_InsertAsync(
        SharpValue key,
        SharpValue item,
        long? capacity = null,
        double? error = null,
        uint? expansion = null,
        bool nocreate = false,
        bool nonscaling = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Insert(
            key: key,
            item: item,
            capacity: capacity,
            error: error,
            expansion: expansion,
            nocreate: nocreate,
            nonscaling: nonscaling) | flags, cancellationToken);
    }

    /// <summary>
    /// Creates a new Bloom filter if the key does not exist using the specified error rate, capacity, and expansion, then adds all specified items to the Bloom Filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将多个元素添加到 Bloom Filter 中，支持在 key 不存在时自动创建过滤器</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter to add items to.
    /// <para>布隆过滤器的Key</para>
    /// </param>
    /// <param name="items">One or more items to add.
    /// <para>要添加到布隆过滤器的元素, 一个或多个</para>
    /// </param>
    /// <param name="capacity">Specifies the desired capacity for the filter to be created. This parameter is ignored if the filter already exists. If the filter is automatically created and this parameter is absent, then the module-level capacity is used.
    /// <para>预期插入的元素数量</para>
    /// </param>
    /// <param name="error">Specifies the error ratio of the newly created filter if it does not yet exist.
    /// <para>期望的假阳性错误率</para>
    /// </param>
    /// <param name="expansion">When capacity is reached, an additional sub-filter is created. The size of the new sub-filter is the size of the last sub-filter multiplied by expansion, specified as a positive integer.
    /// <para>扩容因子，当容量满时新过滤器容量倍数</para>
    /// </param>
    /// <param name="nocreate">Indicates that the filter should not be created if it does not already exist
    /// <para>若 key 不存在，不自动创建，返回错误</para>
    /// </param>
    /// <param name="nonscaling">Prevents the filter from creating additional sub-filters if initial capacity is reached. Non-scaling filters require slightly less memory than their scaling counterparts. The filter returns an error when capacity is reached.
    /// <para>禁止自动扩容，超过容量后返回错误</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.insert">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.insert">命令文档</see></para>
    /// </remarks>
    public Task<AddStatus[]> BF_InsertAsync(
        SharpValue key,
        SharpValueSpan items,
        long? capacity = null,
        double? error = null,
        uint? expansion = null,
        bool nocreate = false,
        bool nonscaling = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_Insert(
            key: key,
            items: items,
            capacity: capacity,
            error: error,
            expansion: expansion,
            nocreate: nocreate,
            nonscaling: nonscaling) | flags, cancellationToken);
    }

    /// <summary>
    /// Begins an incremental save of the Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>开始或继续对 Bloom Filter 进行增量式序列化，用于备份或迁移</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter to save.
    /// <para>要操作的布隆过滤器Key</para>
    /// </param>
    /// <param name="iterator">Iterator value; either 0 or the iterator from a previous invocation of this command
    /// <para>迭代器值，首次调用传 0，后续传上次返回值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.scandump">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.scandump">命令文档</see></para>
    /// </remarks>
    public Task<ScanDumpResult> BF_ScanDumpAsync(
        SharpValue key,
        long iterator,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_ScanDump(
            key: key,
            iterator: iterator) | flags | CommandFlags.ReturnBytes, cancellationToken);
    }

    /// <summary>
    /// Restores a Bloom filter previously saved using BF.SCANDUMP.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>从 BF.SCANDUMP 生成的数据块中恢复 Bloom Filter 的状态，用于增量式恢复或迁移</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter to restore.
    /// <para>用于恢复数据的布隆过滤器Key</para>
    /// </param>
    /// <param name="iterator">Iterator value associated with data (returned by BF.SCANDUMP)
    /// <para>数据块对应的迭代器值</para>
    /// </param>
    /// <param name="data">Current data chunk (returned by BF.SCANDUMP)
    /// <para>BF.SCANDUMP 返回的序列化数据块</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.loadchunk">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.loadchunk">命令文档</see></para>
    /// </remarks>
    public Task<bool> BF_LoadChunkAsync(
        SharpValue key,
        long iterator,
        byte[] data,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_LoadChunk(
            key: key,
            iterator: iterator,
            data: data) | flags | CommandFlags.ReturnBytes, cancellationToken);
    }

    /// <summary>
    /// Adds one or more items to a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将多个元素批量添加到 Bloom Filter 中。若 key 不存在则使用默认参数自动创建</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter to add the items to.
    /// <para>要添加元素的布隆过滤器Key</para>
    /// </param>
    /// <param name="item">One or more items to add.
    /// <para>要添加的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.madd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.madd">命令文档</see></para>
    /// </remarks>
    public Task<AddStatus> BF_MAddAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_MAdd(
            key: key,
            item: item) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds one or more items to a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将多个元素批量添加到 Bloom Filter 中。若 key 不存在则使用默认参数自动创建</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter to add the items to.
    /// <para>要添加元素的布隆过滤器Key</para>
    /// </param>
    /// <param name="items">One or more items to add.
    /// <para>要添加的元素, 一个或多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.madd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.madd">命令文档</see></para>
    /// </remarks>
    public Task<AddStatus[]> BF_MAddAsync(SharpValue key, SharpValueSpan items, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_MAdd(
            key: key,
            items: items) | flags, cancellationToken);
    }

    /// <summary>
    /// Determines whether one item were added to a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>检查一个元素是否可能存在于Bloom Filter 中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter.
    /// <para>布隆过滤器Key</para>
    /// </param>
    /// <param name="item">One item to check.
    /// <para>要检查的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A value of 1 means the item was probably already added to the filter. A value of 0 means the item was definitely not added, key does not exist, or key contains a value of the wrong type.
    /// <para>1 -> True, 0 -> False</para>
    /// <para>返回True表示此元素可能存在, False表示一定不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.mexists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.mexists">命令文档</see></para>
    /// </remarks>
    public Task<bool> BF_MExistsAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_MExists(
            key: key,
            item: item) | flags, cancellationToken);
    }

    /// <summary>
    /// Determines whether one or more items were added to a Bloom filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>批量检查多个元素是否可能存在于 Bloom Filter 中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a Bloom filter.
    /// <para>布隆过滤器Key</para>
    /// </param>
    /// <param name="items">One or more items to check.
    /// <para>要检查的元素，可指定多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of integers, where each element corresponds to an item in the request. A value of 1 means the item was probably already added to the filter. A value of 0 means the item was definitely not added, key does not exist, or key contains a value of the wrong type.
    /// <para>1 -> True, 0 -> False</para>
    /// <para>和传入元素集合位置对应的bool数组. 返回True表示此元素可能存在, False表示一定不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/bf.mexists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bf.mexists">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> BF_MExistsAsync(SharpValue key, SharpValueSpan items, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(BloomCommands.BF_MExists(
            key: key,
            items: items) | flags, cancellationToken);
    }
}
