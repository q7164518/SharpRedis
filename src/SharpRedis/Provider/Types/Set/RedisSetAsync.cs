using SharpRedis.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public sealed partial class RedisSet
{
    /// <summary>
    /// Add the specified member to the set stored at key
    /// <para>Available since: 1.0.0</para>
    /// <para>将一个元素添加到指定Key的集合中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="member">member</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that were added to the set, not including all the elements already present in the set.
    /// <para>成功添加到集合中的元素数量, 不包括集合中已存在的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SAdd">命令文档</see></para>
    /// </remarks>
    public Task<int> SAddAsync(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SAdd(key, member) | flags, cancellationToken);

    /// <summary>
    /// Add the specified members to the set stored at key
    /// <para>Available since: 2.4.0</para>
    /// <para>将多个元素添加到指定Key的集合中</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="members">Multiple members</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements that were added to the set, not including all the elements already present in the set.
    /// <para>成功添加到集合中的元素数量, 不包括集合中已存在的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SAdd">命令文档</see></para>
    /// </remarks>
    public Task<int> SAddAsync(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SAdd(key, members) | flags, cancellationToken);

    /// <summary>
    /// Returns the set cardinality (number of elements) of the set stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得指定Key的集合中元素数量</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The cardinality (number of elements) of the set, or 0 if the key does not exist.
    /// <para>集合中元素的数量, Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SCard">命令文档</see></para>
    /// </remarks>
    public Task<long> SCardAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SCard(key) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得第一个Key与第二个Key的差值</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SDiff">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SDiff">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SDiffAsync(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SDiff(key1, key2) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all the successive sets.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得第一个Key与后续所有Key的差值</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SDiff">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SDiff">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SDiffAsync(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SDiff(keys) | flags, cancellationToken);

    /// <summary>
    /// This command is equal to SDIFF, but instead of returning the resulting set, it is stored in destination.
    /// <para>Available since: 1.0.0</para>
    /// <para>和SDIFF命令效果一样, 只不过不返回差值, 而是把结果存储在指定的destination中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key
    /// <para>保存差值结果的Key</para>
    /// </param>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set.
    /// <para>差值元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SDiffStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SDiffStore">命令文档</see></para>
    /// </remarks>
    public Task<long> SDiffStoreAsync(SharpValue destination, SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SDiffStore(key1, key2, destination) | flags, cancellationToken);

    /// <summary>
    /// This command is equal to SDIFF, but instead of returning the resulting set, it is stored in destination.
    /// <para>Available since: 1.0.0</para>
    /// <para>和SDIFF命令效果一样, 只不过不返回差值, 而是把结果存储在指定的destination中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key
    /// <para>保存差值结果的Key</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set.
    /// <para>差值元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SDiffStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SDiffStore">命令文档</see></para>
    /// </remarks>
    public Task<long> SDiffStoreAsync(SharpValue destination, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SDiffStore(keys, destination) | flags, cancellationToken);

    /// <summary>
    /// Returns all the members of the set value stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得指定Key集合中的所有元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array with all the members of the set.
    /// <para>集合中所有元素的数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SMembers">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SMembers">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SMembersAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SMembers(key) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the intersection of all the given sets.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得第一个Key与第二个Key的交集</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SInter">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SInter">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SInterAsync(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SInter(key1, key2) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the intersection of all the given sets.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得第一个Key与后续所有Key的交集</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list with members of the resulting set.
    /// <para>差值结果数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SInter">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SInter">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SInterAsync(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SInter(keys) | flags, cancellationToken);

    /// <summary>
    /// This command is similar to <b>SINTER</b>, but instead of returning the result set, it returns just the cardinality of the result.
    /// <para>Available since: 7.0.0</para>
    /// <para>该命令和<b>SINTER</b>类似, 不过不返回交集值, 而是返回交集数量</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="limit"> When provided with the optional LIMIT argument (which defaults to 0 and means unlimited), if the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality.
    /// <para>限制最多交集元素数量, 默认0, 表示不限制</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting intersection.
    /// <para>交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SInterCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SInterCard">命令文档</see></para>
    /// </remarks>
    public Task<long> SInterCardAsync(SharpValue key1, SharpValue key2, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SInterCard(key1, key2, limit) | flags, cancellationToken);

    /// <summary>
    /// This command is similar to <b>SINTER</b>, but instead of returning the result set, it returns just the cardinality of the result.
    /// <para>Available since: 7.0.0</para>
    /// <para>该命令和<b>SINTER</b>类似, 不过不返回交集值, 而是返回交集数量</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="limit"> When provided with the optional LIMIT argument (which defaults to 0 and means unlimited), if the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality.
    /// <para>限制最多交集元素数量, 默认0, 表示不限制</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting intersection.
    /// <para>交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SInterCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SInterCard">命令文档</see></para>
    /// </remarks>
    public Task<long> SInterCardAsync(SharpValueSpan keys, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SInterCard(keys, limit) | flags, cancellationToken);

    /// <summary>
    /// This command is equal to <b>[SINTER]</b>, but instead of returning the resulting set, it is stored in destination.
    /// <para>Available since: 1.0.0</para>
    /// <para>和<b> [SINTER] </b>命令效果一样, 只不过不返回差值, 而是把结果存储在指定的destination中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key
    /// <para>保存交集结果的Key</para>
    /// </param>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the result set.
    /// <para>交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SInterStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SInterStore">命令文档</see></para>
    /// </remarks>
    public Task<long> SInterStoreAsync(SharpValue destination, SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SInterStore(key1, key2, destination) | flags, cancellationToken);

    /// <summary>
    /// This command is equal to <b>[SINTER]</b>, but instead of returning the resulting set, it is stored in destination.
    /// <para>Available since: 1.0.0</para>
    /// <para>和<b> [SINTER] </b>命令效果一样, 只不过不返回差值, 而是把结果存储在指定的destination中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key
    /// <para>保存交集结果的Key</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the result set.
    /// <para>交集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SInterStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SInterStore">命令文档</see></para>
    /// </remarks>
    public Task<long> SInterStoreAsync(SharpValue destination, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SInterStore(keys, destination) | flags, cancellationToken);

    /// <summary>
    /// Returns if member is a member of the set stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>指定Key的集合中是否存在指定的元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要判断是否存在的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns><b>False</b> if the element is not a member of the set, or when the key does not exist. <b>True</b> if the element is a member of the set.
    /// <para>Key不存在或Key中不存在该元素返回False, 否则返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SIsMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SIsMember">命令文档</see></para>
    /// </remarks>
    public Task<bool> SIsMemberAsync(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SIsMember(key, member) | flags, cancellationToken);

    /// <summary>
    /// Returns whether each member is a member of the set stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>指定Key的集合中是否存在指定的多个元素</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members">members<para>要判断是否存在的多个元素集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>For every member, True is returned if the value is a member of the set, or False if the element is not a member of the set or if key does not exist.
    /// <para>是否存在数组, 跟指定的元素顺序一致, True存在, False不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SMIsMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SMIsMember">命令文档</see></para>
    /// </remarks>
    public Task<bool[]> SMIsMemberAsync(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SMIsMember(key, members) | flags, cancellationToken);

    /// <summary>
    /// Move member from the set at source to the set at destination.
    /// <para>Available since: 1.0.0</para>
    /// <para>将一个元素从指定的源集合中移动到指定的目标集合中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="source">source key<para>源集合Key</para></param>
    /// <param name="destination">destination key<para>目标集合Key</para></param>
    /// <param name="member">member<para>要移动的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the element is moved, False if the element is not a member of source and no operation was performed.
    /// <para>如果元素被移动返回True, 如果元素在源集合中不存在返回False</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SMove">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SMove">命令文档</see></para>
    /// </remarks>
    public Task<bool> SMoveAsync(SharpValue source, SharpValue destination, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SMove(source, destination, member) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns one random member from the set value store at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>从指定Key的集合中随机返回并移除一个元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPop">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> SPopAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SPop(key, null) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns more random members from the set value store at key.
    /// <para>Available since: 3.2.0</para>
    /// <para>从指定Key的集合中随机返回并移除多个元素</para>
    /// <para>支持此命令的Redis版本, 3.2.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">count<para>要弹出的元素个数</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPop">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SPopAsync(SharpValue key, ulong count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SPop(key, count) | flags, cancellationToken);

    /// <summary>
    /// Gets one random member from the set value store at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>从指定Key的集合中随机获得一个元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SRandMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SRandMember">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> SRandMemberAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SRandMember(key, null) | flags, cancellationToken);

    /// <summary>
    /// Gets more random member from the set value store at key.
    /// <para>Available since: 2.6.0</para>
    /// <para>从指定Key的集合中随机获得多个元素</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">
    /// If the provided count argument is positive, return an array of distinct elements.
    /// <para>If called with a negative count, the behavior changes and the command is allowed to return the same element multiple times. In this case, the number of returned elements is the absolute value of the specified count.</para>
    /// <para>获得的元素数量, 如果为正数, 返回的元素不会重复, 如果为负数, 返回的元素可能会重复</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SRandMember">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SRandMember">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SRandMemberAsync(SharpValue key, long count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SRandMember(key, count) | flags, cancellationToken);

    /// <summary>
    /// Remove the specified members from the set stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>从指定Key的集合中移除一个元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member">member<para>要移除的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members that were removed from the set, not including non existing members.
    /// <para>被移除的元素数量, 不包括不存在的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SRem">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SRem">命令文档</see></para>
    /// </remarks>
    public Task<long> SRemAsync(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SRem(key, member) | flags, cancellationToken);

    /// <summary>
    /// Remove the specified members from the set stored at key.
    /// <para>Available since: 2.4.0</para>
    /// <para>从指定Key的集合中移除多个元素</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members">members<para>要移除的元素集合</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of members that were removed from the set, not including non existing members.
    /// <para>被移除的元素数量, 不包括不存在的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SRem">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SRem">命令文档</see></para>
    /// </remarks>
    public Task<long> SRemAsync(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SRem(key, members) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the union of all the given sets.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得指定Key集合的并集</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array with members of the resulting set.
    /// <para>并集元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SUnion">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SUnion">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SUnionAsync(SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SUnion(key1, key2) | flags, cancellationToken);

    /// <summary>
    /// Returns the members of the set resulting from the union of all the given sets.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得指定Key集合的并集</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array with members of the resulting set.
    /// <para>并集元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SUnion">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SUnion">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    SUnionAsync(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SUnion(keys) | flags, cancellationToken);

    /// <summary>
    /// This command is equal to <b>[SUNION]</b>, but instead of returning the resulting set, it is stored in destination.
    /// <para>Available since: 1.0.0</para>
    /// <para>该命令和 <b>[SUNION]</b> 类似, 只不过不返回并集结果, 而是将结果存储在指定的destination中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key
    /// <para>保存并集结果的Key</para>
    /// </param>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set.
    /// <para>并集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SUnionStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SUnionStore">命令文档</see></para>
    /// </remarks>
    public Task<long> SUnionStoreAsync(SharpValue destination, SharpValue key1, SharpValue key2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SUnionStore(key1, key2, destination) | flags, cancellationToken);

    /// <summary>
    /// This command is equal to <b>[SUNION]</b>, but instead of returning the resulting set, it is stored in destination.
    /// <para>Available since: 1.0.0</para>
    /// <para>该命令和 <b>[SUNION]</b> 类似, 只不过不返回并集结果, 而是将结果存储在指定的destination中</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="destination">destination key
    /// <para>保存并集结果的Key</para>
    /// </param>
    /// <param name="keys"></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting set.
    /// <para>并集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SUnionStore">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SUnionStore">命令文档</see></para>
    /// </remarks>
    public Task<long> SUnionStoreAsync(SharpValue destination, SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SUnionStore(keys, destination) | flags, cancellationToken);

    /// <summary>
    /// SSCAN iterates members of Set types.
    /// <para>Available since: 2.8.0</para>
    /// <para>遍历Set里面的元素</para>
    /// <para>支持此命令的Redis版本, 2.8.0+</para>
    /// </summary>
    /// <param name="key">Set key</param>
    /// <param name="pattern">Member selection criteria
    /// <para>筛选条件</para>
    /// </param>
    /// <param name="cursor">The cursor position to start at. default is 0
    /// <para>开始遍历的游标, 默认为0</para>
    /// </param>
    /// <param name="count">The number of items returned this time, default is 10
    /// <para>本次返回的条目个数, 默认为10个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SScan">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SScan">命令文档</see></para>
    /// </remarks>
    public Task<SetScanResult> SScanAsync(SharpValue key, SharpValue? pattern = null, ulong cursor = 0, uint count = 10, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(SetCommands.SScan(key, cursor, pattern, count) | flags, cancellationToken);

    /// <summary>
    /// Returns the cardinality of the difference between the first set and all the successive sets. This is the count-only counterpart of SDIFF: it returns just the number of elements in the difference, not the members themselves.
    /// <para>Available since: 8.10.0</para>
    /// <para>返回第一个Key与后续所有Key的差集元素数量. 这是 <b>SDIFF</b> 的仅计数版本：它只返回差集中元素的数量，而不是元素本身的数量。</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="key1">key1</param>
    /// <param name="key2">key2</param>
    /// <param name="limit">Stop counting once the cardinality reaches limit. 0 (the default) means no limit.
    /// <para>限制最多差集元素数量, 默认0, 表示不限制</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting difference.
    /// <para>差集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SDiffCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SDiffCard">命令文档</see></para>
    /// </remarks>
    public Task<long> SDiffCardAsync(SharpValue key1, SharpValue key2, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SDiffCard(key1, key2, limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the cardinality of the difference between the first set and all the successive sets. This is the count-only counterpart of SDIFF: it returns just the number of elements in the difference, not the members themselves.
    /// <para>Available since: 8.10.0</para>
    /// <para>返回第一个Key与后续所有Key的差集元素数量. 这是 <b>SDIFF</b> 的仅计数版本：它只返回差集中元素的数量，而不是元素本身的数量。</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="keys">keys array</param>
    /// <param name="limit">Stop counting once the cardinality reaches limit. 0 (the default) means no limit.
    /// <para>限制最多差集元素数量, 默认0, 表示不限制</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting difference.
    /// <para>差集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SDiffCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SDiffCard">命令文档</see></para>
    /// </remarks>
    public Task<long> SDiffCardAsync(SharpValueSpan keys, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SDiffCard(keys, limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the cardinality of the union of the given sets. This is the count-only counterpart of SUNION: it returns just the number of distinct elements in the union, not the members themselves.
    /// <para>Available since: 8.10.0</para>
    /// <para>返回第一个Key与后续所有Key的并集元素数量。这是 SUNION 的计数版本：它仅返回并集中不同元素的数量，而不是元素本身的数量。</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="key1">key1</param>
    /// <param name="key2">key2</param>
    /// <param name="approx">Return an approximate cardinality instead of an exact count.
    /// <para>是否返回近似数量而非精确计数</para>
    /// </param>
    /// <param name="limit">Stop counting once the cardinality reaches limit. 0 (the default) means no limit.
    /// <para>限制最多并集元素数量, 默认0, 表示不限制</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting union.
    /// <para>并集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SUnionCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SUnionCard">命令文档</see></para>
    /// </remarks>
    public Task<long> SUnionCardAsync(SharpValue key1, SharpValue key2, bool approx = false, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SUnionCard(key1, key2, approx, limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the cardinality of the union of the given sets. This is the count-only counterpart of SUNION: it returns just the number of distinct elements in the union, not the members themselves.
    /// <para>Available since: 8.10.0</para>
    /// <para>返回第一个Key与后续所有Key的并集元素数量。这是 SUNION 的计数版本：它仅返回并集中不同元素的数量，而不是元素本身的数量。</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="keys">keys array</param>
    /// <param name="approx">Return an approximate cardinality instead of an exact count.
    /// <para>是否返回近似数量而非精确计数</para>
    /// </param>
    /// <param name="limit">Stop counting once the cardinality reaches limit. 0 (the default) means no limit.
    /// <para>限制最多并集元素数量, 默认0, 表示不限制</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of elements in the resulting union.
    /// <para>并集元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SUnionCard">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SUnionCard">命令文档</see></para>
    /// </remarks>
    public Task<long> SUnionCardAsync(SharpValueSpan keys, bool approx = false, ulong limit = 0, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SUnionCard(keys, approx, limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Add one or more members to a set, and optionally specify their expiration time or time to live (TTL). If the key doesn't exist, Redis creates a new set.
    /// <para>Available since: 8.12.0</para>
    /// <para>将一个元素添加到指定Key的集合中, 并可选的设置失效时间. 如果Key不存在会创建</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合key</para>
    /// </param>
    /// <param name="member">member</param>
    /// <param name="expiry">The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// </param>
    /// <param name="when">Specifies the execution condition for the SADDEX command.
    /// <para>指定 SADDEX 命令添加或更新 Set 成员时的执行条件</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if no member was added, updated, or removed. This includes the case where the MNX or MXX condition was not met.
    /// <para>True if at least one member was added, updated, or removed.</para>
    /// <para>False 表示没有添加、更新或删除任何成员。这包括未满足 MNX 或 MXX 条件的情况</para>
    /// <para>如果至少添加、更新或删除了一名成员，则返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SAddEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SAddEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> SAddExAsync(
        SharpValue key,
        SharpValue member,
        SharpExpiry? expiry = null,
        ExpirationFlags expiryFlags = ExpirationFlags.EX,
        AddExWhen when = AddExWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SAddEx(
            key: key,
            when: when,
            expiry: expiry,
            expiryFlags: expiryFlags,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// Add one or more members to a set, and optionally specify their expiration time or time to live (TTL). If the key doesn't exist, Redis creates a new set.
    /// <para>Available since: 8.12.0</para>
    /// <para>将一个元素添加到指定Key的集合中, 并可选的设置失效时间. 如果Key不存在会创建</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合key</para>
    /// </param>
    /// <param name="members">The added member can support multiple options.
    /// <para>添加的成员, 支持多个</para>
    /// </param>
    /// <param name="expiry">The due date can be passed as one of the five types: TimeSpan | DateTime | DateTimeOffset | long | int
    /// <para>到期时间, 可以使用TimeSpan | DateTime | DateTimeOffset | long | int 五个类型之一</para>
    /// </param>
    /// <param name="expiryFlags">Effective time processing method, default indicates the number of seconds until expiration
    /// <para>有效时间处理方式, 默认为有效期秒数</para>
    /// </param>
    /// <param name="when">Specifies the execution condition for the SADDEX command.
    /// <para>指定 SADDEX 命令添加或更新 Set 成员时的执行条件</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>False if no member was added, updated, or removed. This includes the case where the MNX or MXX condition was not met.
    /// <para>True if at least one member was added, updated, or removed.</para>
    /// <para>False 表示没有添加、更新或删除任何成员。这包括未满足 MNX 或 MXX 条件的情况</para>
    /// <para>如果至少添加、更新或删除了一名成员，则返回True</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SAddEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SAddEx">命令文档</see></para>
    /// </remarks>
    public Task<bool> SAddExAsync(
        SharpValue key,
        SharpValueSpan members,
        SharpExpiry? expiry = null,
        ExpirationFlags expiryFlags = ExpirationFlags.EX,
        AddExWhen when = AddExWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SAddEx(
            key: key,
            when: when,
            expiry: expiry,
            expiryFlags: expiryFlags,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// Specify an expiration (TTL or time to live), in seconds, for one member of a set. Redis removes each member from the set automatically when its TTL expires.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置指定秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="seconds">The time to live, in seconds. Each specified member is removed after this many seconds.
    /// <para>剩余有效期, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SExpireAsync(
        SharpValue key,
        SharpValue member,
        long seconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpire(
            key: key,
            seconds: seconds,
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// Specify an expiration (TTL or time to live), in seconds, for one member of a set. Redis removes each member from the set automatically when its TTL expires.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置指定秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="seconds">The time to live, in seconds. Each specified member is removed after this many seconds.
    /// <para>剩余有效期, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SExpireAsync(
        SharpValue key,
        SharpValue member,
        TimeSpan seconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpire(
            key: key,
            seconds: (long)seconds.TotalSeconds,
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// Specify an expiration (TTL or time to live), in seconds, for one or more members of a set. Redis removes each member from the set automatically when its TTL expires.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置指定秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 可以指定多个</para>
    /// </param>
    /// <param name="seconds">The time to live, in seconds. Each specified member is removed after this many seconds.
    /// <para>剩余有效期, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SExpireAsync(
        SharpValue key,
        SharpValueSpan members,
        long seconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpire(
            key: key,
            seconds: seconds,
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// Specify an expiration (TTL or time to live), in seconds, for one or more members of a set. Redis removes each member from the set automatically when its TTL expires.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置指定秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 可以指定多个</para>
    /// </param>
    /// <param name="seconds">The time to live, in seconds. Each specified member is removed after this many seconds.
    /// <para>剩余有效期, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SExpireAsync(
        SharpValue key,
        SharpValueSpan members,
        TimeSpan seconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpire(
            key: key,
            seconds: (long)seconds.TotalSeconds,
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but instead of a TTL in seconds, it takes the absolute Unix time at which the members expire, in seconds since the Unix epoch.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置到期时间, 时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in seconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SExpireAtAsync(
        SharpValue key,
        SharpValue member,
        long timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpireAt(
            key: key,
            timestamp: timestamp,
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but instead of a TTL in seconds, it takes the absolute Unix time at which the members expire, in seconds since the Unix epoch.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置到期时间, 时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in seconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SExpireAtAsync(
        SharpValue key,
        SharpValue member,
        DateTimeOffset timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpireAt(
            key: key,
            timestamp: timestamp.ToUnixTimeSeconds(),
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but instead of a TTL in seconds, it takes the absolute Unix time at which the members expire, in seconds since the Unix epoch.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置到期时间, 时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 可以指定多个</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in seconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SExpireAtAsync(
        SharpValue key,
        SharpValueSpan members,
        long timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpireAt(
            key: key,
            timestamp: timestamp,
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but instead of a TTL in seconds, it takes the absolute Unix time at which the members expire, in seconds since the Unix epoch.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置到期时间, 时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 可以指定多个</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in seconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SExpireAtAsync(
        SharpValue key,
        SharpValueSpan members,
        DateTimeOffset timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpireAt(
            key: key,
            timestamp: timestamp.ToUnixTimeSeconds(),
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the absolute Unix timestamp, in seconds since the Unix epoch
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个成员的到期时间的时间戳. 秒级时间戳</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to retrieve the expiration time for.
    /// <para>要获得到期时间的成员</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the expiration time, as a Unix timestamp in seconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>成员到期时间的秒级时间戳. -2: 成员不存在, -1: 成员用不失效.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<long> SExpireTimeAsync(
        SharpValue key,
        SharpValue member,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpireTime(
            key: key,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the absolute Unix timestamp, in seconds since the Unix epoch
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个或多个成员的到期时间的时间戳. 秒级时间戳</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to retrieve the expiration time for.
    /// <para>要获得到期时间的成员, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the expiration time, as a Unix timestamp in seconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>成员到期时间的秒级时间戳. -2: 成员不存在, -1: 成员用不失效.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<long[]> SExpireTimeAsync(
        SharpValue key,
        SharpValueSpan members,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SExpireTime(
            key: key,
            members: members) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Removes the existing expiration from one or more set members. This turns each member from volatile (a member with an expiration) back into persistent (a member with no TTL, which never expires).
    /// <para>Available since: 8.12.0</para>
    /// <para>移除一个成员的失效时间, 使其变为用不过期</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合Key</para>
    /// </param>
    /// <param name="member">The set member to remove the expiration from
    /// <para>要移除过期时间的成员</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPersist">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPersist">命令文档</see></para>
    /// </remarks>
    public Task<SetPersistResult> SPersistAsync(
        SharpValue key,
        SharpValue member,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPersist(
            key: key,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// Removes the existing expiration from one or more set members. This turns each member from volatile (a member with an expiration) back into persistent (a member with no TTL, which never expires).
    /// <para>Available since: 8.12.0</para>
    /// <para>移除一个或多个成员的失效时间, 使其变为用不过期</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合Key</para>
    /// </param>
    /// <param name="members">The set members to remove the expiration from
    /// <para>要移除过期时间的成员, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPersist">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPersist">命令文档</see></para>
    /// </remarks>
    public Task<SetPersistResult[]> SPersistAsync(
        SharpValue key,
        SharpValueSpan members,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPersist(
            key: key,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but the expiration of a member is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置指定毫秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="milliseconds">The time to live, in milliseconds. Each specified member is removed after this many milliseconds.
    /// <para>剩余有效期, 单位: 毫秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SPExpireAsync(
        SharpValue key,
        SharpValue member,
        long milliseconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpire(
            key: key,
            milliseconds: milliseconds,
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but the expiration of a member is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置指定毫秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="milliseconds">The time to live, in milliseconds. Each specified member is removed after this many milliseconds.
    /// <para>剩余有效期, 单位: 毫秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SPExpireAsync(
        SharpValue key,
        SharpValue member,
        TimeSpan milliseconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpire(
            key: key,
            milliseconds: (long)milliseconds.TotalMilliseconds,
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but the expiration of a member is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置指定毫秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 支持多个</para>
    /// </param>
    /// <param name="milliseconds">The time to live, in milliseconds. Each specified member is removed after this many milliseconds.
    /// <para>剩余有效期, 单位: 毫秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SPExpireAsync(
        SharpValue key,
        SharpValueSpan members,
        long milliseconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpire(
            key: key,
            milliseconds: milliseconds,
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRE, but the expiration of a member is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置指定毫秒的有效期. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 支持多个</para>
    /// </param>
    /// <param name="milliseconds">The time to live, in milliseconds. Each specified member is removed after this many milliseconds.
    /// <para>剩余有效期, 单位: 毫秒. 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpire">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpire">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SPExpireAsync(
        SharpValue key,
        SharpValueSpan members,
        TimeSpan milliseconds,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpire(
            key: key,
            milliseconds: (long)milliseconds.TotalMilliseconds,
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIREAT, but the absolute Unix time at which the members expire is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置到期时间, 毫秒级时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in milliseconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 毫秒, 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SPExpireAtAsync(
        SharpValue key,
        SharpValue member,
        long timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpireAt(
            key: key,
            timestamp: timestamp,
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIREAT, but the absolute Unix time at which the members expire is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个成员设置到期时间, 毫秒级时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to specify expiration for.
    /// <para>要设置有效期的成员</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in milliseconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 毫秒, 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult> SPExpireAtAsync(
        SharpValue key,
        SharpValue member,
        DateTimeOffset timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpireAt(
            key: key,
            timestamp: timestamp.ToUnixTimeMilliseconds(),
            when: when,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIREAT, but the absolute Unix time at which the members expire is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置到期时间, 毫秒级时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 支持多个</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in milliseconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 毫秒, 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SPExpireAtAsync(
        SharpValue key,
        SharpValueSpan members,
        long timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpireAt(
            key: key,
            timestamp: timestamp,
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIREAT, but the absolute Unix time at which the members expire is specified in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>给Set集合中的一个或多个成员设置到期时间, 毫秒级时间戳形式的. 到期后成员会自动删除</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to specify expiration for.
    /// <para>要设置有效期的成员, 支持多个</para>
    /// </param>
    /// <param name="timestamp">The expiration time, as a Unix timestamp in milliseconds. A time in the past removes the members immediately.
    /// <para>到期时间的时间戳形式, 单位: 毫秒, 到期后成员会删除</para>
    /// </param>
    /// <param name="when">Enum of conditions for setting the validity period of members in SET
    /// <para>Set中成员失效时间设置条件枚举</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpireAt">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpireAt">命令文档</see></para>
    /// </remarks>
    public Task<SetExpireResult[]> SPExpireAtAsync(
        SharpValue key,
        SharpValueSpan members,
        DateTimeOffset timestamp,
        ExpirationWhen when = ExpirationWhen.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpireAt(
            key: key,
            timestamp: timestamp.ToUnixTimeMilliseconds(),
            when: when,
            members: members) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRETIME, but returns the absolute Unix expiration timestamp of each member in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个成员的到期时间的时间戳. 毫秒级时间戳</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to retrieve the expiration time for.
    /// <para>要获得到期时间的成员</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the expiration time, as a Unix timestamp in seconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>成员到期时间的毫秒级时间戳. -2: 成员不存在, -1: 成员用不失效.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<long> SPExpireTimeAsync(
        SharpValue key,
        SharpValue member,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpireTime(
            key: key,
            member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like SEXPIRETIME, but returns the absolute Unix expiration timestamp of each member in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个或多个成员的到期时间的时间戳. 毫秒级时间戳</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to retrieve the expiration time for.
    /// <para>要获得到期时间的成员, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the expiration time, as a Unix timestamp in seconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>成员到期时间的毫秒级时间戳. -2: 成员不存在, -1: 成员用不失效.</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPExpireTime">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPExpireTime">命令文档</see></para>
    /// </remarks>
    public Task<long[]> SPExpireTimeAsync(
        SharpValue key,
        SharpValueSpan members,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPExpireTime(
            key: key,
            members: members) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Returns the remaining TTL (time to live), in seconds, of one set member that have an expiration. Use it to check how many more seconds a member stays in the set.
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个成员剩余的有效期. 剩余有效期的秒数</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to retrieve the remaining TTL for.
    /// <para>要获得剩余有效期的成员</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the TTL in seconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>剩余有效期秒数. 如果不存在Key或成员返回-2, 如果用不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/STtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/STtl">命令文档</see></para>
    /// </remarks>
    public Task<long> STtlAsync(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.STtl(key: key, member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the remaining TTL (time to live), in seconds, of one or more set members that have an expiration. Use it to check how many more seconds a member stays in the set.
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个或多个成员剩余的有效期. 剩余有效期的秒数</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to retrieve the remaining TTL for.
    /// <para>要获得剩余有效期的成员, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the TTL in seconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>剩余有效期秒数. 如果不存在Key或成员返回-2, 如果用不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/STtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/STtl">命令文档</see></para>
    /// </remarks>
    public Task<long[]> STtlAsync(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.STtl(key: key, members: members) | flags, cancellationToken)!;
    }

    /// <summary>
    /// This command works like STTL, but returns the remaining TTL (time to live) of each member in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个成员剩余的有效期. 剩余有效期的毫秒数</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="member">The set member to retrieve the remaining TTL for.
    /// <para>要获得剩余有效期的成员</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the TTL in milliseconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>剩余有效期毫秒数. 如果不存在Key或成员返回-2, 如果用不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPTtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPTtl">命令文档</see></para>
    /// </remarks>
    public Task<long> SPTtlAsync(SharpValue key, SharpValue member, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPTtl(key: key, member: member) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like STTL, but returns the remaining TTL (time to live) of each member in milliseconds instead of seconds.
    /// <para>Available since: 8.12.0</para>
    /// <para>获得一个或多个成员剩余的有效期. 剩余有效期的毫秒数</para>
    /// <para>支持此命令的Redis版本, 8.12.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the set.
    /// <para>Set集合的Key</para>
    /// </param>
    /// <param name="members">The set members to retrieve the remaining TTL for.
    /// <para>要获得剩余有效期的成员, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the TTL in milliseconds.
    /// <para>-2 if no such member exists in the set, or the key does not exist.</para>
    /// <para>-1 if the member exists but has no associated expiration.</para>
    /// <para>剩余有效期毫秒数. 如果不存在Key或成员返回-2, 如果用不过期返回-1</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/SPTtl">Command documentation</see>
    /// <para><see href="https://redis.io/commands/SPTtl">命令文档</see></para>
    /// </remarks>
    public Task<long[]> SPTtlAsync(SharpValue key, SharpValueSpan members, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(SetCommands.SPTtl(key: key, members: members) | flags, cancellationToken)!;
    }
}
