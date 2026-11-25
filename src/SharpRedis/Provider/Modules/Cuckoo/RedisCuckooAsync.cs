using SharpRedis.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Modules;

public sealed partial class RedisCuckoo
{
    /// <summary>
    /// Adds an item to the cuckoo filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>添加一个元素到布谷鸟过滤器中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to add the item to. If key does not exist - a new cuckoo filter is created.
    /// <para>布谷鸟过滤器Key, 如果Key不存在, 将自动创建一个布谷鸟过滤器</para>
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
    /// <returns>True for successfully adding an item to the filter.
    /// <para>True添加成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.add">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.add">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_AddAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Add(key, item) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an item to a cuckoo filter if the item does not exist.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>仅在元素不存在于 Cuckoo Filter 时添加该元素（Add if Not eXists）</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to add items to. If key does not exist - a new cuckoo filter is created.
    /// <para>布谷鸟过滤器Key。若 key 不存在则自动创建</para>
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
    /// <returns>1 for successfully adding an item to the filter or 0 if the item's fingerprint already exists in the filter.
    /// <para>1：元素之前不存在，添加成功. 0：元素已存在（或假阳性误判为存在），未添加</para>
    /// <para>1 -> true, 0 -> false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.addnx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.addnx">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_AddNxAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_AddNx(key, item) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns an estimation of the number of times a given item was added to a cuckoo filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>返回指定元素在 Cuckoo Filter 中的估计计数（插入次数）</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter.
    /// <para>布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="item">Is an item to check.
    /// <para>要计数的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>where a positive value is an estimation of the number of times item was added to the filter. An overestimation is possible, but not an underestimation. 0 means that key does not exist or that item had not been added to the filter.
    /// <para>元素的估计插入次数。对于未插入的元素，返回 0 或假阳性误判的小正数</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.count">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.count">命令文档</see></para>
    /// </remarks>
    public Task<long> CF_CountAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Count(key, item) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes an item once from the filter. If the item exists only once, it will be removed from the filter. If the item was added multiple times, it will still be present.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>从 Cuckoo Filter 中删除一个元素. 如果元素支持存在一次, 则删除, 如果元素添加了多次, 元素还会存在于过滤器中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter.
    /// <para>要删除元素的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="item">Is an item to delete.
    /// <para>要删除的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>1 for successfully deleting an item, or 0 if no such item was found in the filter.
    /// <para>1：删除成功（找到了该元素并移除一个计数） 0：元素不存在于过滤器中（或假阴性）</para>
    /// <para>1 -> true. 0 -> false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.del">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.del">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_DelAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Del(key, item) | flags, cancellationToken);
    }

    /// <summary>
    /// Determines whether a given item was added to a cuckoo filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>检查单个元素是否可能存在于 Cuckoo Filter 中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter.
    /// <para>要检查的布谷鸟过滤器Key</para>
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
    /// <returns>where a value of 1 means the item was probably already added to the filter. A value of 0 means the item was definitely not added, key does not exist, or key contains a value of the wrong type.
    /// <para>1：元素可能存在于过滤器中（存在假阳性可能） 0：元素一定不存在于过滤器中（或假阴性，概率极低）</para>
    /// <para>1 -> true. 0 -> false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.exists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.exists">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_ExistsAsync(SharpValue key, SharpValue item, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Exists(key, item) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds one or more items to a cuckoo filter, allowing the filter to be created with a custom capacity if it does not exist yet.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将一个元素添加到 Cuckoo Filter 中，如果过滤器不存在则自动创建</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to add items to. If key does not exist - a new cuckoo filter is created.
    /// <para>要插入元素的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="item">One item to add.
    /// <para>要插入的元素</para>
    /// </param>
    /// <param name="capacity">Specifies the desired capacity of the new filter, if this filter does not exist yet. If the filter already exists, then this parameter is ignored.
    /// <para>新过滤器的期望容量（仅在过滤器不存在时生效）</para>
    /// </param>
    /// <param name="nocreate">If specified, prevents automatic filter creation if the filter does not exist (Instead, an error is returned). This option is mutually exclusive with CAPACITY.
    /// <para>禁止自动创建过滤器，如果键不存在则返回错误. 此选项和 CAPACITY 互斥</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>1 when the item is added successfully, or -1 when the item cannot be added because the filter is full.
    /// <para>1：元素成功插入. -1：无法插入，因为过滤器已满（需要扩展容量或允许自动扩展）</para>
    /// <para>1 -> true, -1 -> false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.insert">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.insert">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_InsertAsync(
        SharpValue key,
        SharpValue item,
        int? capacity = null,
        bool nocreate = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Insert(
            key: key,
            item: item,
            capacity: capacity,
            nocreate: nocreate) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds one or more items to a cuckoo filter, allowing the filter to be created with a custom capacity if it does not exist yet.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将一个或多个元素添加到 Cuckoo Filter 中，如果过滤器不存在则自动创建</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to add items to. If key does not exist - a new cuckoo filter is created.
    /// <para>要插入元素的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="items">One or more items to add.
    /// <para>要插入的元素, 支持多个</para>
    /// </param>
    /// <param name="capacity">Specifies the desired capacity of the new filter, if this filter does not exist yet. If the filter already exists, then this parameter is ignored.
    /// <para>新过滤器的期望容量（仅在过滤器不存在时生效）</para>
    /// </param>
    /// <param name="nocreate">If specified, prevents automatic filter creation if the filter does not exist (Instead, an error is returned). This option is mutually exclusive with CAPACITY.
    /// <para>禁止自动创建过滤器，如果键不存在则返回错误. 此选项和 CAPACITY 互斥</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>where each element is an integer reply: 1 when the item is added successfully, or -1 when the item cannot be added because the filter is full.
    /// <para>跟添加元素顺序保持一致的数组. 1：元素成功插入. -1：无法插入，因为过滤器已满（需要扩展容量或允许自动扩展）</para>
    /// <para>1 -> true, -1 -> false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.insert">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.insert">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> CF_InsertAsync(
        SharpValue key,
        SharpValueSpan items,
        int? capacity = null,
        bool nocreate = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Insert(
            key: key,
            items: items,
            capacity: capacity,
            nocreate: nocreate) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds one or more items to a cuckoo filter if they did not exist previously, allowing the filter to be created with a custom capacity if it does not exist yet.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将一个元素添加到 Cuckoo Filter 中，仅当元素之前不存在时才插入。如果过滤器不存在则自动创建</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to add items to. If key does not exist - a new cuckoo filter is created.
    /// <para>要插入元素的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="item">One item to add.
    /// <para>要插入的元素</para>
    /// </param>
    /// <param name="capacity">Specifies the desired capacity of the new filter, if this filter does not exist yet. If the filter already exists, then this parameter is ignored.
    /// <para>新过滤器的期望容量（仅在过滤器不存在时生效）</para>
    /// </param>
    /// <param name="nocreate">If specified, prevents automatic filter creation if the filter does not exist (Instead, an error is returned). This option is mutually exclusive with CAPACITY.
    /// <para>禁止自动创建过滤器，如果键不存在则返回错误. 此选项和 CAPACITY 互斥</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.insertnx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.insertnx">命令文档</see></para>
    /// </remarks>
    public Task<InsertNxStatus> CF_InsertNxAsync(
        SharpValue key,
        SharpValue item,
        int? capacity = null,
        bool nocreate = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_InsertNx(
            key: key,
            item: item,
            capacity: capacity,
            nocreate: nocreate) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds one or more items to a cuckoo filter if they did not exist previously, allowing the filter to be created with a custom capacity if it does not exist yet.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>将一个或多个元素添加到 Cuckoo Filter 中，仅当元素之前不存在时才插入。如果过滤器不存在则自动创建</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to add items to. If key does not exist - a new cuckoo filter is created.
    /// <para>要插入元素的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="items">One or more items to add.
    /// <para>要插入的元素, 支持多个</para>
    /// </param>
    /// <param name="capacity">Specifies the desired capacity of the new filter, if this filter does not exist yet. If the filter already exists, then this parameter is ignored.
    /// <para>新过滤器的期望容量（仅在过滤器不存在时生效）</para>
    /// </param>
    /// <param name="nocreate">If specified, prevents automatic filter creation if the filter does not exist (Instead, an error is returned). This option is mutually exclusive with CAPACITY.
    /// <para>禁止自动创建过滤器，如果键不存在则返回错误. 此选项和 CAPACITY 互斥</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.insertnx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.insertnx">命令文档</see></para>
    /// </remarks>
    public Task<InsertNxStatus[]> CF_InsertNxAsync(
        SharpValue key,
        SharpValueSpan items,
        int? capacity = null,
        bool nocreate = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_InsertNx(
            key: key,
            items: items,
            capacity: capacity,
            nocreate: nocreate) | flags, cancellationToken);
    }

    /// <summary>
    /// Restores a cuckoo filter previously saved using CF.SCANDUMP.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>从之前使用 CF.SCANDUMP 保存的数据块中恢复 Cuckoo Filter</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to restore.
    /// <para>要恢复为布谷鸟过滤器的Key</para>
    /// </param>
    /// <param name="iterator">Iterator value associated with data (returned by CF.SCANDUMP)
    /// <para>迭代器值（由 CF.SCANDUMP 返回）</para>
    /// </param>
    /// <param name="data">Current data chunk (returned by CF.SCANDUMP)
    /// <para>当前数据块（由 CF.SCANDUMP 返回）</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>true on success
    /// <para>true恢复成功</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.loadchunk">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.loadchunk">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_LoadChunkAsync(
        SharpValue key,
        long iterator,
        byte[] data,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_LoadChunk(
            key: key,
            iterator: iterator,
            data: data) | flags, cancellationToken);
    }

    /// <summary>
    /// Begins an incremental save of the cuckoo filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>开始增量保存 Cuckoo Filter，适用于大型过滤器无法通过标准 DUMP/RESTORE 迁移的场景</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter to save.
    /// <para>要导出保存数据的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="iterator">Iterator value; either 0 or the iterator from a previous invocation of this command
    /// <para>迭代器值，首次调用传 0，后续调用传上次返回的迭代器值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.scandump">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.scandump">命令文档</see></para>
    /// </remarks>
    public Task<ScanDumpResult> CF_ScanDumpAsync(
        SharpValue key,
        long iterator,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_ScanDump(
            key: key,
            iterator: iterator) | flags | CommandFlags.ReturnBytes, cancellationToken);
    }

    /// <summary>
    /// Determines whether one or more items were added to a cuckoo filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>检查一个元素是否存在于 Cuckoo Filter 中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter.
    /// <para>要检查的布谷鸟过滤器Key</para>
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
    /// <para>1：元素很可能已被添加到过滤器中（存在假阳性可能） 0：键不存在，或元素肯定未被添加到过滤器中</para>
    /// <para>1 -> true, 0 -> false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.mexists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.mexists">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_MExistsAsync(
        SharpValue key,
        SharpValue item,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_MExists(
            key: key,
            item: item) | flags, cancellationToken);
    }

    /// <summary>
    /// Determines whether one or more items were added to a cuckoo filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>检查一个或多个元素是否存在于 Cuckoo Filter 中</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter.
    /// <para>要检查的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="items">One or more items to check.
    /// <para>要检查的元素, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of integers, where each element corresponds to an item in the request. A value of 1 means the item was probably already added to the filter. A value of 0 means the item was definitely not added, key does not exist, or key contains a value of the wrong type.
    /// <para>和传入元素顺序一致的数组. 1：元素很可能已被添加到过滤器中（存在假阳性可能） 0：键不存在，或元素肯定未被添加到过滤器中</para>
    /// <para>1 -> true, 0 -> false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.mexists">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.mexists">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> CF_MExistsAsync(
        SharpValue key,
        SharpValueSpan items,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_MExists(
            key: key,
            items: items) | flags, cancellationToken);
    }

    /// <summary>
    /// Creates an empty cuckoo filter with a single sub-filter for the initial specified capacity.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>创建一个空的 Cuckoo Filter，可指定容量和高级参数</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for the cuckoo filter to be created.
    /// <para>要创建的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="capacity">Estimated capacity for the filter. Capacity is rounded to the next 2^n number.
    /// <para>过滤器的预估容量（会被向上取整到最近的 2n）</para>
    /// </param>
    /// <param name="bucketSize">Number of items in each bucket.
    /// <para>每个桶的大小（1-255，默认 2）</para>
    /// </param>
    /// <param name="maxIterations">Number of attempts to swap items between buckets before declaring filter as full and creating an additional filter.
    /// <para>最大迭代次数（1-65535，默认 20）</para>
    /// </param>
    /// <param name="expansion">When a new filter is created, its size is the size of the current filter multiplied by expansion.
    /// <para>扩展倍率（0-32768，默认 1）</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.reserve">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.reserve">命令文档</see></para>
    /// </remarks>
    public Task<bool> CF_ReserveAsync(
        SharpValue key,
        int capacity,
        int? bucketSize = null,
        int? maxIterations = null,
        int? expansion = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Reserve(
            key: key,
            capacity: capacity,
            bucketSize: bucketSize,
            maxIterations: maxIterations,
            expansion: expansion) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns information about a cuckoo filter.
    /// <para>Available in: Bloom 1.0.0</para>
    /// <para>返回 Cuckoo Filter 的详细信息和统计指标</para>
    /// <para>支持此命令的版本: Bloom 1.0.0</para>
    /// </summary>
    /// <param name="key">Is key name for a cuckoo filter.
    /// <para>要返回信息的布谷鸟过滤器Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/cf.info">Command documentation</see>
    /// <para><see href="https://redis.io/commands/cf.info">命令文档</see></para>
    /// </remarks>
    public Task<CuckooInfo> CF_InfoAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(CuckooCommands.CF_Info(key) | flags, cancellationToken);
    }
}
