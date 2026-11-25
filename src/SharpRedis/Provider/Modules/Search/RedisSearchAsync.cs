using SharpRedis.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Modules;

public sealed partial class RedisSearch
{
    /// <summary>
    /// Create an index with the given specification.
    /// <para>Available in: Search 1.0.0</para>
    /// <para>创建一个 RediSearch 索引，指定数据源、字段 schema 和索引选项</para>
    /// <para>支持此命令的版本: Search 1.0.0</para>
    /// </summary>
    /// <param name="index">Is index name to create.
    /// <para>要创建的索引名称</para>
    /// </param>
    /// <param name="field">Index field
    /// <para>索引字段</para>
    /// </param>
    /// <param name="indexOptions">Index configuration
    /// <para>索引的一些配置</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ft.create">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ft.create">命令文档</see></para>
    /// </remarks>
    public Task<bool> FT_CreateAsync(
        string index,
        Field field,
#if MODERN_NET
        CreateIndexOptions? indexOptions = null,
#else
        CreateIndexOptions indexOptions = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default
    )
    {
        return base.InvokeAsync(RedisSearchCommands.FT_Create(
            index: index,
            field: field,
            indexOptions: indexOptions) | flags, cancellationToken);
    }

    /// <summary>
    /// Create an index with the given specification.
    /// <para>Available in: Search 1.0.0</para>
    /// <para>创建一个 RediSearch 索引，指定数据源、字段 schema 和索引选项</para>
    /// <para>支持此命令的版本: Search 1.0.0</para>
    /// </summary>
    /// <param name="index">Is index name to create.
    /// <para>要创建的索引名称</para>
    /// </param>
    /// <param name="schema">List of index fields, supporting multiple ones
    /// <para>索引字段列表, 支持多个</para>
    /// </param>
    /// <param name="indexOptions">Index configuration
    /// <para>索引的一些配置</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ft.create">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ft.create">命令文档</see></para>
    /// </remarks>
    public Task<bool> FT_CreateAsync(
        string index,
        ReadOnlySpan<Field> schema,
#if MODERN_NET
        CreateIndexOptions? indexOptions = null,
#else
        CreateIndexOptions indexOptions = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default
    )
    {
        return base.InvokeAsync(RedisSearchCommands.FT_Create(
            index: index,
            schema: schema,
            indexOptions: indexOptions) | flags, cancellationToken);
    }

    /// <summary>
    /// Create an index with the given specification.
    /// <para>Available in: Search 1.0.0</para>
    /// <para>创建一个 RediSearch 索引，指定数据源、字段 schema 和索引选项</para>
    /// <para>支持此命令的版本: Search 1.0.0</para>
    /// </summary>
    /// <param name="index">Is index name to create.
    /// <para>要创建的索引名称</para>
    /// </param>
    /// <param name="schema">List of index fields, supporting multiple ones
    /// <para>索引字段列表, 支持多个</para>
    /// </param>
    /// <param name="indexOptions">Index configuration
    /// <para>索引的一些配置</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ft.create">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ft.create">命令文档</see></para>
    /// </remarks>
    public Task<bool> FT_CreateAsync(
        string index,
        ICollection<Field> schema,
#if MODERN_NET
        CreateIndexOptions? indexOptions = null,
#else
        CreateIndexOptions indexOptions = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default
    )
    {
        return base.InvokeAsync(RedisSearchCommands.FT_Create(
            index: index,
            schema: schema,
            indexOptions: indexOptions) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns a list of all existing indexes.
    /// <para>Available in: Search 2.0.0</para>
    /// <para>返回当前数据库中所有 RediSearch 索引的列表</para>
    /// <para>支持此命令的版本: Search 2.0.0</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Array of index names
    /// <para>索引名称数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/ft._list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/ft._list">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string[]?>
#else
    Task<string[]>
#endif
    FT__ListAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(RedisSearchCommands.FT__List() | flags, cancellationToken)!;
    }
}
