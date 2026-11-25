using SharpRedis.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public sealed partial class RedisArray
{
    /// <summary>
    /// Sets one or more contiguous values starting at an index in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据指定的下标, 设置数组的值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key, 如果Key不存在则自动创建</para>
    /// </param>
    /// <param name="index">The zero-based integer index at which to start writing.
    /// <para>要设置值的索引, 索引从 0 开始</para>
    /// </param>
    /// <param name="value">The value to be inserted
    /// <para>要插入的值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArSet">命令文档</see></para>
    /// </remarks>
    public Task<long> ArSetAsync(
        SharpValue key,
        long index,
        SharpValue value,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArSet(key, index, value) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets one or more contiguous values starting at an index in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据指定的下标, 设置数组的值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key, 如果Key不存在则自动创建</para>
    /// </param>
    /// <param name="index">The zero-based integer index at which to start writing.
    /// <para>要设置值的索引, 索引从 0 开始</para>
    /// </param>
    /// <param name="values">One or more string values to store at consecutive indices beginning at index. The number of new (previously empty) slots filled is returned.
    /// <para>要插入的值, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArSet">命令文档</see></para>
    /// </remarks>
    public Task<long> ArSetAsync(
        SharpValue key,
        long index,
        SharpValueSpan values,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArSet(key, index, values) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the number of non-empty elements in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>统计数组中, 非空元素的数量</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of non-empty elements, or 0 if key does not exist.
    /// <para>非空元素的数量, 如果Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArCount">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArCount">命令文档</see></para>
    /// </remarks>
    public Task<long> ArCountAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArCount(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements at the specified indices in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除数组中, 指定索引处的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组key</para>
    /// </param>
    /// <param name="index">One zero-based integer indices of the elements to delete
    /// <para>要删除元素的索引, 从0开始的索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDel">命令文档</see></para>
    /// </remarks>
    public Task<long> ArDelAsync(SharpValue key, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArDel(key, index) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements at the specified indices in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除数组中, 指定索引处的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组key</para>
    /// </param>
    /// <param name="indexs">One or more zero-based integer indices of the elements to delete. 
    /// <para>要删除元素的索引, 从0开始的索引. 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDel">命令文档</see></para>
    /// </remarks>
    public Task<long> ArDelAsync(SharpValue key, ReadOnlySpan<long> indexs, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArDel(key, indexs) | flags, cancellationToken);
    }

    /// <summary>
    /// Delete elements in one range.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="start">the start index
    /// <para>开始的索引</para>
    /// </param>
    /// <param name="end">the end index
    /// <para>结束的索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public Task<long> ArDelRangeAsync(SharpValue key, long start, long end, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArDelRange(key, start, end) | flags, cancellationToken);
    }

    /// <summary>
    /// Delete elements in one range.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public Task<long> ArDelRangeAsync(SharpValue key, RedisArray.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArDelRange(key, range) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements in one or more ranges.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素, 可指定一组或多组范围索引</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="ranges">One or more start end pairs, each defining an inclusive range of indices to delete.
    /// <para>包含开始和结束的范围索引, 支持多组传入</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public Task<long> ArDelRangeAsync(SharpValue key, ReadOnlySpan<RedisArray.Range> ranges, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArDelRange(key, ranges) | flags, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Delete elements in one range.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public Task<long> ArDelRangeAsync(SharpValue key, System.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArDelRange(key, range) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes elements in one or more ranges.
    /// <para>Available since: 8.8.0</para>
    /// <para>删除范围索引内的元素, 可指定一组或多组范围索引</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="ranges">One or more start end pairs, each defining an inclusive range of indices to delete.
    /// <para>包含开始和结束的范围索引, 支持多组传入</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of elements deleted.
    /// <para>删除的元素数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArDelRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArDelRange">命令文档</see></para>
    /// </remarks>
    public Task<long> ArDelRangeAsync(SharpValue key, ReadOnlySpan<System.Range> ranges, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArDelRange(key, ranges) | flags, cancellationToken);
    }

    /// <summary>
    /// Gets values in a range of indices. If start is greater than end, elements are returned in reverse index order.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得指定范围索引内的元素值. 如果开始的索引大于结束索引, 则按索引逆序返回元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGetRange">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult[]> ArGetRangeAsync(SharpValue key, System.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArGetRange(key, range) | flags, cancellationToken)!;
    }
#endif

    /// <summary>
    /// Gets the value at an index in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据索引获得元素值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="index">The zero-based integer index of the element to retrieve.
    /// <para>要获取元素值的索引, 从0开始的</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The value at the given index.
    /// <para>索引处的元素值</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGet">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult> ArGetAsync(SharpValue key, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArGet(key, index) | flags, cancellationToken);
    }

    /// <summary>
    /// Gets values in a range of indices. If start is greater than end, elements are returned in reverse index order.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得指定范围索引内的元素值. 如果开始的索引大于结束索引, 则按索引逆序返回元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的key</para>
    /// </param>
    /// <param name="start">The zero-based integer index of the first element to return.
    /// <para>开始的索引, 从0开始</para>
    /// </param>
    /// <param name="end">The zero-based integer index of the last element to return (inclusive)
    /// <para>结束的索引, 从0开始</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGetRange">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult[]> ArGetRangeAsync(SharpValue key, long start, long end, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArGetRange(key, start, end) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Gets values in a range of indices. If start is greater than end, elements are returned in reverse index order.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得指定范围索引内的元素值. 如果开始的索引大于结束索引, 则按索引逆序返回元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的key</para>
    /// </param>
    /// <param name="range">One start end pairs, each defining an inclusive range of indices to delete. 
    /// <para>包含开始和结束的范围索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ArGetRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ArGetRange">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult[]> ArGetRangeAsync(SharpValue key, RedisArray.Range range, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArGetRange(key, range) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Searches array elements in the inclusive range from start to end.
    /// <para>Available since: 8.8.0</para>
    /// <para>按起止索引指定的方向搜索数组元素，包含两个端点。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Array key.
    /// <para>数组的 key。</para>
    /// </param>
    /// <param name="start">First index or boundary.
    /// <para>起始索引或边界符号。</para>
    /// </param>
    /// <param name="end">Last index or boundary.
    /// <para>结束索引或边界符号。</para>
    /// </param>
    /// <param name="conditions">One or more predicates and their common operator.
    /// <para>包含至少一个匹配条件及整组的逻辑运算符。</para>
    /// </param>
    /// <param name="limit">Maximum number of matches. A specified limit must be positive.
    /// <para>最多返回的匹配数，须为正数，省略时返回全部匹配项。</para>
    /// </param>
    /// <param name="withValues">Include matching values.
    /// <para>同时返回匹配元素的值。</para>
    /// </param>
    /// <param name="noCase">Ignore case for all predicates.
    /// <para>全部条件均忽略大小写。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Matches in traversal order. Value is RedisResult.Empty unless withValues is enabled.
    /// <para>按遍历顺序返回匹配项，省略 withValues 时 Value 为 RedisResult.Empty。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/argrep">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/argrep">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<ArItem[]?>
#else
    Task<ArItem[]>
#endif
    ArGrepAsync(
        SharpValue key,
        GrepIndex start,
        GrepIndex end,
        GrepConditions conditions,
        long? limit = null,
        bool withValues = false,
        bool noCase = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArGrep(key, start, end, conditions, limit, withValues, noCase) | flags, cancellationToken);
    }

    /// <summary>
    /// Searches array elements in the inclusive range from start to end.
    /// <para>Available since: 8.8.0</para>
    /// <para>按起止索引指定的方向搜索数组元素，包含两个端点。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Array key.
    /// <para>数组的 key。</para>
    /// </param>
    /// <param name="start">First index or boundary.
    /// <para>起始索引或边界符号。</para>
    /// </param>
    /// <param name="end">Last index or boundary.
    /// <para>结束索引或边界符号。</para>
    /// </param>
    /// <param name="condition">One predicate
    /// <para>一个匹配条件。</para>
    /// </param>
    /// <param name="limit">Maximum number of matches. A specified limit must be positive.
    /// <para>最多返回的匹配数，须为正数，省略时返回全部匹配项。</para>
    /// </param>
    /// <param name="withValues">Include matching values.
    /// <para>同时返回匹配元素的值。</para>
    /// </param>
    /// <param name="noCase">Ignore case for all predicates.
    /// <para>全部条件均忽略大小写。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Matches in traversal order. Value is RedisResult.Empty unless withValues is enabled.
    /// <para>按遍历顺序返回匹配项，省略 withValues 时 Value 为 RedisResult.Empty。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/argrep">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/argrep">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<ArItem[]?>
#else
    Task<ArItem[]>
#endif
    ArGrepAsync(
        SharpValue key,
        GrepIndex start,
        GrepIndex end,
        GrepCondition condition,
        long? limit = null,
        bool withValues = false,
        bool noCase = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArGrep(key, start, end, condition, limit, withValues, noCase) | flags, cancellationToken);
    }

    /// <summary>
    /// Inserts one or more values at consecutive indices.
    /// <para>Available since: 8.8.0</para>
    /// <para>在连续的索引处插入一个或多个值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="value">One or more string values to insert at consecutive indices, beginning at the current insert cursor position. The cursor advances by one for each value inserted.
    /// <para>要插入的值, 从当前插入索引位置开始, 每次插入一个值, 索引则向前进一</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>上次插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arinsert">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arinsert">命令文档</see></para>
    /// </remarks>
    public Task<long> ArInsertAsync(SharpValue key, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArInsert(key, value) | flags, cancellationToken);
    }

    /// <summary>
    /// Inserts one or more values at consecutive indices.
    /// <para>Available since: 8.8.0</para>
    /// <para>在连续的索引处插入一个或多个值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组类型的Key</para>
    /// </param>
    /// <param name="values">One or more string values to insert at consecutive indices, beginning at the current insert cursor position. The cursor advances by one for each value inserted.
    /// <para>要插入的值, 支持多个. 从当前插入索引位置开始, 每次插入一个值, 索引则向前进一</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>上次插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arinsert">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arinsert">命令文档</see></para>
    /// </remarks>
    public Task<long> ArInsertAsync(SharpValue key, SharpValueSpan values, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArInsert(key, values) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the most recently inserted elements.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得最近插入的元素列表</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="count">The maximum number of most recently inserted elements to return. If the array contains fewer elements than count, all elements are returned.
    /// <para>要返回的最近插入元素的最多数量。如果数组中的元素数量少于 count ，则返回所有元素。</para>
    /// </param>
    /// <param name="rev">When present, returns elements in reverse chronological order (most recent first) instead of the default oldest-first order.
    /// <para>是否按插入时间, 倒序返回. 默认为: 插入最早的在前</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arlastitems">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arlastitems">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<RedisResult[]?>
#else
    Task<RedisResult[]>
#endif
    ArLastItemsAsync(SharpValue key, long count, bool rev = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArLastItems(key, count, rev) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the length of an array (max index + 1).
    /// <para>Available since: 8.8.0</para>
    /// <para>获得数组的长度</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The length of the array (max index + 1), or 0 if key does not exist.
    /// <para>数组的长度, 如果Key不存在返回0</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arlen">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arlen">命令文档</see></para>
    /// </remarks>
    public Task<long> ArLenAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArLen(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Gets values at multiple indices in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据多个索引下标, 批量获取元素值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="indexs">One or more zero-based integer indices of the elements to retrieve. The reply preserves the order of the requested indices and returns nil for any index that is not set.
    /// <para>一个获多个索引下标</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of elements that are saved in the same order as the specified index sequence
    /// <para>和指定索引下标顺序保存一致的元素数组. 如果索引处没有元素, 返回null(RedisResult.IsNull == true)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/armget">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/armget">命令文档</see></para>
    /// </remarks>
    public Task<RedisResult[]> ArMGetAsync(SharpValue key, ReadOnlySpan<long> indexs, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArMGet(key, indexs) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Sets multiple index-value pairs in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据索引和值, 批量设置索引和值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="indexValues">One or more index value pairs. Each index is a zero-based integer specifying where to write, and each value is the string to store at that position. Pairs may be non-contiguous and in any order.
    /// <para>一个或多个 索引和值 的集合. 索引可以不是连续的, 可以任意指定</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/armset">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/armset">命令文档</see></para>
    /// </remarks>
    public Task<long> ArMSetAsync(
        SharpValue key,
        ReadOnlySpan<(long index, SharpValue value)> indexValues,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArMSet(key, indexValues) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets multiple index-value pairs in an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>根据索引和值, 批量设置索引和值</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="indexValues">One or more index value pairs. Each index is a zero-based integer specifying where to write, and each value is the string to store at that position. Pairs may be non-contiguous and in any order.
    /// <para>一个或多个 索引和值 的集合. 索引可以不是连续的, 可以任意指定</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of new slots that were set (previously empty).
    /// <para>新增的数组槽位数量 (非设置成功的值数量)</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/armset">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/armset">命令文档</see></para>
    /// </remarks>
    public Task<long> ArMSetAsync(
        SharpValue key,
        ReadOnlySpan<KeyValuePair<long, SharpValue>> indexValues,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArMSet(key, indexValues) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the next index ARINSERT would use.
    /// <para>Available since: 8.8.0</para>
    /// <para>返回 ARINSERT 将使用的下一个索引下标</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The next index ARINSERT would use. Returns 0 for missing keys or when no insert happened yet.
    /// <para>Null when the insertion cursor is exhausted (next insert would overflow).</para>
    /// <para>ARINSERT 将使用的下一个索引下标. Key不存在或者没有插入操作返回0</para>
    /// <para>当插入索引下标耗尽时（下一次插入将溢出）, 返回null</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arnext">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arnext">命令文档</see></para>
    /// </remarks>
    public Task<long?> ArNextAsync(SharpValue key,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArNext(key) | flags, cancellationToken);
    }

    /// <summary>
    /// Performs aggregate operations on array elements in a range.
    /// <para>Available since: 8.8.0</para>
    /// <para>对指定范围内的元素进行聚合操作</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="start">The zero-based integer index of the first element in the range to aggregate.
    /// <para>要聚合的开始元素下标</para>
    /// </param>
    /// <param name="end">The zero-based integer index of the last element in the range to aggregate (inclusive).
    /// <para>要聚合的结束元素下标</para>
    /// </param>
    /// <param name="op">Aggregation operation type
    /// <para>聚合操作类型</para>
    /// </param>
    /// <param name="matchValue">Counts the values of the elements that specify a certain quantity. The parameter is invalid if it is not used for specifying the quantity of elements.
    /// <para>统计指定元素值数量的值, 非统计元素值数量时, 该参数无效</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arop">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arop">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> ArOpAsync(
        SharpValue key,
        long start,
        long end,
        RedisArray.AggregateOp op,
        SharpValue matchValue = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArOp(key: key, start: start, end: end, op: op, matchValue: matchValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Performs aggregate operations on array elements in a range.
    /// <para>Available since: 8.8.0</para>
    /// <para>对指定范围内的元素进行聚合操作</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be aggregated
    /// <para>要聚合的元素索引范围</para>
    /// </param>
    /// <param name="op">Aggregation operation type
    /// <para>聚合操作类型</para>
    /// </param>
    /// <param name="matchValue">Counts the values of the elements that specify a certain quantity. The parameter is invalid if it is not used for specifying the quantity of elements.
    /// <para>统计指定元素值数量的值, 非统计元素值数量时, 该参数无效</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arop">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arop">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> ArOpAsync(
        SharpValue key,
        RedisArray.Range range,
        RedisArray.AggregateOp op,
        SharpValue matchValue = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArOp(key: key, range: range, op: op, matchValue: matchValue) | flags, cancellationToken);
    }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Performs aggregate operations on array elements in a range.
    /// <para>Available since: 8.8.0</para>
    /// <para>对指定范围内的元素进行聚合操作</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be aggregated
    /// <para>要聚合的元素索引范围</para>
    /// </param>
    /// <param name="op">Aggregation operation type
    /// <para>聚合操作类型</para>
    /// </param>
    /// <param name="matchValue">Counts the values of the elements that specify a certain quantity. The parameter is invalid if it is not used for specifying the quantity of elements.
    /// <para>统计指定元素值数量的值, 非统计元素值数量时, 该参数无效</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arop">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arop">命令文档</see></para>
    /// </remarks>
    public Task<RedisNumber> ArOpAsync(
        SharpValue key,
        System.Range range,
        RedisArray.AggregateOp op,
        SharpValue matchValue = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArOp(key: key, range: range, op: op, matchValue: matchValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Iterates existing elements in a range, returning index-value pairs.
    /// <para>Available since: 8.8.0</para>
    /// <para>遍历指定范围内的现有元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be scanned
    /// <para>要扫描的元素索引范围. 如果开始大于结束, 则按索引逆序返回元素</para>
    /// </param>
    /// <param name="limit">The maximum number of index-value pairs to return. When omitted, all elements in the range are returned.
    /// <para>做多返回元素数量. 如果不设置此参数, 则返回索引范围内的所有元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arscan">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arscan">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<ArItem[]?>
#else
    Task<GrepResult[]>
#endif
    ArScanAsync(
        SharpValue key,
        System.Range range,
        long? limit = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArScan(key: key, range: range, limit: limit) | flags, cancellationToken);
    }
#endif

    /// <summary>
    /// Inserts values into a ring buffer of specified size, wrapping and truncating as needed.
    /// <para>Available since: 8.8.0</para>
    /// <para>将值插入指定大小的环形缓冲区，根据需要进行循环和截断。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="size">The size of the ring buffer window. Each value is inserted at insert_idx % size, wrapping back to index 0 when the end of the window is reached. When the buffer is full, newer values overwrite older ones. If size is smaller than the current window, the array is truncated to fit.
    /// <para>环形缓冲区窗口的大小。每个值都会被插入到 insert_idx % size 的位置，当窗口到达末尾时会循环回到索引 0。当缓冲区满时，新值会覆盖旧值。如果大小小于当前窗口，则数组会被截断以适应。</para>
    /// </param>
    /// <param name="value">One or more string values to insert into the ring buffer. Each value is placed at the next position in the ring and the cursor advances accordingly.
    /// <para>一个或多个字符串值要插入到环形缓冲区中。每个值都会被放置到环形缓冲区的下一个位置，同时光标相应地向前移动。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>最后一个插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arring">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arring">命令文档</see></para>
    /// </remarks>
    public Task<long> ArRingAsync(SharpValue key, long size, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArRing(key: key, size: size, value: value) | flags, cancellationToken);
    }

    /// <summary>
    /// Inserts values into a ring buffer of specified size, wrapping and truncating as needed.
    /// <para>Available since: 8.8.0</para>
    /// <para>将值插入指定大小的环形缓冲区，根据需要进行循环和截断。</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="size">The size of the ring buffer window. Each value is inserted at insert_idx % size, wrapping back to index 0 when the end of the window is reached. When the buffer is full, newer values overwrite older ones. If size is smaller than the current window, the array is truncated to fit.
    /// <para>环形缓冲区窗口的大小。每个值都会被插入到 insert_idx % size 的位置，当窗口到达末尾时会循环回到索引 0。当缓冲区满时，新值会覆盖旧值。如果大小小于当前窗口，则数组会被截断以适应。</para>
    /// </param>
    /// <param name="values">One or more string values to insert into the ring buffer. Each value is placed at the next position in the ring and the cursor advances accordingly.
    /// <para>一个或多个字符串值要插入到环形缓冲区中。每个值都会被放置到环形缓冲区的下一个位置，同时光标相应地向前移动。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The last index where a value was inserted.
    /// <para>最后一个插入值的索引下标</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arring">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arring">命令文档</see></para>
    /// </remarks>
    public Task<long> ArRingAsync(SharpValue key, long size, SharpValueSpan values, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArRing(key: key, size: size, values: values) | flags, cancellationToken);
    }

    /// <summary>
    /// Iterates existing elements in a range, returning index-value pairs.
    /// <para>Available since: 8.8.0</para>
    /// <para>遍历指定范围内的现有元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="start">The zero-based integer index at which to begin scanning. If start is greater than end, elements are returned in reverse index order.
    /// <para>开始扫描的元素下标, 从0开始. 如果开始大于结束, 则按索引逆序返回元素</para>
    /// </param>
    /// <param name="end">The zero-based integer index at which to stop scanning (inclusive).
    /// <para>停止扫描的元素下标</para>
    /// </param>
    /// <param name="limit">The maximum number of index-value pairs to return. When omitted, all elements in the range are returned.
    /// <para>做多返回元素数量. 如果不设置此参数, 则返回索引范围内的所有元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arscan">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arscan">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<ArItem[]?>
#else
    Task<ArItem[]>
#endif
    ArScanAsync(
        SharpValue key,
        long start,
        long end,
        long? limit = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArScan(key: key, start: start, end: end, limit: limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Iterates existing elements in a range, returning index-value pairs.
    /// <para>Available since: 8.8.0</para>
    /// <para>遍历指定范围内的现有元素</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="range">The index range of the elements to be scanned
    /// <para>要扫描的元素索引范围. 如果开始大于结束, 则按索引逆序返回元素</para>
    /// </param>
    /// <param name="limit">The maximum number of index-value pairs to return. When omitted, all elements in the range are returned.
    /// <para>做多返回元素数量. 如果不设置此参数, 则返回索引范围内的所有元素</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arscan">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arscan">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<ArItem[]?>
#else
    Task<ArItem[]>
#endif
    ArScanAsync(
        SharpValue key,
        RedisArray.Range range,
        long? limit = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArScan(key: key, range: range, limit: limit) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the ARINSERT / ARRING cursor to a specific index.
    /// <para>Available since: 8.8.0</para>
    /// <para>将 ARINSERT / ARRING 游标设置为指定索引</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="index">The zero-based integer index to set as the new insert cursor position for subsequent ARINSERT calls.
    /// <para>要设置为后续 ARINSERT 调用的新插入光标位置的从零开始的整数索引</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the cursor was set, False if the key does not exist.
    /// <para>True: 设置成功. False: Key不存在</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arseek">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arseek">命令文档</see></para>
    /// </remarks>
    public Task<bool> ArSeekAsync(SharpValue key, long index, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArSeek(key: key, index: index) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns metadata about an array.
    /// <para>Available since: 8.8.0</para>
    /// <para>获得数组的元数据</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">The name of the key that holds the array.
    /// <para>数组的Key</para>
    /// </param>
    /// <param name="full">When present, includes per-slice statistics in the reply: the number of dense and sparse slices and their average sizes and fill rates. Raises the complexity from O(1) to O(N) where N is the number of slices.
    /// <para>当存在时，回复中包含每片的统计信息：密集片和稀疏片的数量及其平均大小和填充率。将复杂度从 O(1) 提升到 O(N)，其中 N 为片数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array metadata. Slice statistics are null unless FULL is requested.
    /// <para>数组元数据。FULL 专属统计字段在普通模式下为 null。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/docs/latest/commands/arinfo">Command documentation</see>
    /// <para><see href="https://redis.io/docs/latest/commands/arinfo">命令文档</see></para>
    /// </remarks>
    public Task<ArInfoResult> ArInfoAsync(SharpValue key, bool full = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(ArraysCommands.ArInfo(key, full) | flags, cancellationToken);
    }
}
