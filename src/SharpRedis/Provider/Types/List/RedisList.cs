using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis lists
/// <para>List类型操作类</para>
/// <para>If you are new to lists, see <see href="https://redis.io/docs/data-types/lists">Introduction to Redis lists</see></para>
/// <para>如果你不太懂lists类型, 请<see href="https://redis.io/docs/data-types/lists">查看lists文档</see></para>
/// </summary>
public sealed partial class RedisList : RedisTypeBase
{
    #region List enums
    /// <summary>
    /// The enum of the insertion position for the LINSERT command
    /// <para>LINSERT命令插入的位置枚举</para>
    /// </summary>
    public enum InsertPosition : byte
    {
        /// <summary>
        /// Insert an element before the specified pivot
        /// <para>在指定的元素前面插入元素</para>
        /// </summary>
        Before = 0,

        /// <summary>
        /// Insert an element after the specified pivot
        /// <para>在指定的元素后面插入元素</para>
        /// </summary>
        After = 1,
    }

    /// <summary>
    /// Enum of popped or pushed positions
    /// <para>移出或插入的位置枚举</para>
    /// </summary>
    public enum WherePosition : byte
    {
        /// <summary>
        /// The head position of the list
        /// <para>头部位置</para>
        /// </summary>
        Left = 0,

        /// <summary>
        /// The tail position of the list
        /// <para>尾部位置</para>
        /// </summary>
        Right = 1,
    }

    /// <summary>
    /// Move count mode. Defines whether the number parameter represents a maximum count or an exact count.
    /// <para>移动数量参数模式。定义数量参数是最大数量还是精确数量。</para>
    /// </summary>
    public enum MoveCountMode : byte
    {
        /// <summary>
        /// COUNT count. Move up to the specified number of elements. If fewer elements are available, move all available elements.
        /// <para>移动最多指定数量的元素。如果源列表元素不足，则移动实际存在的全部元素。</para>
        /// </summary>
        Count = 0,

        /// <summary>
        /// EXACTLY exactly. Move exactly the specified number of elements. The operation fails if there are not enough elements.
        /// <para>必须精确移动指定数量的元素。如果源列表元素不足，则操作失败。</para>
        /// </summary>
        Exactly = 1,
    }

    /// <summary>
    /// Move response mode. Defines how moved elements are returned.
    /// <para>移动返回模式。定义移动元素结果的返回方式。</para>
    /// </summary>
    public enum MoveReplyMode : byte
    {
        /// <summary>
        /// OBO (One By One). Return moved elements one by one.
        /// <para>逐个返回移动的元素。</para>
        /// </summary>
        Obo = 0,

        /// <summary>
        /// BULK. Return moved elements as a bulk response.
        /// <para>批量返回移动的元素。</para>
        /// </summary>
        Bulk = 1,
    }
    #endregion

    internal RedisList(BaseRedis redis)
        : base(redis)
    { }

    /// <summary>
    /// Returns the element at index index in the list stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>根据下标获得元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="index">The index is zero-based, so 0 means the first element, 1 the second element and so on. Negative indices can be used to designate elements starting at the tail of the list. Here, -1 means the last element, -2 means the penultimate and so forth.
    /// <para>下标, 下标从0开始, 表示第一个元素. 也可以使用负数, -1表示倒数第一个元素, 以此类推</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The requested element.
    /// <para>下标处的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LIndex">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LIndex">命令文档</see></para>
    /// </remarks>
    public RedisResult LIndex(SharpValue key, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LIndex(key, index) | flags, cancellationToken);

    /// <summary>
    /// Inserts element in the list stored at key either before or after the reference value pivot.
    /// <para>Available since: 2.2.0</para>
    /// <para>在指定的元素之前或之后插入一个元素</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="pivot">The reference value pivot
    /// <para>指定的条件元素</para>
    /// </param>
    /// <param name="element">The inserts element
    /// <para>要插入的元素</para>
    /// </param>
    /// <param name="position">The position of insertion
    /// <para>插入位置</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The list length after a successful insert operation.
    /// <para><b>0 when the key doesn't exist. </b> <b>-1 when the pivot wasn't found.</b></para>
    /// <para>插入成功后List的长度(里面的元素数量)</para>
    /// <para><b>返回0表示List不存在, 返回-1表示条件元素不存在, 这两种情况都是插入失败</b></para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LInsert">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LInsert">命令文档</see></para>
    /// </remarks>
    public RedisNumber LInsert(SharpValue key, SharpValue pivot, SharpValue element, InsertPosition position, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LInsert(key, position, pivot, element) | flags, cancellationToken);

    /// <summary>
    /// Returns the length of the list stored at key. If key does not exist, it is interpreted as an empty list and 0 is returned. An error is returned when the value stored at key is not a list.
    /// <para>Available since: 1.0.0</para>
    /// <para>获得List的长度(元素数量)</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list.
    /// <para>List的长度(元素数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LLen">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LLen">命令文档</see></para>
    /// </remarks>
    public RedisNumber LLen(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LLen(key) | flags, cancellationToken);

    /// <summary>
    /// Insert the specified value at the head of the list stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>将一个元素插入到List的第一个位置</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">element<para>要插入的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功之后的List长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPush">命令文档</see></para>
    /// </remarks>
    public RedisNumber LPush(SharpValue key, SharpValue element, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LPush(key, element) | flags, cancellationToken);

    /// <summary>
    /// Insert all the specified values at the head of the list stored at key.
    /// <para>Available since: 2.4.0</para>
    /// <para>将多个元素插入到List的头部(开始位置)</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="elements">elements<para>要插入的元素集合</para></param>
    /// <param name="reverse">Whether to reverse the elements before inserting them, the default is false
    /// <para>是否将元素反转后再插入, 默认为false</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功之后的List长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPush">命令文档</see></para>
    /// </remarks>
    public RedisNumber LPush(SharpValue key, SharpValueSpan elements, bool reverse = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LPush(key, elements, reverse) | flags, cancellationToken);

    /// <summary>
    /// Insert the specified value at the tail of the list stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>将一个元素插入到List的最后一个位置</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">element<para>要插入的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功之后的List长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RPush">命令文档</see></para>
    /// </remarks>
    public RedisNumber RPush(SharpValue key, SharpValue element, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.RPush(key, element) | flags, cancellationToken);

    /// <summary>
    /// Insert all the specified values at the tail of the list stored at key.
    /// <para>Available since: 2.4.0</para>
    /// <para>将多个元素插入到List的尾部(结束位置)</para>
    /// <para>支持此命令的Redis版本, 2.4.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="elements">elements<para>要插入的元素集合</para></param>
    /// <param name="reverse">Whether to reverse the elements before inserting them, the default is false
    /// <para>是否将元素反转后再插入, 默认为false</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功之后的List长度</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RPush">命令文档</see></para>
    /// </remarks>
    public RedisNumber RPush(SharpValue key, SharpValueSpan elements, bool reverse = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.RPush(key, elements, reverse) | flags, cancellationToken);

    /// <summary>
    /// Atomically returns and removes the first/last element (head/tail depending on the wherefrom argument) of the list stored at source, and pushes the element at the first/last element (head/tail depending on the whereto argument) of the list stored at destination.
    /// <para>Available since: 6.2.0</para>
    /// <para>原子性的, 从源List中的头部或尾部移出一个元素, 并插入到目标List中的头部或尾部</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="sourceKey">Source list key
    /// <para>源List的Key (要移出元素的List key)</para>
    /// </param>
    /// <param name="whereFrom">Source list pop-up position
    /// <para>源List移出元素的位置</para>
    /// </param>
    /// <param name="destinationKey">Destination list key
    /// <para>目标List key (要插入元素的List key)</para>
    /// </param>
    /// <param name="whereTo">Destination list push position
    /// <para>目标List插入元素的位置</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The element being popped and pushed
    /// <para>被移出并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LMove">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LMove">命令文档</see></para>
    /// </remarks>
    public RedisResult LMove(SharpValue sourceKey, WherePosition whereFrom, SharpValue destinationKey, WherePosition whereTo, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LMove(sourceKey, destinationKey, whereFrom, whereTo) | flags, cancellationToken);

    /// <summary>
    /// Pops one or more elements from the first non-empty list key from the list of provided key names.
    /// <para>Available since: 7.0.0</para>
    /// <para>在指定的多个List中, 从第一个非空的List中弹出一个或多个元素</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys
    /// <para>List key集合</para>
    /// </param>
    /// <param name="where">Specify the pop-out position
    /// <para>指定弹出位置</para>
    /// </param>
    /// <param name="count">The number of pops-out items, defaulting to 1.
    /// <para>弹出的数量, 默认弹出1个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>If no elements are popped out, then the IsNull attribute is set to true.
    /// <para>如果没有弹出任何元素, IsNull属性为true</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LMPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LMPop">命令文档</see></para>
    /// </remarks>
    public ListPopResult LMPop(SharpValueSpan keys, WherePosition where, uint count = 1, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LMPop(keys, where, count) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns the first element of the list stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>移除并返回List的第一个元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the first element.
    /// <para>List的第一个元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPop">命令文档</see></para>
    /// </remarks>
    public RedisResult LPop(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LPop(key) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns the first elements of the list stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>从List头部移除并返回多个元素</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="count">The count of returned elements
    /// <para>要返回的元素数量</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of popped elements.
    /// <para>弹出的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPop">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public RedisResult[]? LPop(SharpValue key, uint count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public RedisResult[] LPop(SharpValue key, uint count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.Invoke(ListCommands.LPop(key, count) | flags, cancellationToken);

    /// <summary>
    /// The command returns the index of matching elements inside a Redis list. By default, when no options are given, it will scan the list from head to tail, looking for the first match of "element". If the element is found, its index (the zero-based position in the list) is returned. Otherwise, if no match is found, nil is returned.
    /// <para>Available since: 6.0.6</para>
    /// <para>查找指定元素在List中的下标</para>
    /// <para>支持此命令的Redis版本, 6.0.6+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">element<para>查找的元素</para></param>
    /// <param name="rank">The optional arguments and options can modify the command's behavior. The RANK option specifies the "rank" of the first element to return, in case there are multiple matches. A rank of 1 means to return the first match, 2 to return the second match, and so forth.
    /// <para>Negative numbers can be used. -1 represents the first item from the end, and so on.</para>
    /// <para>从元素第几次出现的时候开始. 比如: 如果List中元素出现了3次, rank传入2, 则表示从元素出现的第二次开始返回. 也可以传入负数, -1倒数第一次出现, 以此类推</para>
    /// </param>
    /// <param name="maxlen">The MAXLEN option tells the command to compare the provided element only with a given maximum number of list items.
    /// <para>最多查找次数. 比如List有1000个元素, 指定maxlen为100, 则最多只查找100次</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An integer representing the matching element.
    /// <para>匹配元素的下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPos">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPos">命令文档</see></para>
    /// </remarks>
    public RedisNumber LPos(SharpValue key, SharpValue element, long? rank = null, long? maxlen = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LPos(key, element, rank, null, maxlen) | flags, cancellationToken);

    /// <summary>
    /// The command returns the index of matching elements inside a Redis list. By default, when no options are given, it will scan the list from head to tail, looking for the first match of "element". If the element is found, its index (the zero-based position in the list) is returned. Otherwise, if no match is found, nil is returned.
    /// <para>Available since: 6.0.6</para>
    /// <para>查找指定元素在List中的下标, 一次性返回多次出现的位置下标</para>
    /// <para>支持此命令的Redis版本, 6.0.6+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">element<para>查找的元素</para></param>
    /// <param name="count">Sometimes we want to return not just the Nth matching element, but the position of all the first N matching elements. This can be achieved using the COUNT option.
    /// <para>匹配的元素个数</para>
    /// </param>
    /// <param name="rank">The optional arguments and options can modify the command's behavior. The RANK option specifies the "rank" of the first element to return, in case there are multiple matches. A rank of 1 means to return the first match, 2 to return the second match, and so forth.
    /// <para>Negative numbers can be used. -1 represents the first item from the end, and so on.</para>
    /// <para>从元素第几次出现的时候开始. 比如: 如果List中元素出现了3次, rank传入2, 则表示从元素出现的第二次开始返回. 也可以传入负数, -1倒数第一次出现, 以此类推</para>
    /// </param>
    /// <param name="maxlen">The MAXLEN option tells the command to compare the provided element only with a given maximum number of list items.
    /// <para>最多查找次数. 比如List有1000个元素, 指定maxlen为100, 则最多只查找100次</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of integers representing the matching elements (or an empty array if there are no matches).
    /// <para>匹配元素的下标数组, 没有匹配到返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPos">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPos">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public RedisNumber[]? LPos(SharpValue key, SharpValue element, long count, long? rank = null, long? maxlen = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public RedisNumber[] LPos(SharpValue key, SharpValue element, long count, long? rank = null, long? maxlen = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.Invoke(ListCommands.LPos(key, element, rank, count, maxlen) | flags, cancellationToken);

    /// <summary>
    /// Inserts specified values at the head of the list stored at key, only if key already exists and holds a list. In contrary to LPUSH, no operation will be performed when key does not yet exist.
    /// <para>Available since: 2.2.0</para>
    /// <para>仅在指定的List key存在的时候, 才在头部插入元素. 如果List key不存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">element<para>要在头部插入的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功后的List长度(元素个数)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPushX">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPushX">命令文档</see></para>
    /// </remarks>
    public RedisNumber LPushX(SharpValue key, SharpValue element, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LPushX(key, element) | flags, cancellationToken);

    /// <summary>
    /// Inserts specified values at the head of the list stored at key, only if key already exists and holds a list. In contrary to LPUSH, no operation will be performed when key does not yet exist.
    /// <para>Available since: 4.0.0</para>
    /// <para>仅在指定的List key存在的时候, 才在头部插入元素. 如果List key不存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="elements">Elements list<para>要在头部插入的元素集合</para></param>
    /// <param name="reverse">Whether to reverse the elements before inserting them, the default is false
    /// <para>是否将元素反转后再插入, 默认为false</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功后的List长度(元素个数)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LPushX">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LPushX">命令文档</see></para>
    /// </remarks>
    public RedisNumber LPushX(SharpValue key, SharpValueSpan elements, bool reverse = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LPushX(key, elements, reverse) | flags, cancellationToken);

    /// <summary>
    /// Returns the specified elements of the list stored at key. The offsets start and stop are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on.
    /// <para>These offsets can also be negative numbers indicating offsets starting at the end of the list. For example, -1 is the last element of the list, -2 the penultimate, and so on.</para>
    /// <para>Available since: 1.0.0</para>
    /// <para>根据指定的开始和结束位置(下标), 获得范围内的所有元素. 下标从0开始, 表示第一个. 也可以是负数, -1表示倒数第一个, 以此类推</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="start">The offsets start<para>开始的下标</para></param>
    /// <param name="stop">The offsets stop<para>结束的下标</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A list of elements in the specified range, or an empty array if the key doesn't exist.
    /// <para>范围内的所有元素数组, 如果key不存在, 返回null. (包含开始和结束位置的元素)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LRange">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public RedisResult[]? LRange(SharpValue key, long start, long stop, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public RedisResult[] LRange(SharpValue key, long start, long stop, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.Invoke(ListCommands.LRange(key, start, stop) | flags, cancellationToken);

    /// <summary>
    /// Removes the first count occurrences of elements equal to element from the list stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>从List中移除指定的元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">The element to be removed<para>要移除的元素</para></param>
    /// <param name="count">count &gt; 0: Remove elements equal to element moving from head to tail.
    /// <para>count &lt; 0: Remove elements equal to element moving from tail to head.</para>
    /// <para>count = 0: Remove all elements equal to element.</para>
    /// <para>要移除的元素数量. count &gt; 0: 从头部开始, 移除count个指定的元素. count &lt; 0: 从尾部开始, 移除count个指定的元素. count = 0: 移除所有指定的元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of removed elements.
    /// <para>移除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LRem">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LRem">命令文档</see></para>
    /// </remarks>
    public RedisNumber LRem(SharpValue key, SharpValue element, long count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LRem(key, count, element) | flags, cancellationToken);

    /// <summary>
    /// Sets the list element at index to element.
    /// <para>Available since: 1.0.0</para>
    /// <para>设置指定下标处的元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">element</param>
    /// <param name="index">The index is zero-based, so 0 means the first element, 1 the second element and so on. Negative indices can be used to designate elements starting at the tail of the list. Here, -1 means the last element, -2 means the penultimate and so forth.
    /// <para>要设置元素的下标. 下标从0开始, 表示第一个, 1表示第二个. 也可以为负数, -1为倒数第一个, -2为倒数第二个, 以此类推</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LSet">命令文档</see></para>
    /// </remarks>
    public bool LSet(SharpValue key, SharpValue element, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LSet(key, index, element) | flags, cancellationToken);

    /// <summary>
    /// Trim an existing list so that it will contain only the specified range of elements specified.
    /// <para>Start and end can also be negative numbers indicating offsets from the end of the list, where -1 is the last element of the list, -2 the penultimate element and so on.</para>
    /// <para>Available since: 1.0.0</para>
    /// <para>根据指定的范围下标, 修剪List. 只保留下标范围内的元素, 其它全部删除. 下标从0开始, 表示第一个, 1表示第二个. 下标也可以使用负数, -1倒数第一个, -2倒数第二个, 以此类推</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="start">The offsets start<para>开始的下标</para></param>
    /// <param name="stop">The offsets stop<para>结束的下标</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LTrim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LTrim">命令文档</see></para>
    /// </remarks>
    public bool LTrim(SharpValue key, long start, long stop, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.LTrim(key, start, stop) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns the last element of the list stored at key.
    /// <para>Available since: 1.0.0</para>
    /// <para>移除并返回List的最后一个元素</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value of the last element.
    /// <para>List的最后一个元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RPop">命令文档</see></para>
    /// </remarks>
    public RedisResult RPop(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.RPop(key) | flags, cancellationToken);

    /// <summary>
    /// Removes and returns the last elements of the list stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>从List尾部移除并返回多个元素</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="count">The count of returned elements
    /// <para>要返回的元素数量</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of popped elements.
    /// <para>弹出的元素数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RPop">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public RedisResult[]? RPop(SharpValue key, uint count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public RedisResult[] RPop(SharpValue key, uint count, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
        => base.Invoke(ListCommands.RPop(key, count) | flags, cancellationToken);

    /// <summary>
    /// Atomically returns and removes the last element (tail) of the list stored at source, and pushes the element at the first element (head) of the list stored at destination.
    /// <para>Available since: 1.2.0 <b>As of Redis version 6.2.0, this command is regarded as deprecated.</b></para>
    /// <para>从源List的尾部移除一个元素, 并插入到目标List的头部</para>
    /// <para>支持此命令的Redis版本, 1.2.0+ <b>此命令从Redis 6.2.0+开始视为弃用</b></para>
    /// </summary>
    /// <param name="sourceKey">Source key of list<para>源List key. 要从尾部移除一个元素的Key</para></param>
    /// <param name="destinationKey">Destination key of list<para>目标List key. 要在头部插入元素的Key</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The element being popped and pushed.
    /// <para>移除并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RPopLPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RPopLPush">命令文档</see></para>
    /// </remarks>
    public RedisResult RPopLPush(SharpValue sourceKey, SharpValue destinationKey, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.RPopLPush(sourceKey, destinationKey) | flags, cancellationToken);

    /// <summary>
    /// Inserts specified values at the tail of the list stored at key, only if key already exists and holds a list. In contrary to RPUSH, no operation will be performed when key does not yet exist.
    /// <para>Available since: 2.2.0</para>
    /// <para>仅在指定的List key存在的时候, 才在尾部插入元素. 如果List key不存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 2.2.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="element">element<para>要在尾部插入的元素</para></param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功后的List长度(元素个数)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RPushX">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RPushX">命令文档</see></para>
    /// </remarks>
    public RedisNumber RPushX(SharpValue key, SharpValue element, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.RPushX(key, element) | flags, cancellationToken);

    /// <summary>
    /// Inserts specified values at the tail of the list stored at key, only if key already exists and holds a list. In contrary to RPUSH, no operation will be performed when key does not yet exist.
    /// <para>Available since: 4.0.0</para>
    /// <para>仅在指定的List key存在的时候, 才在尾部插入元素. 如果List key不存在, 不执行任何操作</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="elements">Elements list<para>要在尾部插入的元素集合</para></param>
    /// <param name="reverse">Whether to reverse the elements before inserting them, the default is false
    /// <para>是否将元素反转后再插入, 默认为false</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the list after the push operation.
    /// <para>插入成功后的List长度(元素个数)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/RPushX">Command documentation</see>
    /// <para><see href="https://redis.io/commands/RPushX">命令文档</see></para>
    /// </remarks>
    public RedisNumber RPushX(SharpValue key, SharpValueSpan elements, bool reverse = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ListCommands.RPushX(key, elements, reverse) | flags, cancellationToken);

    /// <summary>
    /// BLMOVE is the blocking variant of LMOVE.
    /// <para>Available since: 6.2.0</para>
    /// <para>BLMOVE 是 LMOVE 的阻塞版本</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="sourceKey">Source list key
    /// <para>源List的Key (要移出元素的List key)</para>
    /// </param>
    /// <param name="whereFrom">Source list pop-up position
    /// <para>源List移出元素的位置</para>
    /// </param>
    /// <param name="destinationKey">Destination list key
    /// <para>目标List key (要插入元素的List key)</para>
    /// </param>
    /// <param name="whereTo">Destination list push position
    /// <para>目标List插入元素的位置</para>
    /// </param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns>The element being popped from the source and pushed to the destination.
    /// <para>被移出并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMove">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMove">命令文档</see></para>
    /// </remarks>
    public RedisResult BLMove(SharpValue sourceKey, WherePosition whereFrom, SharpValue destinationKey, WherePosition whereTo, double timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLMove(sourceKey, destinationKey, whereFrom, whereTo, timeout) | flags, default);
    }

    /// <summary>
    /// BLMOVE is the blocking variant of LMOVE.
    /// <para>Available since: 6.2.0</para>
    /// <para>BLMOVE 是 LMOVE 的阻塞版本</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="sourceKey">Source list key
    /// <para>源List的Key (要移出元素的List key)</para>
    /// </param>
    /// <param name="whereFrom">Source list pop-up position
    /// <para>源List移出元素的位置</para>
    /// </param>
    /// <param name="destinationKey">Destination list key
    /// <para>目标List key (要插入元素的List key)</para>
    /// </param>
    /// <param name="whereTo">Destination list push position
    /// <para>目标List插入元素的位置</para>
    /// </param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns>The element being popped from the source and pushed to the destination.
    /// <para>被移出并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMove">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMove">命令文档</see></para>
    /// </remarks>
    public RedisResult BLMove(SharpValue sourceKey, WherePosition whereFrom, SharpValue destinationKey, WherePosition whereTo, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLMove(sourceKey, destinationKey, whereFrom, whereTo, timeout.TotalSeconds) | flags, default);
    }

    /// <summary>
    /// BLMPOP is the blocking variant of LMPOP.
    /// <para>Available since: 7.0.0</para>
    /// <para>在指定的多个List中, 从第一个非空的List中弹出一个或多个元素. 如果源Key不存在, 可指定阻塞等待时间</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys
    /// <para>List key集合</para>
    /// </param>
    /// <param name="where">Specify the pop-out position
    /// <para>指定弹出位置</para>
    /// </param>
    /// <param name="count">The number of pops-out items, defaulting to 1.
    /// <para>弹出的元素数量, 默认为1个</para>
    /// </param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns>If no elements are popped out, then the IsNull attribute is set to true.
    /// <para>如果没有弹出任何元素, IsNull属性为true</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMPop">命令文档</see></para>
    /// </remarks>
    public ListPopResult BLMPop(SharpValueSpan keys, WherePosition where, double timeout, uint count = 1, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLMPop(keys, where, count, timeout) | flags, default);
    }

    /// <summary>
    /// BLMPOP is the blocking variant of LMPOP.
    /// <para>Available since: 7.0.0</para>
    /// <para>在指定的多个List中, 从第一个非空的List中弹出一个或多个元素. 如果源Key不存在, 可指定阻塞等待时间</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys
    /// <para>List key集合</para>
    /// </param>
    /// <param name="where">Specify the pop-out position
    /// <para>指定弹出位置</para>
    /// </param>
    /// <param name="count">The number of pops-out items, defaulting to 1.
    /// <para>弹出的元素数量, 默认为1个</para>
    /// </param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns>If no elements are popped out, then the IsNull attribute is set to true.
    /// <para>如果没有弹出任何元素, IsNull属性为true</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMPop">命令文档</see></para>
    /// </remarks>
    public ListPopResult BLMPop(SharpValueSpan keys, WherePosition where, TimeSpan timeout, uint count = 1, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLMPop(keys, where, count, timeout.TotalSeconds) | flags, default);
    }

    /// <summary>
    /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BLPOP是LPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BLPop(SharpValue key, int timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLPop(key, timeout) | flags, default);
    }

    /// <summary>
    /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>BLPOP是LPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BLPop(SharpValue key, double timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLPop(key, timeout) | flags, default);
    }

    /// <summary>
    /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>BLPOP是LPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BLPop(SharpValue key, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLPop(key, timeout.TotalSeconds) | flags, default);
    }

    /// <summary>
    /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BLPOP是LPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BLPop(SharpValueSpan keys, int timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLPop(keys, timeout) | flags, default);
    }

    /// <summary>
    /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>BLPOP是LPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BLPop(SharpValueSpan keys, double timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLPop(keys, timeout) | flags, default);
    }

    /// <summary>
    /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 6.0.0</para>
    /// <para>BLPOP是LPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BLPop(SharpValueSpan keys, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BLPop(keys, timeout.TotalSeconds) | flags, default);
    }

    /// <summary>
    /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BRPOP是RPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BRPop(SharpValue key, int timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPop(key, timeout) | flags, default);
    }

    /// <summary>
    /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BRPOP是RPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BRPop(SharpValue key, double timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPop(key, timeout) | flags, default);
    }

    /// <summary>
    /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BRPOP是RPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="key">List key</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BRPop(SharpValue key, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPop(key, timeout.TotalSeconds) | flags, default);
    }

    /// <summary>
    /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BRPOP是RPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BRPop(SharpValueSpan keys, int timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPop(keys, timeout) | flags, default);
    }

    /// <summary>
    /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BRPOP是RPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BRPop(SharpValueSpan keys, double timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPop(keys, timeout) | flags, default);
    }

    /// <summary>
    /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given.
    /// <para>Available since: 2.0.0</para>
    /// <para>BRPOP是RPOP的阻塞等待版本</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="keys">List keys</param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPop">命令文档</see></para>
    /// </remarks>
    public ListBlockPopResult BRPop(SharpValueSpan keys, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPop(keys, timeout.TotalSeconds) | flags, default);
    }

    /// <summary>
    /// BRPOPLPUSH is the blocking variant of RPOPLPUSH. When source contains elements, this command behaves exactly like RPOPLPUSH. When used inside a MULTI/EXEC block or a Lua script, this command behaves exactly like RPOPLPUSH. When source is empty, Redis will block the connection until another client pushes to it or until timeout is reached. A timeout of zero can be used to block indefinitely.
    /// <para>Available since: 2.2.0 <b>As of Redis version 6.2.0, this command is regarded as deprecated.</b></para>
    /// <para>BRPOPLPUSH 是 RPOPLPUSH 的阻塞版本。当 source 包含元素时，此命令的行为与 RPOPLPUSH 完全相同</para>
    /// <para>支持此命令的Redis版本, 2.2.0+ <b>Redis 6.2.0及以后的版本视为弃用</b></para>
    /// </summary>
    /// <param name="sourceKey">Source key of list<para>源List key. 要从尾部移除一个元素的Key</para></param>
    /// <param name="destinationKey">Destination key of list<para>目标List key. 要在头部插入元素的Key</para></param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns>The element being popped and pushed.
    /// <para>移除并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPopLPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPopLPush">命令文档</see></para>
    /// </remarks>
    public RedisResult BRPopLPush(SharpValue sourceKey, SharpValue destinationKey, int timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPopLPush(sourceKey, destinationKey, timeout) | flags, default);
    }

    /// <summary>
    /// BRPOPLPUSH is the blocking variant of RPOPLPUSH. When source contains elements, this command behaves exactly like RPOPLPUSH. When used inside a MULTI/EXEC block or a Lua script, this command behaves exactly like RPOPLPUSH. When source is empty, Redis will block the connection until another client pushes to it or until timeout is reached. A timeout of zero can be used to block indefinitely.
    /// <para>Available since: 6.0.0 <b>As of Redis version 6.2.0, this command is regarded as deprecated.</b></para>
    /// <para>BRPOPLPUSH 是 RPOPLPUSH 的阻塞版本。当 source 包含元素时，此命令的行为与 RPOPLPUSH 完全相同</para>
    /// <para>支持此命令的Redis版本, 6.0.0+ <b>Redis 6.2.0及以后的版本视为弃用</b></para>
    /// </summary>
    /// <param name="sourceKey">Source key of list<para>源List key. 要从尾部移除一个元素的Key</para></param>
    /// <param name="destinationKey">Destination key of list<para>目标List key. 要在头部插入元素的Key</para></param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns>The element being popped and pushed.
    /// <para>移除并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPopLPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPopLPush">命令文档</see></para>
    /// </remarks>
    public RedisResult BRPopLPush(SharpValue sourceKey, SharpValue destinationKey, double timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPopLPush(sourceKey, destinationKey, timeout) | flags, default);
    }

    /// <summary>
    /// BRPOPLPUSH is the blocking variant of RPOPLPUSH. When source contains elements, this command behaves exactly like RPOPLPUSH. When used inside a MULTI/EXEC block or a Lua script, this command behaves exactly like RPOPLPUSH. When source is empty, Redis will block the connection until another client pushes to it or until timeout is reached. A timeout of zero can be used to block indefinitely.
    /// <para>Available since: 6.0.0 <b>As of Redis version 6.2.0, this command is regarded as deprecated.</b></para>
    /// <para>BRPOPLPUSH 是 RPOPLPUSH 的阻塞版本。当 source 包含元素时，此命令的行为与 RPOPLPUSH 完全相同</para>
    /// <para>支持此命令的Redis版本, 6.0.0+ <b>Redis 6.2.0及以后的版本视为弃用</b></para>
    /// </summary>
    /// <param name="sourceKey">Source key of list<para>源List key. 要从尾部移除一个元素的Key</para></param>
    /// <param name="destinationKey">Destination key of list<para>目标List key. 要在头部插入元素的Key</para></param>
    /// <param name="timeout">When source i empty, Redis will block the connection until another client pushes to it or until timeout (a double value specifying the maximum number of seconds to block) is reached. A timeout of zero can be used to block indefinitely.
    /// <para>当源List为空时, 阻塞超时的时间, 单位: 秒. 无限等待传0</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <returns>The element being popped and pushed.
    /// <para>移除并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BRPopLPush">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BRPopLPush">命令文档</see></para>
    /// </remarks>
    public RedisResult BRPopLPush(SharpValue sourceKey, SharpValue destinationKey, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        return base.Invoke(ListCommands.BRPopLPush(sourceKey, destinationKey, timeout.TotalSeconds) | flags, default);
    }

    /// <summary>
    /// Atomically move an element stored in the source List to the target List, and return the moved element.
    /// <para>Available since: 8.10.0</para>
    /// <para>原子性地将源List中存储的一个元素移动到目标List中，并返回被移动的元素。</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="sourceKey">The key of the source list.
    /// <para>源List的Key</para>
    /// </param>
    /// <param name="destinationKey">The key of the destination list.
    /// <para>目标List的Key (存储移出元素的List的Key)</para>
    /// </param>
    /// <param name="whereFrom">The end of source to pop elements from: LEFT (head) or RIGHT (tail).
    /// <para>从源List的哪一端弹出元素</para>
    /// </param>
    /// <param name="whereTo">The end of destination to push elements to: LEFT (head) or RIGHT (tail).
    /// <para>将元素推入目标List的哪一端</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The moved element
    /// <para>被移出并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LMoveM">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LMoveM">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    LMoveM(
        SharpValue sourceKey,
        SharpValue destinationKey,
        WherePosition whereFrom,
        WherePosition whereTo,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(ListCommands.LMoveM(sourceKey, destinationKey, whereFrom, whereTo, null, null, null) | flags, cancellationToken);
    }

    /// <summary>
    /// Atomically moves one or more elements from the list stored at source to the list stored at destination and returns the moved elements. LMOVEM is the multiple-element version of LMOVE: without the optional how-many block, it behaves exactly like LMOVE but returns the moved element as a one-element array.
    /// <para>Available since: 8.10.0</para>
    /// <para>原子性地将源List中存储的一个或多个元素移动到目标List中，并返回被移动的元素。LMOVEM 是 LMOVE 的多元素版本：如果没有可选的“how-many”块，其行为与 LMOVE 完全相同，但返回的移动元素会作为一个单元素数组。</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="sourceKey">The key of the source list.
    /// <para>源List的Key</para>
    /// </param>
    /// <param name="destinationKey">The key of the destination list.
    /// <para>目标List的Key (存储移出元素的List的Key)</para>
    /// </param>
    /// <param name="whereFrom">The end of source to pop elements from: LEFT (head) or RIGHT (tail).
    /// <para>从源List的哪一端弹出元素</para>
    /// </param>
    /// <param name="whereTo">The end of destination to push elements to: LEFT (head) or RIGHT (tail).
    /// <para>将元素推入目标List的哪一端</para>
    /// </param>
    /// <param name="count">The number of elements removed
    /// <para>要移出的元素数量</para>
    /// </param>
    /// <param name="countMode">Move count mode. Defines whether the number parameter represents a maximum count or an exact count.
    /// <para>移动数量参数模式。定义数量参数是最大数量还是精确数量。</para>
    /// </param>
    /// <param name="replyMode">Move response mode. Defines how moved elements are returned.
    /// <para>移动返回模式。定义移动元素结果的返回方式。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the moved elements, in destination order.
    /// <para>被移出并插入的元素数组, 按目标顺序插入返回的</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/LMoveM">Command documentation</see>
    /// <para><see href="https://redis.io/commands/LMoveM">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    LMoveM(
        SharpValue sourceKey,
        SharpValue destinationKey,
        WherePosition whereFrom,
        WherePosition whereTo,
        uint count,
        MoveCountMode countMode,
        MoveReplyMode replyMode,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(ListCommands.LMoveM(sourceKey, destinationKey, whereFrom, whereTo, count, countMode, replyMode) | flags, cancellationToken);
    }

    /// <summary>
    /// BLMOVEM is the blocking variant of LMOVEM. When source holds enough elements to satisfy the request, this command behaves exactly like LMOVEM. Otherwise, Redis blocks the connection until another client pushes the required elements to source or until timeout is reached.
    /// <para>Available since: 8.10.0</para>
    /// <para>BLMOVEM 是 LMOVEM 的阻塞版本</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="sourceKey">The key of the source list.
    /// <para>源List的Key</para>
    /// </param>
    /// <param name="destinationKey">The key of the destination list.
    /// <para>目标List的Key (存储移出元素的List的Key)</para>
    /// </param>
    /// <param name="whereFrom">The end of source to pop elements from: LEFT (head) or RIGHT (tail).
    /// <para>从源List的哪一端弹出元素</para>
    /// </param>
    /// <param name="whereTo">The end of destination to push elements to: LEFT (head) or RIGHT (tail).
    /// <para>将元素推入目标List的哪一端</para>
    /// </param>
    /// <param name="timeout">The maximum time to block, in seconds. A timeout of 0 blocks indefinitely.
    /// <para>最长阻塞时间，单位: 秒。为0表示无限期阻塞。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The moved element
    /// <para>被移出并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMoveM">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMoveM">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    BLMoveM(
        SharpValue sourceKey,
        SharpValue destinationKey,
        WherePosition whereFrom,
        WherePosition whereTo,
        double timeout,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(ListCommands.BLMoveM(sourceKey, destinationKey, whereFrom, whereTo, timeout, null, null, null) | flags, cancellationToken);
    }

    /// <summary>
    /// BLMOVEM is the blocking variant of LMOVEM. When source holds enough elements to satisfy the request, this command behaves exactly like LMOVEM. Otherwise, Redis blocks the connection until another client pushes the required elements to source or until timeout is reached.
    /// <para>Available since: 8.10.0</para>
    /// <para>BLMOVEM 是 LMOVEM 的阻塞版本</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="sourceKey">The key of the source list.
    /// <para>源List的Key</para>
    /// </param>
    /// <param name="destinationKey">The key of the destination list.
    /// <para>目标List的Key (存储移出元素的List的Key)</para>
    /// </param>
    /// <param name="whereFrom">The end of source to pop elements from: LEFT (head) or RIGHT (tail).
    /// <para>从源List的哪一端弹出元素</para>
    /// </param>
    /// <param name="whereTo">The end of destination to push elements to: LEFT (head) or RIGHT (tail).
    /// <para>将元素推入目标List的哪一端</para>
    /// </param>
    /// <param name="timeout">The maximum time to block, in seconds. A timeout of 0 blocks indefinitely.
    /// <para>最长阻塞时间，单位: 秒。为0表示无限期阻塞。</para>
    /// </param>
    /// <param name="count">The number of elements removed
    /// <para>要移出的元素数量</para>
    /// </param>
    /// <param name="countMode">Move count mode. Defines whether the number parameter represents a maximum count or an exact count.
    /// <para>移动数量参数模式。定义数量参数是最大数量还是精确数量。</para>
    /// </param>
    /// <param name="replyMode">Move response mode. Defines how moved elements are returned.
    /// <para>移动返回模式。定义移动元素结果的返回方式。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the moved elements, in destination order.
    /// <para>被移出并插入的元素数组, 按目标顺序插入返回的</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMoveM">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMoveM">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    BLMoveM(
        SharpValue sourceKey,
        SharpValue destinationKey,
        WherePosition whereFrom,
        WherePosition whereTo,
        double timeout,
        uint count,
        MoveCountMode countMode,
        MoveReplyMode replyMode,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(ListCommands.BLMoveM(sourceKey, destinationKey, whereFrom, whereTo, timeout, count, countMode, replyMode) | flags, cancellationToken);
    }

    /// <summary>
    /// BLMOVEM is the blocking variant of LMOVEM. When source holds enough elements to satisfy the request, this command behaves exactly like LMOVEM. Otherwise, Redis blocks the connection until another client pushes the required elements to source or until timeout is reached.
    /// <para>Available since: 8.10.0</para>
    /// <para>BLMOVEM 是 LMOVEM 的阻塞版本</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="sourceKey">The key of the source list.
    /// <para>源List的Key</para>
    /// </param>
    /// <param name="destinationKey">The key of the destination list.
    /// <para>目标List的Key (存储移出元素的List的Key)</para>
    /// </param>
    /// <param name="whereFrom">The end of source to pop elements from: LEFT (head) or RIGHT (tail).
    /// <para>从源List的哪一端弹出元素</para>
    /// </param>
    /// <param name="whereTo">The end of destination to push elements to: LEFT (head) or RIGHT (tail).
    /// <para>将元素推入目标List的哪一端</para>
    /// </param>
    /// <param name="timeout">The maximum time to block, in seconds. A timeout of 0 blocks indefinitely.
    /// <para>最长阻塞时间，单位: 秒。为0表示无限期阻塞。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The moved element
    /// <para>被移出并插入的元素</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMoveM">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMoveM">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    BLMoveM(
        SharpValue sourceKey,
        SharpValue destinationKey,
        WherePosition whereFrom,
        WherePosition whereTo,
        TimeSpan timeout,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(ListCommands.BLMoveM(sourceKey, destinationKey, whereFrom, whereTo, timeout.TotalSeconds, null, null, null) | flags, cancellationToken);
    }

    /// <summary>
    /// BLMOVEM is the blocking variant of LMOVEM. When source holds enough elements to satisfy the request, this command behaves exactly like LMOVEM. Otherwise, Redis blocks the connection until another client pushes the required elements to source or until timeout is reached.
    /// <para>Available since: 8.10.0</para>
    /// <para>BLMOVEM 是 LMOVEM 的阻塞版本</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="sourceKey">The key of the source list.
    /// <para>源List的Key</para>
    /// </param>
    /// <param name="destinationKey">The key of the destination list.
    /// <para>目标List的Key (存储移出元素的List的Key)</para>
    /// </param>
    /// <param name="whereFrom">The end of source to pop elements from: LEFT (head) or RIGHT (tail).
    /// <para>从源List的哪一端弹出元素</para>
    /// </param>
    /// <param name="whereTo">The end of destination to push elements to: LEFT (head) or RIGHT (tail).
    /// <para>将元素推入目标List的哪一端</para>
    /// </param>
    /// <param name="timeout">The maximum time to block, in seconds. A timeout of 0 blocks indefinitely.
    /// <para>最长阻塞时间，单位: 秒。为0表示无限期阻塞。</para>
    /// </param>
    /// <param name="count">The number of elements removed
    /// <para>要移出的元素数量</para>
    /// </param>
    /// <param name="countMode">Move count mode. Defines whether the number parameter represents a maximum count or an exact count.
    /// <para>移动数量参数模式。定义数量参数是最大数量还是精确数量。</para>
    /// </param>
    /// <param name="replyMode">Move response mode. Defines how moved elements are returned.
    /// <para>移动返回模式。定义移动元素结果的返回方式。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the moved elements, in destination order.
    /// <para>被移出并插入的元素数组, 按目标顺序插入返回的</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/BLMoveM">Command documentation</see>
    /// <para><see href="https://redis.io/commands/BLMoveM">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    BLMoveM(
        SharpValue sourceKey,
        SharpValue destinationKey,
        WherePosition whereFrom,
        WherePosition whereTo,
        TimeSpan timeout,
        uint count,
        MoveCountMode countMode,
        MoveReplyMode replyMode,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.Invoke(ListCommands.BLMoveM(sourceKey, destinationKey, whereFrom, whereTo, timeout.TotalSeconds, count, countMode, replyMode) | flags, cancellationToken);
    }
}
