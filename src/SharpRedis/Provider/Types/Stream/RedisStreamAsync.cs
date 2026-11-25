using SharpRedis.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public partial class RedisStream
{
    #region XAdd
    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="field">The field name for the stream entry.
    /// <para>消息字段名称</para>
    /// </param>
    /// <param name="value">The value to set in the stream entry.
    /// <para>消息字段的值</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        EntryId id,
        SharpValue field,
        SharpValue value,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            field: field,
            value: value) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (field, value)
    /// <para>消息字段名称和值的元组 (field, value)</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        EntryId id,
        (SharpValue field, SharpValue value) fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValue: fieldValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (Key: field, Value: value)
    /// <para>消息字段名称和值, 键值对格式 (Key: field, Value: value)</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        EntryId id,
        KeyValuePair<SharpValue, SharpValue> fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValue: fieldValue) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="field">The field name for the stream entry.
    /// <para>消息字段名称</para>
    /// </param>
    /// <param name="value">The value to set in the stream entry.
    /// <para>消息字段的值</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        SharpValue field,
        SharpValue value,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAddAsync(
            key: key,
            id: "*",
            field: field,
            value: value,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (field, value)
    /// <para>消息字段名称和值的元组 (field, value)</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        (SharpValue field, SharpValue value) fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAddAsync(
            key: key,
            id: "*",
            fieldValue: fieldValue,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValue">The field name and value for the stream entry. (Key: field, Value: value)
    /// <para>消息字段名称和值, 键值对格式 (Key: field, Value: value)</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        KeyValuePair<SharpValue, SharpValue> fieldValue,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAddAsync(
            key: key,
            id: "*",
            fieldValue: fieldValue,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        EntryId id,
        ICollection<(SharpValue field, SharpValue value)> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueCollection: fieldValueCollection) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        ICollection<(SharpValue field, SharpValue value)> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAddAsync(
            key: key,
            id: "*",
            fieldValueCollection: fieldValueCollection,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        EntryId id,
        ICollection<KeyValuePair<SharpValue, SharpValue>> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueCollection: fieldValueCollection) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueCollection">The collection of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        ICollection<KeyValuePair<SharpValue, SharpValue>> fieldValueCollection,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAddAsync(
            key: key,
            id: "*",
            fieldValueCollection: fieldValueCollection,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        EntryId id,
        ReadOnlySpan<(SharpValue field, SharpValue value)> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueSpan: fieldValueSpan) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        ReadOnlySpan<(SharpValue field, SharpValue value)> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAddAsync(
            key: key,
            id: "*",
            fieldValueSpan: fieldValueSpan,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key.
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="id">The ID to assign to the stream entry
    /// <para>流消息的唯一Id</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        EntryId id,
        ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAdd(
            key: key,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            trimOptions: trimOptions,
            id: id,
            fieldValueSpan: fieldValueSpan) | flags, cancellationToken);
    }

    /// <summary>
    /// Adds an entry using the specified values to the given stream key. auto-generate a unique ID,
    /// <para>Available since: 5.0.0</para>
    /// <para>添加一个消息到指定Stream中, 自动生成消息ID</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="fieldValueSpan">The span of field-value pairs for the stream entry.
    /// <para>消息字段和值的集合</para>
    /// </param>
    /// <param name="nomkstream">Prevents the creation of a new stream if the key does not exist. <b>Available since Redis 6.2.0</b>
    /// <para>如果指定的Key不存在, 则不创建Stream, 默认会创建. <b>自Redis 6.2.0+支持</b></para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="idempotent">Enables idempotent message processing (at-most-once production) to prevent duplicate entries. <b>Available since Redis 8.6.</b>
    /// <para>消息幂等处理. 给消息消息设置一个幂等的唯一Id, 防止同一条消息重复插入 <b>自 Redis 8.6.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The ID of the newly created message
    /// <para>新创建的消息ID</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAdd">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAdd">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<string?>
#else
    Task<string>
#endif
    XAddAsync(
        SharpValue key,
        ReadOnlySpan<KeyValuePair<SharpValue, SharpValue>> fieldValueSpan,
        TrimOptions trimOptions = default,
        bool nomkstream = false,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        IdempotentArg idempotent = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return this.XAddAsync(
            key: key,
            id: "*",
            fieldValueSpan: fieldValueSpan,
            trimOptions: trimOptions,
            nomkstream: nomkstream,
            delPolicy: delPolicy,
            idempotent: idempotent,
            flags: flags,
            cancellationToken: cancellationToken);
    }
    #endregion

    /// <summary>
    /// Returns the number of entries inside a stream. If the specified key does not exist the command returns zero, as if the stream was empty. However note that unlike other Redis types, zero-length streams are possible, so you should call TYPE or EXISTS in order to check if a key exists or not.
    /// <para>Available since: 5.0.0</para>
    /// <para>获得消息条目数量, 如果Key不存在返回0. 注意, Stream和其它类型不一样, 0条消息的Stream是可以存在的, 因此如需判断Stream是否存在应该使用 <b>[EXISTS]</b> 命令</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries of the stream at key.
    /// <para>消息条目数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XLen">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XLen">命令文档</see></para>
    /// </remarks>
    public Task<long> XLenAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.InvokeAsync(StreamCommands.XLen(key) | flags, cancellationToken);

    /// <summary>
    /// Removes the specified entries from a stream, and returns the number of entries deleted.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息Id删除消息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="id">One message ID to delete.
    /// <para>要删除消息的ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries that were deleted.
    /// <para>已删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDel">命令文档</see></para>
    /// </remarks>
    public Task<int> XDelAsync(SharpValue key, EntryId id, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XDel(key, id) | flags, cancellationToken);
    }

    /// <summary>
    /// Removes the specified entries from a stream, and returns the number of entries deleted.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息Id删除消息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="ids">One or more message IDs to delete.
    /// <para>要删除消息的ID, 一个或多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries that were deleted.
    /// <para>已删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDel">命令文档</see></para>
    /// </remarks>
    public Task<int> XDelAsync(SharpValue key, ICollection<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XDel(key, ids) | flags, cancellationToken);
    }

    /// <summary>
    /// Removes the specified entries from a stream, and returns the number of entries deleted.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息Id删除消息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">Stream key</param>
    /// <param name="ids">One or more message IDs to delete.
    /// <para>要删除消息的ID, 一个或多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries that were deleted.
    /// <para>已删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDel">命令文档</see></para>
    /// </remarks>
    public Task<int> XDelAsync(SharpValue key, ReadOnlySpan<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XDel(key, ids) | flags, cancellationToken);
    }

    /// <inheritdoc cref="XAck(SharpValue, string, EntryId, CommandFlags, CancellationToken)"/>
    public Task<int> XAckAsync(SharpValue key, string group, EntryId id, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAck(key, group, id) | flags, cancellationToken);
    }

    /// <inheritdoc cref="XAck(SharpValue, string, ICollection{EntryId}, CommandFlags, CancellationToken)"/>
    public Task<int> XAckAsync(SharpValue key, string group, ICollection<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAck(key, group, ids) | flags, cancellationToken);
    }

    /// <inheritdoc cref="XAck(SharpValue, string, ReadOnlySpan{EntryId}, CommandFlags, CancellationToken)"/>
    public Task<int> XAckAsync(SharpValue key, string group, ReadOnlySpan<EntryId> ids, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAck(key, group, ids) | flags, cancellationToken);
    }

    /// <summary>
    /// Create a new consumer group uniquely identified by groupname for the stream stored at key
    /// <para>Available since: 5.0.0</para>
    /// <para>创建一个新的消费者组</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>消息流Key</para>
    /// </param>
    /// <param name="group">The name of the consumer group to create.
    /// <para>要创建的消费者组名称</para>
    /// </param>
    /// <param name="id">The ID of the last delivered message the group starts reading after. Use $ for the last message currently in the stream, or 0 to read from the start.
    /// <para>起始消息ID, $表示最新，0表示从头</para>
    /// </param>
    /// <param name="mkStream">Create the stream as an empty stream if it does not already exist.
    /// <para>如果消息流不存在, 是否创建一个空的消息流</para>
    /// </param>
    /// <param name="entriesRead">Set the group's initial entries-read counter, used to compute its lag.
    /// <para><b>Starting from version 7.0.0 of Redis, this parameter has been supported.</b></para>
    /// <para>初始化消费者组的逻辑已读取消息计数器</para>
    /// <para><b>Redis 7.0.0及以上版本才支持此参数</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-create">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-create">命令文档</see></para>
    /// </remarks>
    public Task<bool> XGroupCreateAsync(
        SharpValue key,
        string group,
        EntryId id,
        bool mkStream = false,
        long? entriesRead = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XGroupCreate(key, group, id, mkStream, entriesRead) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The key to read from
    /// <para>读取的Stream Key</para>
    /// </param>
    /// <param name="id">For each stream, entries with an ID greater than the given one are returned. Use $ to read only entries added after the call.
    /// <para>起始消息 ID（$ 最新，0-0 从头）</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public Task<XReadResult?> XReadAsync(
        SharpValue key,
        EntryId id,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XRead(
            key: key,
            id: id,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadAsync(ICollection<KeyValuePair<SharpValue, EntryId>> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadAsync(
        ReadOnlySpan<KeyValuePair<SharpValue, EntryId>> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadAsync(ICollection<(SharpValue key, EntryId id)> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// Read data from one streams, returning only entries with an ID greater than the last received ID reported by the caller. This command has an option to block if items are not available, in a similar fashion to BRPOP or BZPOPMIN and others.
    /// <para>Available since: 5.0.0</para>
    /// <para>从一个 Stream 中读取消息。支持阻塞读取（BLOCK）、从指定 ID 之后读取。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array where each element is an array composed of a two elements containing the key name and the entries reported for that key.
    /// <para>一个数组, 每项都包含返回消息的Stream key和消息条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRead">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRead">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadAsync(ReadOnlySpan<(SharpValue key, EntryId id)> keysIds,
        long? count = null,
        ulong? block = null,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XRead(
            keysIds: keysIds,
            count: count,
            block: block,
            maxCount: maxCount,
            maxSize: maxSize) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The key to read from
    /// <para>读取的Stream Key</para>
    /// </param>
    /// <param name="id">Use > to read messages never delivered to any other consumer, or a specific ID to read from this consumer's pending list.
    /// <para>> 表示从未消费的消息，其他 ID 表示已投递的消息</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public Task<XReadResult?> XReadGroupAsync(
        SharpValue key,
        EntryId id,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XReadGroup(
            key: key,
            id: id,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadGroupAsync(
        ICollection<KeyValuePair<SharpValue, EntryId>> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadGroupAsync(
        ICollection<(SharpValue key, EntryId id)> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadGroupAsync(
        ReadOnlySpan<KeyValuePair<SharpValue, EntryId>> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>XREADGROUP</b> command is a special version of the <b>XREAD</b> command with support for consumer groups.
    /// <para>Available since: 5.0.0</para>
    /// <para><b>XREADGROUP</b> 通过消费者组读取 Stream 中的消息。消息被读取后进入 PEL（待确认列表），需用 XACK 确认。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="keysIds">The key to be read and the starting ID. One or more
    /// <para>要读取的Key和起始ID, 一个或多个</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The consumer name.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return per stream.
    /// <para>最多返回的消息数量</para>
    /// </param>
    /// <param name="maxCount"><b>Added in Redis 8.10.</b> The maximum number of entries to return in total across all streams named in the command
    /// <para><b>Redis 8.10 新增。</b>指定的所有流返回的最大条目数。</para>
    /// </param>
    /// <param name="maxSize"><b>Added in Redis 8.10.</b> The maximum size, in bytes, of the reply across all streams named in the command.
    /// <para><b>Redis 8.10 新增。</b>指定的所有流的返回的最大字节数</para>
    /// </param>
    /// <param name="block">Block for up to this many milliseconds if no entries are available. 0 blocks indefinitely.
    /// <para>如果没有可用消息，则阻塞最多指定毫秒数。0无限期阻塞。</para>
    /// </param>
    /// <param name="claim"><b>Added in Redis 8.4.0.</b> Claim messages that have been idle for at least this long (in milliseconds) before reading.
    /// <para><b>Redis 8.4.0 新增。</b> 读取至少闲置了一定时间的消息, 单位: 毫秒</para>
    /// </param>
    /// <param name="noack">Do not add the read messages to the PEL; treat them as acknowledged immediately.
    /// <para>是否视为已确认消息, 不进入PEL(待确认列表)</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XReadGroup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XReadGroup">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XReadResult[]?>
#else
    Task<XReadResult[]>
#endif
    XReadGroupAsync(
        ReadOnlySpan<(SharpValue key, EntryId id)> keysIds,
        string group,
        string consumer,
        long? count = null,
        ulong? block = null,
        ulong? claim = null,
        bool noack = false,
        ulong? maxCount = null,
        ulong? maxSize = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XReadGroup(
            keysIds: keysIds,
            group: group,
            consumer: consumer,
            count: count,
            maxCount: maxCount,
            maxSize: maxSize,
            block: block,
            claim: claim,
            noack: noack) | flags, cancellationToken);
    }

    /// <summary>
    /// Acknowledges and conditionally deletes one entries (messages) for a stream consumer group at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>有条件的确认并删除指定Stream中的一个消息</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The name of the consumer group.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="id">One stream entry ID to acknowledge and delete
    /// <para>要确认并删除的Stream消息ID</para>
    /// </param>
    /// <param name="delPolicy">pecifies how to handle consumer group references when acknowledging and deleting entries.
    /// <para>指定在确认和删除消息时如何处理消费者组的引用</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAckDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAckDel">命令文档</see></para>
    /// </remarks>
    public Task<DeleteResult> XAckDelAsync(
        SharpValue key,
        string group,
        EntryId id,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAckDel(
            key: key,
            group: group,
            delPolicy: delPolicy,
            id: id) | flags, cancellationToken);
    }

    /// <summary>
    /// Acknowledges and conditionally deletes one or multiple entries (messages) for a stream consumer group at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>有条件的确认并删除指定Stream中的一个或多个消息</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The name of the consumer group.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to acknowledge and delete
    /// <para>要确认并删除的Stream消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">pecifies how to handle consumer group references when acknowledging and deleting entries.
    /// <para>指定在确认和删除消息时如何处理消费者组的引用</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAckDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAckDel">命令文档</see></para>
    /// </remarks>
    public Task<DeleteResult[]> XAckDelAsync(
        SharpValue key,
        string group,
        ICollection<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAckDel(
            key: key,
            group: group,
            delPolicy: delPolicy,
            ids: ids) | flags, cancellationToken);
    }

    /// <summary>
    /// Acknowledges and conditionally deletes one or multiple entries (messages) for a stream consumer group at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>有条件的确认并删除指定Stream中的一个或多个消息</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The name of the consumer group.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to acknowledge and delete
    /// <para>要确认并删除的Stream消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">pecifies how to handle consumer group references when acknowledging and deleting entries.
    /// <para>指定在确认和删除消息时如何处理消费者组的引用</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAckDel">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAckDel">命令文档</see></para>
    /// </remarks>
    public Task<DeleteResult[]> XAckDelAsync(
        SharpValue key,
        string group,
        ReadOnlySpan<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAckDel(
            key: key,
            group: group,
            delPolicy: delPolicy,
            ids: ids) | flags, cancellationToken);
    }

    /// <summary>
    /// The command returns the stream entries matching a given range of IDs. The range is specified by a minimum and maximum ID. All the entries having an ID between the two specified or exactly one of the two IDs specified (closed interval) are returned.
    /// <para>Available since: 5.0.0</para>
    /// <para>根据指定的消息ID范围, 返回区间里面的消息. 包含ID值相等的也会返回</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="start">The start of the ID range, inclusive. Use - for the smallest ID.
    /// <para>开始扫描的消息ID, 可使用 - 表示最小的ID</para>
    /// </param>
    /// <param name="end">The end of the ID range, inclusive. Use + for the greatest ID.
    /// <para>结束扫描的消息ID, 可使用 + 表示最大的ID</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return.
    /// <para>最多返回消息条数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of stream entries with IDs matching the specified range.
    /// <para>一个包含消息ID和消息内容的数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<EntryItem[]?>
#else
    Task<EntryItem[]>
#endif
    XRangeAsync(
        SharpValue key,
        EntryId start,
        EntryId end,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XRange(
            key: key,
            start: start,
            end: end,
            count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// This command works like XRANGE, but returns entries in reverse order. Specify the end ID first, then the start ID.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回 Stream 中指定 ID 范围内的消息，按 ID 从大到小排序。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="start">The start of the ID range, inclusive. Use - for the smallest ID.
    /// <para>开始扫描的消息ID, 可使用 - 表示最小的ID</para>
    /// </param>
    /// <param name="end">The end of the ID range, inclusive. Entries are returned in reverse order. Use + for the greatest ID.
    /// <para>结束扫描的消息ID, 可使用 + 表示最大的ID</para>
    /// </param>
    /// <param name="count">The maximum number of entries to return.
    /// <para>最多返回消息条数</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>A array of stream entries with IDs matching the specified range.
    /// <para>一个包含消息ID和消息内容的数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XRevRange">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XRevRange">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<EntryItem[]?>
#else
    Task<EntryItem[]>
#endif
    XRevRangeAsync(
        SharpValue key,
        EntryId end,
        EntryId start,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XRevRange(
            key: key,
            end: end,
            start: start,
            count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// This command transfers ownership of pending stream entries that match the specified criteria. Conceptually, XAUTOCLAIM is equivalent to calling XPENDING and then XCLAIM, but provides a more straightforward way to deal with message delivery failures via SCAN-like semantics.
    /// <para>Available since: 6.2.0</para>
    /// <para>此命令会转移符合指定条件的待处理流条目的所有权。从概念上讲， XAUTOCLAIM 等同于先调用 XPENDING 再调用 XCLAIM ，但它提供了一种更直接的方式来处理类似 SCAN 语义的消息传递失败。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stre key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>仅认领闲置至少达到多少时长（以毫秒为单位）的消息。</para>
    /// </param>
    /// <param name="startId">The message ID to start scanning from. Use 0 to start at the beginning of the PEL.
    /// <para>开始扫描的消息ID, 使用0表示从头开始</para>
    /// </param>
    /// <param name="count">The number of messages to attempt to claim per call. Defaults to 100.
    /// <para>尝试认领的消息最多数量, 默认为100</para>
    /// </param>
    /// <param name="justid">Return only the IDs of the claimed messages, without their fields, and do not increment their retry counters.
    /// <para>仅返回已声明消息的 ID，不包含其字段，并且不增加其重试计数器。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XAutoClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XAutoClaim">命令文档</see></para>
    /// </remarks>
    public Task<XAutoClaimResult> XAutoClaimAsync(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        EntryId startId,
        long? count = null,
        bool justid = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XAutoClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            start: startId,
            count: count,
            justid: justid) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns summary information about the pending entries in a consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>获取指定消费者组中待处理消息（PEL）的摘要信息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>Stream键</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Summary information about pending entries.
    /// <para>待处理消息总数、最小ID、最大ID及各消费者的待处理数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XPending">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XPending">命令文档</see></para>
    /// </remarks>
    public Task<XPendingSummary> XPendingAsync(
        SharpValue key,
        string group,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XPending(
            key: key,
            group: group,
            options: default) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns detailed information about pending entries in a consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>获取指定消费者组中待处理消息（PEL）的详细信息</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>Stream键</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="options">Represents the options for querying detailed pending entries using XPENDING.
    /// <para>Encapsulates the ID range, maximum result count, idle time filter, and consumer filter.</para>
    /// <para>表示 Redis XPENDING 命令查询待处理消息详细信息的选项。</para>
    /// <para>封装消息ID范围、最大返回数量、最小闲置时间以及消费者过滤条件。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Summary information about pending entries.
    /// <para>待处理消息总数、最小ID、最大ID及各消费者的待处理数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XPending">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XPending">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<XPendingItem[]?>
#else
    Task<XPendingItem[]>
#endif
    XPendingAsync(
        SharpValue key,
        string group,
        PendingOptions options,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        if (!options.HasValue) throw new ArgumentException("XPENDING command requires options to be specified for detailed information retrieval.", nameof(options));
        return base.InvokeAsync(StreamCommands.XPending(
            key: key,
            group: group,
            options: options) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the IDMP (Idempotent Message Processing) configuration parameters for a stream. This command configures how long idempotent IDs are retained and the maximum number of idempotent IDs tracked per producer.
    /// <para>Available since: 8.6.0</para>
    /// <para>为 Stream 配置 IDMP（Idempotent Message Processing，幂等消息处理）参数，控制幂等 ID 的保留时长和容量上限。</para>
    /// <para>支持此命令的Redis版本, 8.6.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key. The stream must already exist.
    /// <para>Stream Key, 该Key必须存在, 否则异常</para>
    /// </param>
    /// <param name="duration">Sets the duration, in seconds, for which each idempotent ID (iid) is retained in the stream's IDMP map. Valid range: 1-86,400 seconds. Default: 100 seconds.
    /// <para>幂等 ID 保留时长（秒），范围 1~86400，默认 100</para>
    /// </param>
    /// <param name="maxsize">Sets the maximum number of most recent idempotent IDs kept for each producer in the stream's IDMP map. Valid range: 1-10,000 entries. Default: 100 entries.
    /// <para>每个生产者保留的最大幂等 ID 数，范围 1~10000，默认 100</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if the configuration was set successfully.
    /// <para>设置成功返回 true，否则返回 false</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XCfgSet">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XCfgSet">命令文档</see></para>
    /// </remarks>
    public Task<bool> XCfgSetAsync(
        SharpValue key,
        int? duration = 100,
        int? maxsize = 100,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XCfgSet(
            key: key,
            duration: duration,
            maxsize: maxsize) | flags, cancellationToken);
    }

    /// <summary>
    /// XCLAIM is used to change (or obtain) the ownership of a certain message within the consumer group, and reassign the message to the specified consumer.
    /// <para>Available since: 5.0.0</para>
    /// <para>XCLAIM 用于改变（或获取）消费者组中某条消息的所有权，将消息重新分配给指定的消费者。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>目标消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>最小空闲时间（毫秒），消息空闲时间必须大于此值才能被认领</para>
    /// </param>
    /// <param name="id">One message IDs to claim.
    /// <para>消息ID</para>
    /// </param>
    /// <param name="idle">Set the idle time (last time it was delivered) of the message. If IDLE is not specified, an idle time of 0 is assumed, that is, the time count is reset because the message now has a new owner trying to process it.
    /// <para>设置消息的空闲时间（毫秒）</para>
    /// </param>
    /// <param name="time">The same as IDLE but sets the idle time to a specific Unix time (in milliseconds) instead of a relative amount of milliseconds.
    /// <para>设置消息的空闲时间为指定的 Unix 时间戳（毫秒）</para>
    /// </param>
    /// <param name="retryCount">Set the retry counter to the specified value
    /// <para>设置消息的重试计数</para>
    /// </param>
    /// <param name="force">Create the pending message entry in the PEL even if the specified IDs are not already in the PEL assigned to a different client.
    /// <para>强制认领，即使消息不在 PEL 中</para>
    /// </param>
    /// <param name="justid">Return just an array of IDs of messages successfully claimed, without returning the actual messages. Using this option means the retry counter is not incremented.
    /// <para>仅返回消息 ID，不返回内容, 且重试计数也不会增加</para>
    /// </param>
    /// <param name="lastid">Update the consumer group's last-delivered ID to the given ID.
    /// <para>设置消费者组的最后投递 ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The claimed message entry. When <paramref name="justid"/> is true, only the message ID is populated and the field/value mapping is null. If no message is claimed, a default (empty) entry is returned.
    /// <para>被认领的消息条目。当 justid 为 true 时, 仅填充消息 ID, 不包含字段值; 未认领到任何消息时返回默认（空）条目</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XClaim">命令文档</see></para>
    /// </remarks>
    public Task<EntryItem?> XClaimAsync(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        EntryId id,
        ulong? idle = null,
        SharpExpiry? time = null,
        ulong? retryCount = null,
        bool force = false,
        bool justid = false,
        EntryId lastid = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            id: id,
            idle: idle,
            time: time,
            retryCount: retryCount,
            force: force,
            justid: justid,
            lastid: lastid) | flags, cancellationToken);
    }

    /// <summary>
    /// XCLAIM is used to change (or obtain) the ownership of one or more messages within the consumer group, and reassign the messages to the specified consumer.
    /// <para>Available since: 5.0.0</para>
    /// <para>XCLAIM 用于改变（或获取）消费者组中一条或多条消息的所有权，将消息重新分配给指定的消费者。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>目标消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>最小空闲时间（毫秒），消息空闲时间必须大于此值才能被认领</para>
    /// </param>
    /// <param name="ids">One or more message IDs to claim.
    /// <para>要认领的消息ID, 一个或多个</para>
    /// </param>
    /// <param name="idle">Set the idle time (last time it was delivered) of the message. If IDLE is not specified, an idle time of 0 is assumed, that is, the time count is reset because the message now has a new owner trying to process it.
    /// <para>设置消息的空闲时间（毫秒）</para>
    /// </param>
    /// <param name="time">The same as IDLE but sets the idle time to a specific Unix time (in milliseconds) instead of a relative amount of milliseconds.
    /// <para>设置消息的空闲时间为指定的 Unix 时间戳（毫秒）</para>
    /// </param>
    /// <param name="retryCount">Set the retry counter to the specified value
    /// <para>设置消息的重试计数</para>
    /// </param>
    /// <param name="force">Create the pending message entry in the PEL even if the specified IDs are not already in the PEL assigned to a different client.
    /// <para>强制认领，即使消息不在 PEL 中</para>
    /// </param>
    /// <param name="justid">Return just an array of IDs of messages successfully claimed, without returning the actual messages. Using this option means the retry counter is not incremented.
    /// <para>仅返回消息 ID，不返回内容, 且重试计数也不会增加</para>
    /// </param>
    /// <param name="lastid">Update the consumer group's last-delivered ID to the given ID.
    /// <para>设置消费者组的最后投递 ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of the claimed message entries. When <paramref name="justid"/> is true, each entry only contains its message ID and the field/value mapping is null. An empty array is returned if no message is claimed.
    /// <para>被认领的消息条目数组。当 justid 为 true 时, 每个条目仅包含消息 ID, 不包含字段值; 未认领到任何消息时返回空数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XClaim">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<EntryItem[]?>
#else
    Task<EntryItem[]>
#endif
    XClaimAsync(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        ICollection<EntryId> ids,
        ulong? idle = null,
        SharpExpiry? time = null,
        ulong? retryCount = null,
        bool force = false,
        bool justid = false,
        EntryId lastid = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            ids: ids,
            idle: idle,
            time: time,
            retryCount: retryCount,
            force: force,
            justid: justid,
            lastid: lastid) | flags, cancellationToken);
    }

    /// <summary>
    /// XCLAIM is used to change (or obtain) the ownership of one or more messages within the consumer group, and reassign the messages to the specified consumer.
    /// <para>Available since: 5.0.0</para>
    /// <para>XCLAIM 用于改变（或获取）消费者组中一条或多条消息的所有权，将消息重新分配给指定的消费者。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>要操作的Stream key</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer that will own the claimed messages.
    /// <para>目标消费者名称</para>
    /// </param>
    /// <param name="minIdleTime">Claim only messages that have been idle for at least this long, in milliseconds.
    /// <para>最小空闲时间（毫秒），消息空闲时间必须大于此值才能被认领</para>
    /// </param>
    /// <param name="ids">One or more message IDs to claim.
    /// <para>要认领的消息ID, 一个或多个</para>
    /// </param>
    /// <param name="idle">Set the idle time (last time it was delivered) of the message. If IDLE is not specified, an idle time of 0 is assumed, that is, the time count is reset because the message now has a new owner trying to process it.
    /// <para>设置消息的空闲时间（毫秒）</para>
    /// </param>
    /// <param name="time">The same as IDLE but sets the idle time to a specific Unix time (in milliseconds) instead of a relative amount of milliseconds.
    /// <para>设置消息的空闲时间为指定的 Unix 时间戳（毫秒）</para>
    /// </param>
    /// <param name="retryCount">Set the retry counter to the specified value
    /// <para>设置消息的重试计数</para>
    /// </param>
    /// <param name="force">Create the pending message entry in the PEL even if the specified IDs are not already in the PEL assigned to a different client.
    /// <para>强制认领，即使消息不在 PEL 中</para>
    /// </param>
    /// <param name="justid">Return just an array of IDs of messages successfully claimed, without returning the actual messages. Using this option means the retry counter is not incremented.
    /// <para>仅返回消息 ID，不返回内容, 且重试计数也不会增加</para>
    /// </param>
    /// <param name="lastid">Update the consumer group's last-delivered ID to the given ID.
    /// <para>设置消费者组的最后投递 ID</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>An array of the claimed message entries. When <paramref name="justid"/> is true, each entry only contains its message ID and the field/value mapping is null. An empty array is returned if no message is claimed.
    /// <para>被认领的消息条目数组。当 justid 为 true 时, 每个条目仅包含消息 ID, 不包含字段值; 未认领到任何消息时返回空数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XClaim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XClaim">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<EntryItem[]?>
#else
    Task<EntryItem[]>
#endif
    XClaimAsync(
        SharpValue key,
        string group,
        string consumer,
        ulong minIdleTime,
        ReadOnlySpan<EntryId> ids,
        ulong? idle = null,
        SharpExpiry? time = null,
        ulong? retryCount = null,
        bool force = false,
        bool justid = false,
        EntryId lastid = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XClaim(
            key: key,
            group: group,
            consumer: consumer,
            minIdleTime: minIdleTime,
            ids: ids,
            idle: idle,
            time: time,
            retryCount: retryCount,
            force: force,
            justid: justid,
            lastid: lastid) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes one entries from the stream at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>从 Stream 中删除一个消息条目，并提供对消费组 PEL 引用的精细化控制。</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="id">One stream entry IDs to delete
    /// <para>要删除的消息条目 ID</para>
    /// </param>
    /// <param name="delPolicy">The deletion policy to use.
    /// <para>删除策略</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDelEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDelEx">命令文档</see></para>
    /// </remarks>
    public Task<DeleteResult> XDelExAsync(
        SharpValue key,
        EntryId id,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XDelEx(
            key: key,
            id: id,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes one or multiple entries from the stream at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>从 Stream 中删除一个或多个消息条目，并提供对消费组 PEL 引用的精细化控制。</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to delete
    /// <para>要删除的消息条目 ID, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">The deletion policy to use.
    /// <para>删除策略</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDelEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDelEx">命令文档</see></para>
    /// </remarks>
    public Task<DeleteResult[]> XDelExAsync(
        SharpValue key,
        ICollection<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XDelEx(
            key: key,
            ids: ids,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Deletes one or multiple entries from the stream at the specified key.
    /// <para>Available since: 8.2.0</para>
    /// <para>从 Stream 中删除一个或多个消息条目，并提供对消费组 PEL 引用的精细化控制。</para>
    /// <para>支持此命令的Redis版本, 8.2.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="ids">One or more stream entry IDs to delete
    /// <para>要删除的消息条目 ID, 一个或多个</para>
    /// </param>
    /// <param name="delPolicy">The deletion policy to use.
    /// <para>删除策略</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XDelEx">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XDelEx">命令文档</see></para>
    /// </remarks>
    public Task<DeleteResult[]> XDelExAsync(
        SharpValue key,
        ReadOnlySpan<EntryId> ids,
        DeletionPolicy delPolicy = DeletionPolicy.None,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XDelEx(
            key: key,
            ids: ids,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Create a consumer named consumername in the consumer group groupname of the stream that's stored at key.
    /// <para>Available since: 6.2.0</para>
    /// <para>在指定的消费者组中创建一个新的消费者</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer to create.
    /// <para>要创建的消费者名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of created consumers, either 0 or 1. <b>0 -> False, 1 -> True</b>
    /// <para>成功创建消费者的数量, 1个或0个. <b>0 -> False, 1 -> True</b></para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-createconsumer">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-createconsumer">命令文档</see></para>
    /// </remarks>
    public Task<bool> XGroupCreateConsumerAsync(SharpValue key, string group, string consumer, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XGroupCreateConsumer(
            key: key,
            group: group,
            consumer: consumer) | flags, cancellationToken);
    }

    /// <summary>
    /// The XGROUP DELCONSUMER command deletes a consumer from the consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>从 Stream 消费者组中删除一个消费者</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="consumer">The name of the consumer to delete.
    /// <para>要删除的消费者名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of pending messages the consumer had before it was deleted.
    /// <para>消费者被删除前待处理消息的数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-delconsumer">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-delconsumer">命令文档</see></para>
    /// </remarks>
    public Task<int> XGroupDelConsumerAsync(SharpValue key, string group, string consumer, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XGroupDelConsumer(
            key: key,
            group: group,
            consumer: consumer) | flags, cancellationToken);
    }

    /// <summary>
    /// The XGROUP DESTROY command completely destroys a consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>销毁一个 Stream 消费者组</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The name of the consumer group to destroy.
    /// <para>要销毁的消费者组名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of destroyed consumer groups, either 0 or 1. <b>0 -> False, 1 -> True</b>
    /// <para>被摧毁的消费者组的数量， 0个 或 1个. <b>0 -> False, 1 -> True</b></para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-destroy">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-destroy">命令文档</see></para>
    /// </remarks>
    public Task<bool> XGroupDestroyAsync(SharpValue key, string group, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XGroupDestroy(
           key: key,
           group: group) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the last-delivered ID of a Stream consumer group.
    /// <para>Available since: 5.0.0</para>
    /// <para>设置 Stream 消费者组最后投递的消息 ID。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.
    /// <para>Stream 键名</para>
    /// </param>
    /// <param name="group">The consumer group name.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="id">The last-delivered ID or "$".
    /// <para>最后投递的消息 ID，也可以使用 "$"。</para>
    /// </param>
    /// <param name="entriesRead">
    /// The logical number of entries read by the consumer group.
    /// <para>Available since: 7.0.0</para>
    /// <para>消费者组逻辑已读取消息数量，用于计算 Lag。</para>
    /// <para>Null 表示不指定此参数。</para>
    /// <para>支持此参数的Redis版本, 7.0.0+</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// True if the last-delivered ID was updated successfully.
    /// <para>成功更新返回 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xgroup-setid">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xgroup-setid">命令文档</see></para>
    /// </remarks>
    public Task<bool> XGroupSetIDAsync(
        SharpValue key,
        string group,
        EntryId id,
        long? entriesRead = null,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XGroupSetID(
            key: key,
            group: group,
            id: id,
            entriesRead: entriesRead) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns the list of consumers that belong to the groupname consumer group of the stream stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回指定 Stream 消费组中所有消费者的详细信息，包括 pending 数量、idle 时间等。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="group">The consumer group name.
    /// <para>消费组名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xinfo-consumers">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xinfo-consumers">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<ConsumerInfo[]?>
#else
    Task<ConsumerInfo[]>
#endif
    XInfoConsumersAsync(SharpValue key, string group, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XInfoConsumers(
            key: key,
            group: group) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns the list of all consumer groups of the stream stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>获得指定 Stream 的所有消费组列表及其状态信息，包括消费者数量、pending 数量和消费延迟（lag）。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xinfo-groups">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xinfo-groups">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Task<GroupInfo[]?>
#else
    Task<GroupInfo[]>
#endif
    XInfoGroupsAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XInfoGroups(key: key) | flags, cancellationToken);
    }

    /// <summary>
    /// This command returns information about the stream stored at key.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回指定 Stream 的详细统计信息，包括长度、radix tree 结构、消费组数量和首尾条目等。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="key">The stream key.</param>
    /// <param name="full">Return the full stream state, including all entries and detailed PEL information, instead of a summary.
    /// <para><b>Starting with Redis version 6.0.0</b></para>
    /// <para>返回扩展信息，包括 entries 数组和各消费组的完整状态</para>
    /// <para><b>自Redis 6.0.0+支持该参数</b></para>
    /// </param>
    /// <param name="count">With FULL, the number of entries to return. 0 returns all entries; the default is 10.
    /// <para><b>Starting with Redis version 6.0.0</b></para>
    /// <para>FULL 模式下限制 entries 和 pending 返回数量，默认 10；0 表示全部</para>
    /// <para><b>自Redis 6.0.0+支持该参数</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xinfo-stream">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xinfo-stream">命令文档</see></para>
    /// </remarks>
    public Task<StreamInfo> XInfoStreamAsync(
        SharpValue key,
        bool full = false,
        long? count = null,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XInfoStream(key: key, full: full, count: count) | flags, cancellationToken);
    }

    /// <summary>
    /// In the context of a stream consumer group, this command allows a consumer to explicitly release pending messages back to the group's Pending Entries List (PEL) without acknowledging them.
    /// <para>Available since: 8.8.0</para>
    /// <para>将消费者组中的已确认消息重新标记为待处理状态，将其重新放回 PEL（Pending Entries List）中</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Name of the stream.</param>
    /// <param name="group">Name of the consumer group.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="id">The stream entry ID to be released
    /// <para>要处理的消息ID</para>
    /// </param>
    /// <param name="mode">Controls the delivery counter adjustment
    /// <para>否定确认模式，用于控制消息释放后投递次数的变化</para>
    /// </param>
    /// <param name="retryCount">Directly sets the delivery counter to the specified value, overriding the mode-based adjustment. This gives explicit control over the retry counter regardless of the mode selected.
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>直接设置消息投递次数，覆盖 NACK 模式默认的计数调整行为。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要指定。</i></b></para>
    /// </param>
    /// <param name="force">Creates new unowned PEL entries for IDs that are not already in the group PEL. Each entry must exist in the stream. When FORCE creates an entry, the delivery counter is set to 0 (or to RETRYCOUNT if specified, or to LLONG_MAX if mode is FATAL).
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>当消息存在于 Stream 中、但不在当前消费组 PEL 中时，强制创建无消费者所有权的 PEL 记录。不会创建新的 Stream 消息。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要启用。</i></b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of messages successfully released back to the group PEL. Messages that are not in the consumer group PEL will not be counted.
    /// <para>成功恢复为待处理状态的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XNAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XNAck">命令文档</see></para>
    /// </remarks>
    public Task<long> XNAckAsync(
        SharpValue key,
        string group,
        EntryId id,
        NackMode mode,
        long? retryCount = null,
        bool force = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XNAck(
            key: key,
            group: group,
            mode: mode,
            id: id,
            retryCount: retryCount,
            force: force) | flags, cancellationToken);
    }

    /// <summary>
    /// In the context of a stream consumer group, this command allows a consumer to explicitly release pending messages back to the group's Pending Entries List (PEL) without acknowledging them.
    /// <para>Available since: 8.8.0</para>
    /// <para>将消费者组中的已确认消息重新标记为待处理状态，将其重新放回 PEL（Pending Entries List）中</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Name of the stream.</param>
    /// <param name="group">Name of the consumer group.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="ids">Block of arguments that specifies the message IDs to release
    /// <para>要处理的消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="mode">Controls the delivery counter adjustment
    /// <para>否定确认模式，用于控制消息释放后投递次数的变化</para>
    /// </param>
    /// <param name="retryCount">Directly sets the delivery counter to the specified value, overriding the mode-based adjustment. This gives explicit control over the retry counter regardless of the mode selected.
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>直接设置消息投递次数，覆盖 NACK 模式默认的计数调整行为。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要指定。</i></b></para>
    /// </param>
    /// <param name="force">Creates new unowned PEL entries for IDs that are not already in the group PEL. Each entry must exist in the stream. When FORCE creates an entry, the delivery counter is set to 0 (or to RETRYCOUNT if specified, or to LLONG_MAX if mode is FATAL).
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>当消息存在于 Stream 中、但不在当前消费组 PEL 中时，强制创建无消费者所有权的 PEL 记录。不会创建新的 Stream 消息。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要启用。</i></b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of messages successfully released back to the group PEL. Messages that are not in the consumer group PEL will not be counted.
    /// <para>成功恢复为待处理状态的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XNAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XNAck">命令文档</see></para>
    /// </remarks>
    public Task<long> XNAckAsync(
        SharpValue key,
        string group,
        ICollection<EntryId> ids,
        NackMode mode,
        long? retryCount = null,
        bool force = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XNAck(
            key: key,
            group: group,
            mode: mode,
            ids: ids,
            retryCount: retryCount,
            force: force) | flags, cancellationToken);
    }

    /// <summary>
    /// In the context of a stream consumer group, this command allows a consumer to explicitly release pending messages back to the group's Pending Entries List (PEL) without acknowledging them.
    /// <para>Available since: 8.8.0</para>
    /// <para>将消费者组中的已确认消息重新标记为待处理状态，将其重新放回 PEL（Pending Entries List）中</para>
    /// <para>支持此命令的Redis版本, 8.8.0+</para>
    /// </summary>
    /// <param name="key">Name of the stream.</param>
    /// <param name="group">Name of the consumer group.
    /// <para>消费者组名称</para>
    /// </param>
    /// <param name="ids">Block of arguments that specifies the message IDs to release
    /// <para>要处理的消息ID集合, 一个或多个</para>
    /// </param>
    /// <param name="mode">Controls the delivery counter adjustment
    /// <para>否定确认模式，用于控制消息释放后投递次数的变化</para>
    /// </param>
    /// <param name="retryCount">Directly sets the delivery counter to the specified value, overriding the mode-based adjustment. This gives explicit control over the retry counter regardless of the mode selected.
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>直接设置消息投递次数，覆盖 NACK 模式默认的计数调整行为。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要指定。</i></b></para>
    /// </param>
    /// <param name="force">Creates new unowned PEL entries for IDs that are not already in the group PEL. Each entry must exist in the stream. When FORCE creates an entry, the delivery counter is set to 0 (or to RETRYCOUNT if specified, or to LLONG_MAX if mode is FATAL).
    /// <para><b><i>⚠Note: this is an internal argument that should usually not be used by consumers.</i></b></para>
    /// <para>当消息存在于 Stream 中、但不在当前消费组 PEL 中时，强制创建无消费者所有权的 PEL 记录。不会创建新的 Stream 消息。</para>
    /// <para><b><i>⚠警告提示: 此参数主要用于内部操作，普通消费者通常不需要启用。</i></b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of messages successfully released back to the group PEL. Messages that are not in the consumer group PEL will not be counted.
    /// <para>成功恢复为待处理状态的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XNAck">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XNAck">命令文档</see></para>
    /// </remarks>
    public Task<long> XNAckAsync(
        SharpValue key,
        string group,
        ReadOnlySpan<EntryId> ids,
        NackMode mode,
        long? retryCount = null,
        bool force = false,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XNAck(
            key: key,
            group: group,
            mode: mode,
            ids: ids,
            retryCount: retryCount,
            force: force) | flags, cancellationToken);
    }

    /// <summary>
    /// Sets the last-generated ID of a stream, optionally updating the entries-added counter and maximum deleted entry ID.
    /// <para><b><i>⚠Note: XSETID is an internal command. Users should not call it directly.</i></b></para>
    /// <para>Available since: 5.0.0</para>
    /// <para>设置 Stream 的最后生成消息 ID，并可选择修改累计添加数量和最大已删除消息 ID。</para>
    /// <para><b><i>⚠警告提示: 此命令主要供 Redis 内部复制使用，普通业务代码不建议直接调用。</i></b></para>
    /// <para>支持此命令的 Redis 版本：5.0.0+</para>
    /// </summary>
    /// <param name="key">
    /// The stream key.
    /// <para>Stream 的 Key，必须已经存在。</para>
    /// </param>
    /// <param name="lastId">
    /// The new last-generated entry ID.
    /// <para>要设置的最后生成消息 ID。</para>
    /// <para>不能小于 Stream 中当前最大的有效消息 ID。</para>
    /// </param>
    /// <param name="entriesAdded">
    /// Sets the total number of entries added to the stream.
    /// <para>Available since: 7.0.0</para>
    /// <para>设置 Stream 历史累计添加的消息数量。</para>
    /// <para>该值不能小于 Stream 当前的消息数量。</para>
    /// <para>Null 表示不修改。</para>
    /// <para>自 Redis 7.0.0+ 支持该参数。</para>
    /// </param>
    /// <param name="maxDeletedId">
    /// Sets the maximum deleted entry ID.
    /// <para>Available since: 7.0.0</para>
    /// <para>设置 Stream 历史最大已删除消息 ID。</para>
    /// <para>Null 表示不修改。</para>
    /// <para>自 Redis 7.0.0+ 支持该参数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// True if the operation succeeds.
    /// <para>执行成功返回 True，否则抛出对应异常。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xsetid">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xsetid">命令文档</see></para>
    /// </remarks>
    public Task<bool> XSetIDAsync(
        SharpValue key,
        EntryId lastId,
        long? entriesAdded = null,
        EntryId maxDeletedId = default,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XSetID(
            key: key,
            lastId: lastId,
            entriesAdded: entriesAdded,
            maxDeletedId: maxDeletedId) | flags, cancellationToken);
    }

    /// <summary>
    /// XTRIM trims the stream by evicting older entries (entries with lower IDs) if needed.
    /// <para>Available since: 5.0.0</para>
    /// <para>修剪 Stream 到指定长度或最小 ID。删除最旧的消息。</para>
    /// <para>支持此命令的 Redis 版本：5.0.0+</para>
    /// </summary>
    /// <param name="key">The name of the stream key.
    /// <para>要修剪的Stream Key</para>
    /// </param>
    /// <param name="trimOptions">Options for trimming the stream
    /// <para>修剪流的选项</para>
    /// </param>
    /// <param name="delPolicy">Specifies how to handle consumer group references when trimming. If there are no consumer groups, these arguments have no effect. <b>Available since Redis 8.2.</b>
    /// <para>修剪时如何处理消费者组引用。如果没有消费者组，该参数就没有作用。<b>自 Redis 8.2.0+ 起可用。</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The number of entries deleted from the stream.
    /// <para>从Stream中删除的消息数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/XTrim">Command documentation</see>
    /// <para><see href="https://redis.io/commands/XTrim">命令文档</see></para>
    /// </remarks>
    public Task<long> XTrimAsync(
        SharpValue key,
        TrimOptions trimOptions,
        DeletionPolicy delPolicy,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XTrim(
            key: key,
            trimOptions: trimOptions,
            delPolicy: delPolicy) | flags, cancellationToken);
    }

    /// <summary>
    /// Records IDMP metadata for an existing stream entry.
    /// <para><b><i>⚠ Note: XIDMPRECORD is an internal command. Users should not call it directly.</i></b></para>
    /// <para>Available since: 8.8.0</para>
    /// <para>为 Stream 中已经存在的消息记录生产者 ID 和幂等 ID 的映射关系。</para>
    /// <para><b><i>⚠ 警告提示：这是 Redis 内部命令，主要用于 AOF 恢复，普通业务代码不建议直接调用。</i></b></para>
    /// <para>支持此命令的 Redis 版本：8.8.0+</para>
    /// </summary>
    /// <param name="key">
    /// The stream key.
    /// <para>Stream 的 Key，必须已经存在。</para>
    /// </param>
    /// <param name="pid">
    /// The producer ID.
    /// <para>生产者 ID，用于标识消息生产者，不能为空字符串。</para>
    /// </param>
    /// <param name="iid">
    /// The idempotency ID.
    /// <para>幂等 ID，用于标识生产者发送的唯一消息，不能为空字符串。</para>
    /// </param>
    /// <param name="streamId">
    /// The ID of an existing stream entry.
    /// <para>已经存在于 Stream 中的消息 ID。</para>
    /// <para>不能指定不存在或已经删除的消息 ID。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required.
    /// <para>此次操作使用的行为标记，一般情况下无需设置。</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token.
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>
    /// True if the operation succeeds.
    /// <para>执行成功返回 True，否则抛出对应异常。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/xidmprecord">Command documentation</see>
    /// <para><see href="https://redis.io/commands/xidmprecord">命令文档</see></para>
    /// </remarks>
    public Task<bool> XIdmpRecordAsync(
        SharpValue key,
        string pid,
        string iid,
        EntryId streamId,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.InvokeAsync(StreamCommands.XIdmpRecord(
            key: key,
            pid: pid,
            iid: iid,
            streamId: streamId) | flags, cancellationToken);
    }
}
