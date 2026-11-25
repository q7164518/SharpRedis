using SharpRedis.Commands;
using SharpRedis.Provider.Standard;
using SharpRedis.Provider.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpRedis;

/// <summary>
/// Redis server management class
/// <para>Redis 服务管理操作类</para>
/// </summary>
public sealed partial class RedisServer : RedisTypeBase
{
    /// <summary>
    /// Optional filter for COMMAND LIST.
    /// <para>COMMAND LIST 的可选过滤条件，每次只支持一种条件。</para>
    /// </summary>
    public readonly struct CommandListFilter
    {
        internal string Kind { get; }
        internal string Value { get; }

        private CommandListFilter(string kind, string value)
        {
            this.Kind = kind;
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Filter by module name.
        /// <para>按模块名称过滤。</para>
        /// </summary>
        public static CommandListFilter Module(string name) => new("MODULE", name);

        /// <summary>
        /// Filter by ACL category.
        /// <para>按 ACL 分类过滤。</para>
        /// </summary>
        public static CommandListFilter AclCat(string category) => new("ACLCAT", category);

        /// <summary>
        /// Match command names against a glob pattern.
        /// <para>按 glob 模式匹配命令名称。</para>
        /// </summary>
        public static CommandListFilter Pattern(string pattern) => new("PATTERN", pattern);
    }

    /// <summary>
    /// Hotkeys tracking metrics.
    /// <para>Available since: Redis 8.6.0</para>
    /// <para>HOTKEYS START 命令的 METRICS 参数。</para>
    /// <para>支持 CPU 和 NET 两种监控指标，可同时启用。</para>
    /// <para>METRICS count 中的 count 根据启用的指标自动计算。</para>
    /// <para>支持此命令的 Redis 版本：8.6.0+</para>
    /// </summary>
    public readonly struct HotKeysMetrics
    {
        /// <summary>
        /// Hotkeys tracking metric flags.
        /// <para>HOTKEYS START 命令支持的热点 Key 监控指标标记。</para>
        /// <para>支持通过位运算组合多个监控指标。</para>
        /// <para>Available since: Redis 8.6.0</para>
        /// <para>支持此命令的 Redis 版本：8.6.0+</para>
        /// </summary>
        [Flags]
        private enum MetricFlags : byte
        {
            /// <summary>
            /// No tracking metric is enabled.
            /// <para>不启用任何监控指标。</para>
            /// <para>不能单独作为 HOTKEYS START 的有效 METRICS 参数。</para>
            /// </summary>
            None = 0,

            /// <summary>
            /// Track hotkeys by CPU execution time.
            /// <para>根据命令消耗的 CPU 时间统计热点 Key。</para>
            /// <para>对应 METRICS 1 CPU。</para>
            /// </summary>
            Cpu = 1,

            /// <summary>
            /// Track hotkeys by network traffic.
            /// <para>根据命令产生的网络流量统计热点 Key。</para>
            /// <para>对应 METRICS 1 NET。</para>
            /// </summary>
            Net = 2,
        }

        private readonly MetricFlags _flags;

        /// <summary>
        /// Whether CPU time tracking is enabled.
        /// <para>是否启用 CPU 时间统计。</para>
        /// </summary>
        public readonly bool CPU => (this._flags & MetricFlags.Cpu) != 0;

        /// <summary>
        /// Whether network traffic tracking is enabled.
        /// <para>是否启用 NET 网络流量统计。</para>
        /// </summary>
        public readonly bool NET => (this._flags & MetricFlags.Net) != 0;

        /// <summary>
        /// Number of enabled metrics.
        /// <para>启用的指标数量，对应 METRICS count。</para>
        /// <para>有效值为 1 或 2，默认结构体为 0。</para>
        /// </summary>
        public readonly int Count => (this.CPU ? 1 : 0) + (this.NET ? 1 : 0);

        /// <summary>
        /// Whether at least one metric is enabled.
        /// <para>是否至少启用了一种监控指标。</para>
        /// </summary>
        public readonly bool IsValid => this.Count > 0;

        /// <summary>
        /// Track hotkeys by CPU time.
        /// <para>仅启用 CPU 监控。</para>
        /// <para>对应 METRICS 1 CPU。</para>
        /// </summary>
        public static HotKeysMetrics Cpu => new(true, false);

        /// <summary>
        /// Track hotkeys by network bytes.
        /// <para>仅启用 NET 监控。</para>
        /// <para>对应 METRICS 1 NET。</para>
        /// </summary>
        public static HotKeysMetrics Net => new(false, true);

        /// <summary>
        /// Track hotkeys by both CPU time and network bytes.
        /// <para>同时启用 CPU 和 NET 监控。</para>
        /// <para>对应 METRICS 2 CPU NET。</para>
        /// </summary>
        public static HotKeysMetrics All => new(true, true);

        /// <summary>
        /// Initializes hotkeys tracking metrics.
        /// <para>初始化 HOTKEYS 监控指标。</para>
        /// </summary>
        /// <param name="cpu">
        /// Whether to track CPU time.
        /// <para>是否统计 CPU 时间。</para>
        /// </param>
        /// <param name="net">
        /// Whether to track network bytes.
        /// <para>是否统计网络流量。</para>
        /// </param>
        /// <exception cref="ArgumentException">
        /// Neither CPU nor NET was enabled.
        /// <para>CPU 和 NET 均未启用时引发异常。</para>
        /// </exception>
        public HotKeysMetrics(bool cpu, bool net)
        {
            if (!cpu && !net)
            {
                throw new ArgumentException("At least one of CPU or NET must be enabled.");
            }

            this._flags =
                (cpu ? MetricFlags.Cpu : MetricFlags.None)
                | (net ? MetricFlags.Net : MetricFlags.None);
        }
    }


    internal RedisServer(BaseRedis redis) : base(redis)
    { }

    /// <summary>
    /// The command shows the available ACL categories if called without arguments. If a category name is given, the command shows all the Redis commands in the specified category.
    /// <para>Available since: 6.0.0</para>
    /// <para>返回 ACL 命令分类的完整列表，或返回指定分类下的所有命令</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="category">List only the commands in the given ACL category. If omitted, all categories are listed.
    /// <para>ACL 分类名称。不提供时返回所有分类列表</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>elements representing ACL categories or commands in a given category.
    /// <para>分类列表（无参数时）或命令列表（有参数时）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-cat">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-cat">命令文档</see></para>
    /// </remarks>
    public string[] AclCat(
#if MODERN_NET
        string? category = null,
#else
        string category = null,
#endif
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclCat(category) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Delete all the specified ACL users and terminate all the connections that are authenticated with such users.
    /// <para>Available since: 6.0.0</para>
    /// <para>删除一个 ACL 用户</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="username">One usernames to delete.
    /// <para>要删除的用户名</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-deluser">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-deluser">命令文档</see></para>
    /// </remarks>
    public bool AclDelUser(string username, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclDelUser(username) | flags, cancellationToken);
    }

    /// <summary>
    /// Delete all the specified ACL users and terminate all the connections that are authenticated with such users.
    /// <para>Available since: 6.0.0</para>
    /// <para>删除一个或多个 ACL 用户</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="usernames">One or more usernames to delete.
    /// <para>要删除的用户名, 支持多个</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of users that were deleted. This number will not always match the number of arguments since certain users may not exist.
    /// <para>删除的用户数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-deluser">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-deluser">命令文档</see></para>
    /// </remarks>
    public int AclDelUser(ReadOnlySpan<string> usernames, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclDelUser(usernames) | flags, cancellationToken);
    }

    /// <summary>
    /// Simulate the execution of a given command by a given user. This command can be used to test the permissions of a given user without having to enable the user or cause the side effects of running the command.
    /// <para>Available since: 7.0.0</para>
    /// <para>模拟验证指定用户是否有权限执行给定命令，不实际执行命令</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="username">The user to run the simulated command as.
    /// <para>要验证的 ACL 用户名</para>
    /// </param>
    /// <param name="command">The command to test. For example, ["GET", "test_key"]
    /// <para>要模拟执行的命令, 如 ["GET", "test_key"]</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>OK on success. an error describing why the user can't execute the command.
    /// <para>成功执行返回OK, 否则返回不能执行的原因说明</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-dryrun">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-dryrun">命令文档</see></para>
    /// </remarks>
    public string AclDryRun(string username, SharpValueSpan command, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclDryRun(username, command) | flags, cancellationToken)!;
    }

    /// <summary>
    /// The command output is a hexadecimal representation of a binary string. By default it emits 256 bits (so 64 hex characters). 
    /// <para>Available since: 6.0.0</para>
    /// <para>生成一个加密安全的随机密码，可用于 ACL 用户配置</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="bits">The amount of entropy, in bits, for the generated password. Defaults to 256.
    /// <para>密码的熵位数（比特数），默认 256，范围 1~1024</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>pseudorandom data. By default it contains 64 bytes, representing 256 bits of data. If bits was given, the output string length is the number of specified bits (rounded to the next multiple of 4) divided by 4.
    /// <para>Base64 编码的随机密码</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-genpass">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-genpass">命令文档</see></para>
    /// </remarks>
    public string AclGenPass(int? bits = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclGenPass(bits) | flags, cancellationToken)!;
    }

    /// <summary>
    /// The command returns all the rules defined for an existing ACL user.
    /// <para>Available since: 6.0.0</para>
    /// <para>返回指定 ACL 用户的完整配置信息</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="username">The user whose rules to return.
    /// <para>要查询的 ACL 用户名</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-getuser">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-getuser">命令文档</see></para>
    /// </remarks>
    public AclUserInfo AclGetUser(string username, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclGetUser(username) | flags, cancellationToken);
    }

    /// <summary>
    /// The command shows the currently active ACL rules in the Redis server.
    /// <para>Available since: 6.0.0</para>
    /// <para>返回当前所有 ACL 用户的配置规则，格式与 ACL 配置文件兼容，可直接用于备份和迁移</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>an array of Bulk string reply elements.
    /// <para>每行是一个用户的 ACL 规则文本</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-list">命令文档</see></para>
    /// </remarks>
    public string[] AclList(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclList() | flags, cancellationToken)!;
    }

    /// <summary>
    /// When Redis is configured to use an ACL file (with the aclfile configuration option), this command will reload the ACLs from the file, replacing all the current ACL rules with the ones defined in the file.
    /// <para>Available since: 6.0.0</para>
    /// <para>从 aclfile 加载 ACL 配置。会覆盖当前内存中的 ACL 配置。</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>OK on success. The command may fail with an error for several reasons: if the file is not readable, if there is an error inside the file, and in such cases, the error will be reported to the user in the error. Finally, the command will fail if the server is not configured to use an external ACL file.
    /// <para>加载成功返回OK, 否则返回错误信息或引发异常</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-load">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-load">命令文档</see></para>
    /// </remarks>
    public string AclLoad(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclLoad() | flags, cancellationToken)!;
    }

    /// <summary>
    /// The command shows a list of recent ACL security events
    /// <para>Available since: 6.0.0</para>
    /// <para>ACL LOG 返回 ACL 相关的安全事件日志，如认证失败、权限拒绝（NOPERM）等，用于安全审计和故障排查</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="count">The number of recent entries to return
    /// <para>返回最近的 N 条日志条目，默认 10 条</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-log">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-log">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    AclLogEntry[]?
#else
    AclLogEntry[]
#endif
    AclLog(int? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclLog(count) | flags, cancellationToken);
    }

    /// <summary>
    /// Clear the security log
    /// <para>Available since: 6.0.0</para>
    /// <para>清空 ACL 日志缓冲区</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-log">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-log">命令文档</see></para>
    /// </remarks>
    public bool AclLogReset(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclLog(true) | flags, cancellationToken);
    }

    /// <summary>
    /// When Redis is configured to use an ACL file (with the aclfile configuration option), this command will save the currently defined ACLs from the server memory to the ACL file.
    /// <para>Available since: 6.0.0</para>
    /// <para>将当前 ACL 配置写入 aclfile（redis.conf 中配置的文件路径）</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The command may fail with an error for several reasons: if the file cannot be written or if the server is not configured to use an external ACL file.
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-save">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-save">命令文档</see></para>
    /// </remarks>
    public bool AclSave(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclSave() | flags, cancellationToken);
    }

    /// <summary>
    /// Create an ACL user with the specified rules or modify the rules of an existing user.
    /// <para>Available since: 6.0.0</para>
    /// <para>创建或修改 ACL 用户</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="username">The user to create or modify.
    /// <para>要创建或修改的用户名</para>
    /// </param>
    /// <param name="rules">One or more ACL rules to apply to the user.
    /// <para>权限规则，多个规则叠加</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-setuser">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-setuser">命令文档</see></para>
    /// </remarks>
    public bool AclSetUser(string username, ReadOnlySpan<string> rules = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclSetUser(username, rules) | flags, cancellationToken);
    }

    /// <summary>
    /// Create an ACL user with the specified rules or modify the rules of an existing user.
    /// <para>Available since: 6.0.0</para>
    /// <para>创建或修改 ACL 用户</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="username">The user to create or modify.
    /// <para>要创建或修改的用户名</para>
    /// </param>
    /// <param name="rule">One ACL rules to apply to the user.
    /// <para>权限规则</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-setuser">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-setuser">命令文档</see></para>
    /// </remarks>
    public bool AclSetUser(string username, string rule, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclSetUser(username, rule) | flags, cancellationToken);
    }

    /// <summary>
    /// The command shows a list of all the usernames of the currently configured users in the Redis ACL system.
    /// <para>Available since: 6.0.0</para>
    /// <para>返回 Redis 中所有已配置的 ACL 用户名列表</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>list of existing ACL users.
    /// <para>用户列表数组</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-users">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-users">命令文档</see></para>
    /// </remarks>
    public string[] AclUsers(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclUsers() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the username the current connection is authenticated with.
    /// <para>Available since: 6.0.0</para>
    /// <para>返回当前连接所使用的 ACL 用户名</para>
    /// <para>支持此命令的Redis版本, 6.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The username of the current connection.
    /// <para>当前连接的用户名</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/acl-whoami">Command documentation</see>
    /// <para><see href="https://redis.io/commands/acl-whoami">命令文档</see></para>
    /// </remarks>
    public string AclWhoAmI(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.AclWhoAmI() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Delete all the keys of all the existing databases, not just the currently selected one. This command never fails.
    /// <para>Available since: 1.0.0</para>
    /// <para>清空 Redis 服务器中所有数据库的 key, 此命令永远不会执行失败</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="mode">Starting with Redis version 4.0.0: Added the ASYNC flushing mode modifier.
    /// <para>Starting with Redis version 6.2.0: Added the SYNC flushing mode modifier. </para>
    /// <para>4.0.0+ 支持 ASYNC</para>
    /// <para>从 Redis 6.2.0 版本开始：新增了 SYNC 刷新模式修饰符</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/FlushAll">Command documentation</see>
    /// <para><see href="https://redis.io/commands/FlushAll">命令文档</see></para>
    /// </remarks>
    public bool FlushAll(AsyncSync mode = AsyncSync.Sync, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        => base.Invoke(ServerCommands.FlushAll(mode) | flags, cancellationToken);

    /// <summary>
    /// Delete all the keys of the currently selected DB. This command never fails.
    /// <para>Available since: 1.0.0</para>
    /// <para>清空当前数据库中的所有 key</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="mode">Starting with Redis version 4.0.0: Added the ASYNC flushing mode modifier.
    /// <para>Starting with Redis version 6.2.0: Added the SYNC flushing mode modifier. </para>
    /// <para>4.0.0+ 支持 ASYNC</para>
    /// <para>从 Redis 6.2.0 版本开始：新增了 SYNC 刷新模式修饰符</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/Flushdb">Command documentation</see>
    /// <para><see href="https://redis.io/commands/Flushdb">命令文档</see></para>
    /// </remarks>
    public bool FlushDB(AsyncSync mode = AsyncSync.Sync, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.FlushDB(mode) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns tracking results and metadata from the current or most recent hotkeys tracking session.
    /// <para>Available since: 8.6.0</para>
    /// <para>返回当前或最近一次热key跟踪会话的跟踪结果和元数据</para>
    /// <para>支持此命令的Redis版本, 8.6.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/hotkeys-get">Command documentation</see>
    /// <para><see href="https://redis.io/commands/hotkeys-get">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    HotKeysInfo?
#else
    HotKeysInfo
#endif
    HotKeysGet(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.HotKeysGet() | flags, cancellationToken);
    }

    /// <summary>
    /// Release the resources used for hotkey tracking.
    /// <para>Available since: 8.6.0</para>
    /// <para>重置 HotKeys 模块的统计数据，清空当前已记录的热点键访问频率信息</para>
    /// <para>支持此命令的Redis版本, 8.6.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/hotkeys-reset">Command documentation</see>
    /// <para><see href="https://redis.io/commands/hotkeys-reset">命令文档</see></para>
    /// </remarks>
    public bool HotKeysReset(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.HotKeysReset() | flags, cancellationToken);
    }

    /// <summary>
    /// Starts hotkeys tracking with specified metrics.
    /// <para>Available since: 8.6.0</para>
    /// <para>启动 HotKeys 模块的访问频率监控，开始统计 Redis 实例中各键的访问频率</para>
    /// <para>支持此命令的Redis版本, 8.6.0+</para>
    /// </summary>
    /// <param name="metrics">Specifies which metrics to track and how many hotkeys to track.
    /// <para>指定要跟踪的指标</para>
    /// </param>
    /// <param name="count">Specifies the value of K for the top-K hotkeys tracking.
    /// <para>指定用于跟踪前K个热Key的K值。</para>
    /// </param>
    /// <param name="duration">The duration in seconds for how long the hotkeys tracking should run. After this time period, tracking will automatically stop. 
    /// <para>跟踪的持续时间（以秒为单位）。超过此时间后，跟踪将自动停止</para>
    /// </param>
    /// <param name="sampleRatio">Sampling ratio for probabilistic tracking. Each key is sampled with probability 1/ratio. Higher values reduce performance impact but may miss some hotkeys. Lower values provide more accurate results but with higher performance cost.
    /// <para>跟踪的采样率. 较高的采样率可以降低性能影响，但可能会漏掉一些热Key。较低的采样率可以提供更精确的结果，但会增加性能损失。</para>
    /// </param>
    /// <param name="slots">Specifies which hash slots to track in a cluster environment. Takes a count followed by that many slot numbers. Only keys that hash to the specified slots will be tracked. Useful for tracking hotkeys on specific shards in a Redis cluster. Using SLOTS when not in cluster mode will result in an error.
    /// <para>指定要在集群环境中跟踪哪些哈希槽。参数接受一个计数，后跟相应数量的槽编号。只有哈希到指定槽的键才会被跟踪。这对于跟踪 Redis 集群中特定分片上的热键非常有用。在非集群模式下使用 SLOTS 会导致错误。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/hotkeys-start">Command documentation</see>
    /// <para><see href="https://redis.io/commands/hotkeys-start">命令文档</see></para>
    /// </remarks>
    public bool HotKeysStart(
        HotKeysMetrics metrics,
        long? count = null,
        long? duration = null,
        long? sampleRatio = null,
        ReadOnlySpan<int> slots = default,
        CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.HotKeysStart(
            metrics: metrics,
            count: count,
            duration: duration,
            sampleRatio: sampleRatio,
            slots: slots) | flags, cancellationToken);
    }

    /// <summary>
    /// Stops hotkeys tracking but preserves the collected data.
    /// <para>Available since: 8.6.0</para>
    /// <para>停止 HotKeys 模块的访问频率监控，暂停统计 Redis 实例中各键的访问频率</para>
    /// <para>支持此命令的Redis版本, 8.6.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/hotkeys-stop">Command documentation</see>
    /// <para><see href="https://redis.io/commands/hotkeys-stop">命令文档</see></para>
    /// </remarks>
    public bool HotKeysStop(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.HotKeysStop() | flags, cancellationToken);
    }

    /// <summary>
    /// Save the DB in background.
    /// <para>Available since: 1.0.0</para>
    /// <para>后台执行 RDB 快照保存。通过 fork 子进程完成，不阻塞主线程</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="schedule"><b>Starting with Redis version 3.2.2: Added the SCHEDULE option.</b>
    /// <para>Schedule the save to run after any in-progress BGSAVE finishes, instead of returning an error.</para>
    /// <para><b>3.2.2+ 支持此选项. </b>如果正在执行 AOF 重写，则调度保存</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <remarks>
    /// <see href="https://redis.io/commands/bgsave">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bgsave">命令文档</see></para>
    /// </remarks>
    public string BgSave(bool schedule = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BgSave(schedule) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Cancels a backup that has not been sealed yet.
    /// <para>Available since: 8.10.0</para>
    /// <para>取消正在进行、但尚未完成封存（SEAL）的备份任务</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/backup-abort">Command documentation</see>
    /// <para><see href="https://redis.io/commands/backup-abort">命令文档</see></para>
    /// </remarks>
    public bool BackupAbort(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BackupAbort() | flags, cancellationToken);
    }

    /// <summary>
    /// Removes sealed backup files and returns the backup state machine to idle.
    /// <para>Available since: 8.10.0</para>
    /// <para>删除封存的备份文件，并将备份状态恢复到空闲状态</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/backup-cleanup">Command documentation</see>
    /// <para><see href="https://redis.io/commands/backup-cleanup">命令文档</see></para>
    /// </remarks>
    public bool BackupCleanup(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BackupCleanup() | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the absolute paths of the immutable backup files pinned so far.
    /// <para>Available since: 8.10.0</para>
    /// <para>返回目前为止已锁定的不可变备份文件的绝对路径</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the absolute paths of the immutable files pinned so far.</returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/backup-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/backup-list">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string[]?
#else
    string[]
#endif
    BackupList(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BackupList() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Freezes the current backup, producing the final immutable file set: the BASE snapshot, the incremental append-only file (INCR), and a standalone manifest.
    /// <para>Available since: 8.10.0</para>
    /// <para>冻结当前备份，生成最终的不可变文件集：基本快照、增量追加文件 (INCR) 和独立清单。</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/backup-seal">Command documentation</see>
    /// <para><see href="https://redis.io/commands/backup-seal">命令文档</see></para>
    /// </remarks>
    public bool BackupSeal(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BackupSeal() | flags, cancellationToken);
    }

    /// <summary>
    /// Starts a new backup, opening a backup window and producing a fresh BASE snapshot in the directory named by the backupdirname configuration setting.
    /// <para>Available since: 8.10.0</para>
    /// <para>开始新的备份，打开备份窗口，并在 backupdirname 配置设置指定的目录中生成新的 BASE 快照</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/backup-start">Command documentation</see>
    /// <para><see href="https://redis.io/commands/backup-start">命令文档</see></para>
    /// </remarks>
    public bool BackupStart(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BackupStart() | flags, cancellationToken);
    }

    /// <summary>
    /// Reports the current backup state.
    /// <para>Available since: 8.10.0</para>
    /// <para>获得当前备份状态</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/backup-status">Command documentation</see>
    /// <para><see href="https://redis.io/commands/backup-status">命令文档</see></para>
    /// </remarks>
    public BackupStatusInfo BackupStatus(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BackupStatus() | flags, cancellationToken);
    }

    /// <summary>
    /// Instruct Redis to start an Append Only File rewrite process. The rewrite will create a small optimized version of the current Append Only File.
    /// <para>Available since: 1.0.0</para>
    /// <para>后台重写 AOF（Append Only File）文件。通过 fork 子进程完成，不阻塞主线程。用于压缩 AOF 文件大小</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <remarks>
    /// <see href="https://redis.io/commands/bgrewriteaof">Command documentation</see>
    /// <para><see href="https://redis.io/commands/bgrewriteaof">命令文档</see></para>
    /// </remarks>
    public string BgRewriteAof(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.BgRewriteAof() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Return the number of keys in the currently-selected database.
    /// <para>Available since: 1.0.0</para>
    /// <para>返回当前选中数据库的 key 总数</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>the number of keys in the currently-selected database
    /// <para>当前数据库中的 key 数量</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/DBSize">Command documentation</see>
    /// <para><see href="https://redis.io/commands/DBSize">命令文档</see></para>
    /// </remarks>
    public long DBSize(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.DBSize() | flags, cancellationToken);
    }

    /// <summary>
    /// Return the UNIX TIME of the last DB save executed with success.
    /// <para>Available since: 1.0.0</para>
    /// <para>返回最近一次成功执行 RDB 持久化（如 SAVE 或 BGSAVE）的 Unix 时间戳（秒）</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>UNIX TIME of the last DB save executed with success.
    /// <para>最近一次成功 RDB 保存的 Unix 时间戳（秒）</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/lastsave">Command documentation</see>
    /// <para><see href="https://redis.io/commands/lastsave">命令文档</see></para>
    /// </remarks>
    public long LastSave(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.LastSave() | flags, cancellationToken);
    }

    /// <summary>
    /// The LATENCY DOCTOR command reports about different latency-related issues and advises about possible remedies.
    /// <para>Available since: 2.8.13</para>
    /// <para>LATENCY-DOCTOR 是 Redis 延迟监控的诊断工具，输出延迟问题的诊断报告和建议。</para>
    /// <para>支持此命令的Redis版本, 2.8.13+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>a human readable latency analysis report.
    /// <para>延迟诊断报告</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/latency-doctor">Command documentation</see>
    /// <para><see href="https://redis.io/commands/latency-doctor">命令文档</see></para>
    /// </remarks>
    public string LatencyDoctor(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.LatencyDoctor() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Produces an ASCII-art style graph for the specified event.
    /// <para>Available since: 2.8.13</para>
    /// <para>以 ASCII 图表形式展示指定延迟事件的历史数据</para>
    /// <para>支持此命令的Redis版本, 2.8.13+</para>
    /// </summary>
    /// <param name="event">The latency event to graph.
    /// <para>延迟事件名称</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Latency graph
    /// <para>ASCII 图表</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/latency-graph">Command documentation</see>
    /// <para><see href="https://redis.io/commands/latency-graph">命令文档</see></para>
    /// </remarks>
    public string LatencyGraph(string @event, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.LatencyGraph(@event) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Returns a report on memory usage and suggestions for resolving memory problems.
    /// <para>Available since: 4.0.0</para>
    /// <para>返回内存使用诊断报告和改善内存使用的建议。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The diagnostic report.
    /// <para>内存诊断报告。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/memory-doctor">Command documentation</see>
    /// <para><see href="https://redis.io/commands/memory-doctor">命令文档</see></para>
    /// </remarks>
    public string MemoryDoctor(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.MemoryDoctor() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Returns the statistics reported by the Redis memory allocator.
    /// <para>Available since: 4.0.0</para>
    /// <para>返回 Redis 内存分配器提供的统计报告。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The allocator report. Its contents depend on the allocator in use.
    /// <para>内存分配器报告，具体内容取决于 Redis 使用的分配器。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/memory-malloc-stats">Command documentation</see>
    /// <para><see href="https://redis.io/commands/memory-malloc-stats">命令文档</see></para>
    /// </remarks>
    public string MemoryMallocStats(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.MemoryMallocStats() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Asks the allocator to release unused memory to the operating system.
    /// <para>Available since: 4.0.0</para>
    /// <para>请求内存分配器将未使用的内存归还给操作系统。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True if Redis replies with OK. The amount of memory released depends on the allocator.
    /// <para>Redis 返回 OK 时为 true，实际释放的内存量取决于分配器。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/memory-purge">Command documentation</see>
    /// <para><see href="https://redis.io/commands/memory-purge">命令文档</see></para>
    /// </remarks>
    public bool MemoryPurge(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.MemoryPurge() | flags, cancellationToken);
    }

    /// <summary>
    /// Returns memory usage statistics for the Redis server.
    /// <para>Available since: 4.0.0</para>
    /// <para>返回 Redis 服务端的内存使用统计信息。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Memory metrics by name. Per-database entries contain nested statistics.
    /// <para>按指标名称返回统计值，各数据库对应的条目包含嵌套统计信息。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/memory-stats">Command documentation</see>
    /// <para><see href="https://redis.io/commands/memory-stats">命令文档</see></para>
    /// </remarks>
#if MODERN_NET
    public Dictionary<RedisResult, RedisResult>? MemoryStats(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#else
    public Dictionary<RedisResult, RedisResult> MemoryStats(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
#endif
    {
        return base.Invoke(ServerCommands.MemoryStats() | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the number of bytes that a key and its value occupy in memory.
    /// <para>Available since: 4.0.0</para>
    /// <para>返回指定 key 及其值占用的内存字节数。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="key">The key to inspect.
    /// <para>要统计内存占用的 key</para>
    /// </param>
    /// <param name="samples">Number of nested values to sample. Omit to use the Redis default of 5, or pass 0 to sample all values.
    /// <para>嵌套值的采样数量，省略时 Redis 默认采样 5 个，传入 0 表示统计所有值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The memory usage in bytes, or null if the key does not exist.
    /// <para>内存占用字节数，key 不存在时返回 null。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/memory-usage">Command documentation</see>
    /// <para><see href="https://redis.io/commands/memory-usage">命令文档</see></para>
    /// </remarks>
    public long? MemoryUsage(SharpValue key, long? samples = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.MemoryUsage(key, samples) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns metadata for every command on the server.
    /// <para>Available since: 2.8.13</para>
    /// <para>获取服务端所有命令的元数据。</para>
    /// <para>支持此命令的Redis版本, 2.8.13+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Command metadata including available subcommands.
    /// <para>命令元数据及其子命令。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/command">Command documentation</see>
    /// <para><see href="https://redis.io/commands/command">命令文档</see></para>
    /// </remarks>
    public CommandMetadata[] Command(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Command() | flags, cancellationToken)!;
    }

    /// <summary>
    /// Counts the commands supported by the server.
    /// <para>Available since: 2.8.13</para>
    /// <para>统计服务端支持的命令数。</para>
    /// <para>支持此命令的Redis版本, 2.8.13+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Number of commands.
    /// <para>命令数量。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/command-count">Command documentation</see>
    /// <para><see href="https://redis.io/commands/command-count">命令文档</see></para>
    /// </remarks>
    public long CommandCount(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.CommandCount() | flags, cancellationToken);
    }

    /// <summary>
    /// Returns documentation for the specified commands, or all commands if omitted.
    /// <para>Available since: 7.0.0</para>
    /// <para>获取指定命令的文档，省略命令名称时返回全部文档。</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="names">Command names.
    /// <para>命令名称集合。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Documentation indexed by command name. Unknown names are omitted.
    /// <para>按命令名索引的文档，忽略未知命令。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/command-docs">Command documentation</see>
    /// <para><see href="https://redis.io/commands/command-docs">命令文档</see></para>
    /// </remarks>
    public Dictionary<string, CommandDocumentation> CommandDocs(SharpValueSpan names = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.CommandDocs(names) | flags, cancellationToken);
    }

    /// <summary>
    /// Extracts the keys from a complete command invocation.
    /// <para>Available since: 2.8.13</para>
    /// <para>从完整的命令参数中提取 key。</para>
    /// <para>支持此命令的Redis版本, 2.8.13+</para>
    /// </summary>
    /// <param name="command">Command name followed by its arguments, for example ["MGET", "a", "b"].
    /// <para>完整命令，首项为命令名，后续各项为参数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Keys in server reply order, or null for an empty reply.
    /// <para>按服务端返回顺序排列的 key，空集合返回 null。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/command-getkeys">Command documentation</see>
    /// <para><see href="https://redis.io/commands/command-getkeys">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    RedisResult[]?
#else
    RedisResult[]
#endif
    CommandGetKeys(SharpValueSpan command, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.CommandGetKeys(command, false) | flags, cancellationToken);
    }

    /// <summary>
    /// Extracts the keys and their access flags from a command invocation.
    /// <para>Available since: 7.0.0</para>
    /// <para>从完整的命令参数中提取 key 及访问标记。</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="command">Command name followed by its arguments, for example ["MGET", "a", "b"].
    /// <para>完整命令，首项为命令名，后续各项为参数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Keys and their access flags.
    /// <para>各 key 及其访问标记。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/command-getkeysandflags">Command documentation</see>
    /// <para><see href="https://redis.io/commands/command-getkeysandflags">命令文档</see></para>
    /// </remarks>
    public CommandKeyFlags[] CommandGetKeysAndFlags(SharpValueSpan command, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.CommandGetKeys(command, true) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns metadata for the named commands. Redis 7.0+ accepts an empty list to request all commands.
    /// <para>Available since: 2.8.13</para>
    /// <para>获取指定命令的元数据，Redis 7.0+ 支持省略名称以获取所有命令。</para>
    /// <para>支持此命令的Redis版本, 2.8.13+</para>
    /// </summary>
    /// <param name="names">Command names.
    /// <para>命令名称集合。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>One entry per requested name, in order. An unknown command yields null.
    /// <para>结果与输入顺序一致，未知命令对应 null。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/command-info">Command documentation</see>
    /// <para><see href="https://redis.io/commands/command-info">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    CommandMetadata?[]
#else
    CommandMetadata[]
#endif
    CommandInfo(SharpValueSpan names = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.CommandInfo(names) | flags, cancellationToken);
    }

    /// <summary>
    /// Lists command names with an optional filter.
    /// <para>Available since: 7.0.0</para>
    /// <para>列出命令名称，支持可选的过滤条件。</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="filter">One MODULE, ACLCAT or PATTERN filter.
    /// <para>按模块、ACL 分类或 glob 模式过滤。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Matching command names, or null when there are no matches.
    /// <para>匹配的命令名称，无匹配项时返回 null。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/command-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/command-list">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    string[]?
#else
    string[]
#endif
    CommandList(CommandListFilter filter = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.CommandList(filter) | flags, cancellationToken)!;
    }

    /// <summary>
    /// The <b>CONFIG GET</b> command is used to read the configuration parameters of a running Redis server. 
    /// <para>Available since: 2.0.0</para>
    /// <para>获取 Redis 配置参数的值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="parameter">One configuration parameter names or glob-style patterns to retrieve.
    /// <para>配置参数名，支持通配符 *</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/config-get">Command documentation</see>
    /// <para><see href="https://redis.io/commands/config-get">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Dictionary<string, string>?
#else
    Dictionary<string, string>
#endif
    ConfigGet(string parameter, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ConfigGet(parameter) | flags, cancellationToken);
    }

    /// <summary>
    /// The <b>CONFIG GET</b> command is used to read the configuration parameters of a running Redis server. 
    /// <para>Available since: 2.0.0</para>
    /// <para>获取 Redis 配置参数的值</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="parameters">One or more configuration parameter names or glob-style patterns to retrieve.
    /// <para><b>Starting with Redis version 7.0.0: Added the ability to pass multiple pattern parameters in one call</b></para>
    /// <para>配置参数名，支持通配符 *</para>
    /// <para><b>Redis 7.0.0+开始支持指定多个配置参数名</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/config-get">Command documentation</see>
    /// <para><see href="https://redis.io/commands/config-get">命令文档</see></para>
    /// </remarks>
    public
#if MODERN_NET
    Dictionary<string, string>?
#else
    Dictionary<string, string>
#endif
    ConfigGet(ReadOnlySpan<string> parameters, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ConfigGet(parameters) | flags, cancellationToken);
    }

    /// <summary>
    /// Resets the statistics reported by Redis using the INFO and LATENCY HISTOGRAM commands.
    /// <para>Available since: 2.0.0</para>
    /// <para>重置 INFO 命令返回的统计信息（将各项计数器清零）</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/config-resetstat">Command documentation</see>
    /// <para><see href="https://redis.io/commands/config-resetstat">命令文档</see></para>
    /// </remarks>
    public bool ConfigResetStat(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ConfigResetStat() | flags, cancellationToken);
    }

    /// <summary>
    /// The CONFIG REWRITE command rewrites the redis.conf file the server was started with, applying the minimal changes needed to make it reflect the configuration currently used by the server, which may be different compared to the original one because of the use of the CONFIG SET command.
    /// <para>Available since: 2.8.0</para>
    /// <para>将当前运行时配置重写回 redis.conf 配置文件</para>
    /// <para>支持此命令的Redis版本, 2.8.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/config-rewrite">Command documentation</see>
    /// <para><see href="https://redis.io/commands/config-rewrite">命令文档</see></para>
    /// </remarks>
    public bool ConfigRewrite(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ConfigRewrite() | flags, cancellationToken);
    }

    /// <summary>
    /// CONFIG SET reconfigures Redis at runtime without restarting the server. You can use it to change simple configuration parameters or switch between persistence options.
    /// <para>Available since: 2.0.0</para>
    /// <para>用于动态修改 Redis 配置参数，无需重启服务</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="parameter">the configuration parameter name
    /// <para>配置参数名称</para>
    /// </param>
    /// <param name="value">The values of the configuration parameter
    /// <para>配置参数的值</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/config-set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/config-set">命令文档</see></para>
    /// </remarks>
    public bool ConfigSet(string parameter, string value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ConfigSet(parameter, value) | flags, cancellationToken);
    }

    /// <summary>
    /// CONFIG SET reconfigures Redis at runtime without restarting the server. You can use it to change simple configuration parameters or switch between persistence options.
    /// <para>Available since: 2.0.0</para>
    /// <para>用于动态修改 Redis 配置参数，无需重启服务</para>
    /// <para>支持此命令的Redis版本, 2.0.0+</para>
    /// </summary>
    /// <param name="parameterValues">One or more configuration parameter-value pairs to set.
    /// <para><b>Starting with Redis version 7.0.0: Added the ability to set multiple parameters in one call.</b></para>
    /// <para>一个或多个配置参数名称-配置参数值</para>
    /// <para><b>Redis 7.0.0+ 支持一次性传入多个配置</b></para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/config-set">Command documentation</see>
    /// <para><see href="https://redis.io/commands/config-set">命令文档</see></para>
    /// </remarks>
    public bool ConfigSet(Dictionary<string, string> parameterValues, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ConfigSet(parameterValues) | flags, cancellationToken);
    }

    /// <summary>
    /// Starts a coordinated failover to a replica selected by the primary.
    /// <para>Available since: 6.2.0</para>
    /// <para>发起协调故障转移，由主节点选择从节点。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="timeout">Time in milliseconds for the primary to wait for replica synchronization. Omit to wait indefinitely.
    /// <para>主节点等待从节点追平的时间，单位为毫秒，须大于 0；省略时持续等待。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis accepts the request. The role change completes in the background.
    /// <para>Redis 接受切换请求时返回 true，角色切换在后台完成。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/failover">Command documentation</see>
    /// <para><see href="https://redis.io/commands/failover">命令文档</see></para>
    /// </remarks>
    public bool Failover(long? timeout = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Failover(timeout) | flags, cancellationToken);
    }

    /// <summary>
    /// Starts a coordinated failover to the specified replica.
    /// <para>Available since: 6.2.0</para>
    /// <para>发起协调故障转移，将指定的从节点提升为主节点。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="host">The replica host as known to the primary.
    /// <para>主节点识别的目标从节点地址。</para>
    /// </param>
    /// <param name="port">The replica listening port.
    /// <para>目标从节点的监听端口。</para>
    /// </param>
    /// <param name="force">Promote the specified replica when the timeout expires even if it has not caught up. Requires timeout.
    /// <para>超时后即使目标从节点尚未追平也强制提升，可能丢失写入，必须指定 timeout。</para>
    /// </param>
    /// <param name="timeout">Time in milliseconds for the primary to wait for replica synchronization. Omit to wait indefinitely.
    /// <para>主节点等待从节点追平的时间，单位为毫秒，须大于 0；省略时持续等待。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis accepts the request. The role change completes in the background.
    /// <para>Redis 接受切换请求时返回 true，角色切换在后台完成。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/failover">Command documentation</see>
    /// <para><see href="https://redis.io/commands/failover">命令文档</see></para>
    /// </remarks>
    public bool Failover(string host, int port, bool force = false, long? timeout = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Failover(host, port, force, timeout) | flags, cancellationToken);
    }

    /// <summary>
    /// Aborts a coordinated failover in progress.
    /// <para>Available since: 6.2.0</para>
    /// <para>中止正在进行的协调故障转移。</para>
    /// <para>支持此命令的Redis版本, 6.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis acknowledges the abort.
    /// <para>Redis 确认中止操作时返回 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/failover">Command documentation</see>
    /// <para><see href="https://redis.io/commands/failover">命令文档</see></para>
    /// </remarks>
    public bool FailoverAbort(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.FailoverAbort() | flags, cancellationToken);
    }

    /// <summary>
    /// The INFO command returns information and statistics about the server in a format that is simple to parse by computers and easy to read by humans.
    /// <para>Available since: 1.0.0</para>
    /// <para>返回 Redis 服务器的各种信息和统计数值，包括服务器状态、客户端连接、内存使用、持久化、统计、复制、CPU、集群、模块等详细信息</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/info">Command documentation</see>
    /// <para><see href="https://redis.io/commands/info">命令文档</see></para>
    /// </remarks>
    public string Info(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Info() | flags, cancellationToken)!;
    }

    /// <summary>
    /// The LOLWUT command displays the Redis version: however as a side effect of doing so, it also creates a piece of generative computer art that is different with each version of Redis.
    /// <para>Available since: 5.0.0</para>
    /// <para>返回一幅由 ASCII 字符组成的随机艺术作品，用于测试和娱乐</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="version">Request a specific version of the generated art.
    /// <para>指定版本号，默认最新版本</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/lolwut">Command documentation</see>
    /// <para><see href="https://redis.io/commands/lolwut">命令文档</see></para>
    /// </remarks>
    public string Lolwut(int? version = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Lolwut(version) | flags, cancellationToken)!;
    }

    /// <summary>
    /// Lists the modules loaded on the server.
    /// <para>Available since: 4.0.0</para>
    /// <para>列出服务端已加载的模块。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Loaded modules, or an empty array if none are loaded.
    /// <para>已加载的模块，未加载任何模块时返回空数组。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/module-list">Command documentation</see>
    /// <para><see href="https://redis.io/commands/module-list">命令文档</see></para>
    /// </remarks>
    public ModuleInfo[] ModuleList(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ModuleList() | flags, cancellationToken);
    }

    /// <summary>
    /// Loads a module from a library on the Redis server.
    /// <para>Available since: 4.0.0</para>
    /// <para>从 Redis 服务端的库文件加载模块。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="path">Path to the module library on the Redis server.
    /// <para>Redis 服务端上的模块库文件路径。</para>
    /// </param>
    /// <param name="args">Arguments passed to the module in order.
    /// <para>按顺序传给模块的参数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/module-load">Command documentation</see>
    /// <para><see href="https://redis.io/commands/module-load">命令文档</see></para>
    /// </remarks>
    public bool ModuleLoad(string path, SharpValueSpan args = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ModuleLoad(path, args) | flags, cancellationToken);
    }

    /// <summary>
    /// Loads a module with configuration values and module arguments.
    /// <para>Available since: 7.0.0</para>
    /// <para>加载模块，并设置模块配置项和启动参数。</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="path">Path to the module library on the Redis server.
    /// <para>Redis 服务端上的模块库文件路径。</para>
    /// </param>
    /// <param name="configs">Module configuration names and values. Each pair is sent as a separate CONFIG option.
    /// <para>模块配置名称和值，每一对配置对应一个 CONFIG 选项。</para>
    /// </param>
    /// <param name="args">Arguments passed to the module in order.
    /// <para>按顺序传给模块的参数。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/module-loadex">Command documentation</see>
    /// <para><see href="https://redis.io/commands/module-loadex">命令文档</see></para>
    /// </remarks>
    public bool ModuleLoadEx(string path, ReadOnlySpan<KeyValuePair<string, SharpValue>> configs = default, SharpValueSpan args = default, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ModuleLoadEx(path, configs, args) | flags, cancellationToken);
    }

    /// <summary>
    /// Unloads a module by its registered name.
    /// <para>Available since: 4.0.0</para>
    /// <para>按注册名称卸载模块。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="name">Registered module name returned by MODULE LIST.
    /// <para>MODULE LIST 返回的模块注册名称。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/module-unload">Command documentation</see>
    /// <para><see href="https://redis.io/commands/module-unload">命令文档</see></para>
    /// </remarks>
    public bool ModuleUnload(string name, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ModuleUnload(name) | flags, cancellationToken);
    }

    /// <summary>
    /// Configures the current instance as a replica of the specified primary.
    /// <para>Available since: 5.0.0</para>
    /// <para>将当前实例设为指定主节点的从节点，并开始同步数据。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="host">The hostname or IP address of the primary.
    /// <para>主节点的主机名或 IP 地址</para>
    /// </param>
    /// <param name="port">The listening port of the primary.
    /// <para>主节点的监听端口</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/replicaof">Command documentation</see>
    /// <para><see href="https://redis.io/commands/replicaof">命令文档</see></para>
    /// </remarks>
    public bool ReplicaOf(string host, int port, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ReplicaOf(host, port) | flags, cancellationToken);
    }

    /// <summary>
    /// Stops replication and promotes the current instance to primary, retaining its data.
    /// <para>Available since: 5.0.0</para>
    /// <para>执行 REPLICAOF NO ONE，停止复制并将当前实例提升为主节点，保留已有数据。</para>
    /// <para>支持此命令的Redis版本, 5.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/replicaof">Command documentation</see>
    /// <para><see href="https://redis.io/commands/replicaof">命令文档</see></para>
    /// </remarks>
    public bool ReplicaOfNoOne(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ReplicaOfNoOne() | flags, cancellationToken);
    }

    /// <summary>
    /// The SAVE commands performs a synchronous save of the dataset producing a point in time snapshot of all the data inside the Redis instance, in the form of an RDB file.
    /// <para>Available since: 1.0.0</para>
    /// <para>同步执行 RDB 快照保存。阻塞所有客户端请求直到保存完成</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/save">Command documentation</see>
    /// <para><see href="https://redis.io/commands/save">命令文档</see></para>
    /// </remarks>
    public bool Save(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Save() | flags, cancellationToken);
    }

    /// <summary>
    /// Shuts down the Redis server.
    /// <para>Available since: 1.0.0</para>
    /// <para>关闭 Redis 服务端，成功关闭时连接会断开。</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="save">True for SAVE, false for NOSAVE, or null to follow the server configuration.
    /// <para>true 表示强制保存 RDB，false 表示跳过 RDB 保存，null 表示遵循服务端配置。</para>
    /// </param>
    /// <param name="now">Skip waiting for replicas to catch up (Redis 7.0+).
    /// <para>跳过等待从节点追平的阶段，Redis 7.0+ 支持。</para>
    /// </param>
    /// <param name="force">Proceed even if persistence errors would normally prevent shutdown (Redis 7.0+).
    /// <para>即使持久化错误通常会阻止关机，也强制继续，Redis 7.0+ 支持。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <remarks>
    /// <see href="https://redis.io/commands/shutdown">Command documentation</see>
    /// <para><see href="https://redis.io/commands/shutdown">命令文档</see></para>
    /// </remarks>
    public void Shutdown(bool? save = null, bool now = false, bool force = false, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        _ = base.Invoke(ServerCommands.Shutdown(save, now, force) | flags, cancellationToken);
    }

    /// <summary>
    /// Aborts a pending shutdown.
    /// <para>Available since: 7.0.0</para>
    /// <para>中止正在等待的关机操作。</para>
    /// <para>支持此命令的Redis版本, 7.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 确认中止关机时返回 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/shutdown">Command documentation</see>
    /// <para><see href="https://redis.io/commands/shutdown">命令文档</see></para>
    /// </remarks>
    public bool ShutdownAbort(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.ShutdownAbort() | flags, cancellationToken);
    }

    /// <summary>
    /// Configures the current instance as a replica of the specified primary.
    /// <para>Available since: 1.0.0</para>
    /// <para>将当前实例设为指定主节点的从节点，并开始同步数据。</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="host">The hostname or IP address of the primary.
    /// <para>主节点的主机名或 IP 地址</para>
    /// </param>
    /// <param name="port">The listening port of the primary.
    /// <para>主节点的监听端口</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// Deprecated since Redis 5.0.0. Use REPLICAOF on Redis 5.0 and later.
    /// <para>Redis 5.0.0 起弃用此命令，建议使用 REPLICAOF。</para>
    /// <see href="https://redis.io/commands/slaveof">Command documentation</see>
    /// <para><see href="https://redis.io/commands/slaveof">命令文档</see></para>
    /// </remarks>
    public bool SlaveOf(string host, int port, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.SlaveOf(host, port) | flags, cancellationToken);
    }

    /// <summary>
    /// Stops replication and promotes the current instance to primary, retaining its data.
    /// <para>Available since: 1.0.0</para>
    /// <para>执行 SLAVEOF NO ONE，停止复制并将当前实例提升为主节点，保留已有数据。</para>
    /// <para>支持此命令的Redis版本, 1.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// Deprecated since Redis 5.0.0. Use REPLICAOF on Redis 5.0 and later.
    /// <para>Redis 5.0.0 起弃用此命令，建议使用 REPLICAOF。</para>
    /// <see href="https://redis.io/commands/slaveof">Command documentation</see>
    /// <para><see href="https://redis.io/commands/slaveof">命令文档</see></para>
    /// </remarks>
    public bool SlaveOfNoOne(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.SlaveOfNoOne() | flags, cancellationToken);
    }

    /// <summary>
    /// Reads the most recent slow log entries.
    /// <para>Available since: 2.2.12</para>
    /// <para>获取最近的慢查询日志。</para>
    /// <para>支持此命令的Redis版本, 2.2.12+</para>
    /// </summary>
    /// <param name="count">Maximum number of entries. Omit for the Redis default of 10, or pass -1 for all entries.
    /// <para>最多返回的条目数，省略时 Redis 默认返回最近 10 条，传入 -1 返回全部。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Entries newest first, or an empty array when no entries are returned.
    /// <para>按时间从新到旧排列的日志条目，无记录时返回空数组。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/slowlog-get">Command documentation</see>
    /// <para><see href="https://redis.io/commands/slowlog-get">命令文档</see></para>
    /// </remarks>
    public SlowLogEntry[] SlowLogGet(long? count = null, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.SlowLogGet(count) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the number of entries in the slow log.
    /// <para>Available since: 2.2.12</para>
    /// <para>获取慢查询日志当前的条目数。</para>
    /// <para>支持此命令的Redis版本, 2.2.12+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The current entry count.
    /// <para>当前日志条目数。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/slowlog-len">Command documentation</see>
    /// <para><see href="https://redis.io/commands/slowlog-len">命令文档</see></para>
    /// </remarks>
    public long SlowLogLen(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.SlowLogLen() | flags, cancellationToken);
    }

    /// <summary>
    /// Clears the slow log on the selected server.
    /// <para>Available since: 2.2.12</para>
    /// <para>清空目标节点的慢查询日志。</para>
    /// <para>支持此命令的Redis版本, 2.2.12+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/slowlog-reset">Command documentation</see>
    /// <para><see href="https://redis.io/commands/slowlog-reset">命令文档</see></para>
    /// </remarks>
    public bool SlowLogReset(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.SlowLogReset() | flags, cancellationToken);
    }

    /// <summary>
    /// Swaps the contents of two Redis databases atomically.
    /// <para>Available since: 4.0.0</para>
    /// <para>原子交换两个 Redis 数据库的内容。</para>
    /// <para>支持此命令的Redis版本, 4.0.0+</para>
    /// </summary>
    /// <param name="index1">Zero-based index of the first database.
    /// <para>第一个数据库的下标，从 0 开始。</para>
    /// </param>
    /// <param name="index2">Zero-based index of the second database.
    /// <para>第二个数据库的下标，从 0 开始。</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>True when Redis replies with OK.
    /// <para>Redis 返回 OK 时为 true。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/swapdb">Command documentation</see>
    /// <para><see href="https://redis.io/commands/swapdb">命令文档</see></para>
    /// </remarks>
    public bool SwapDB(int index1, int index2, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.SwapDB(index1, index2) | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the current time of the Redis server.
    /// <para>Available since: 2.6.0</para>
    /// <para>获取 Redis 服务端的当前时间。</para>
    /// <para>支持此命令的Redis版本, 2.6.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>The whole-second timestamp in the client's local time zone and the microseconds within that second.
    /// <para>客户端本地时区的整秒时间戳，以及该秒内的微秒数。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/time">Command documentation</see>
    /// <para><see href="https://redis.io/commands/time">命令文档</see></para>
    /// </remarks>
    public TimeResult Time(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Time() | flags, cancellationToken);
    }

    /// <summary>
    /// Returns the role and replication details of the selected Redis instance.
    /// <para>Available since: 2.8.12</para>
    /// <para>获取所选 Redis 实例的角色及复制信息。</para>
    /// <para>支持此命令的Redis版本, 2.8.12+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns>Primary, replica or Sentinel information according to the reported role.
    /// <para>根据实际角色返回主节点、从节点或哨兵的信息。</para>
    /// </returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/role">Command documentation</see>
    /// <para><see href="https://redis.io/commands/role">命令文档</see></para>
    /// </remarks>
    public RoleResult Role(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
    {
        return base.Invoke(ServerCommands.Role() | flags, cancellationToken);
    }
}
