using SharpRedis.Enums;
using SharpRedis.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharpRedis;

public sealed partial class RedisServer
{
    /// <summary>
    /// ACL user information returned by ACL GETUSER.
    /// <para>Available since: Redis 6.0.0</para>
    /// <para>ACL GETUSER 返回的用户权限配置信息。</para>
    /// <para>兼容 RESP2 和 RESP3，以及 Redis 6.0、6.2、7.0+ 的返回结构。</para>
    /// <para>支持此命令的 Redis 版本：6.0.0+</para>
    /// </summary>
    public sealed class AclUserInfo
    {
        /// <summary>
        /// ACL user flags.
        /// <para>ACL 用户标记，例如 on、off、nopass、allkeys、allcommands。</para>
        /// </summary>
#if MODERN_NET
        public string[]? Flags { get; private set; }
#else
        public string[] Flags { get; private set; }
#endif

        /// <summary>
        /// SHA-256 password hashes registered for this user.
        /// <para>用户配置的密码 SHA-256 哈希值集合。</para>
        /// <para>返回的是密码哈希，不是明文密码。</para>
        /// <para>nopass 用户通常返回空数组。</para>
        /// </summary>
#if MODERN_NET
        public string[]? Passwords { get; private set; }
#else
        public string[] Passwords { get; private set; }
#endif

        /// <summary>
        /// Effective command permission rules.
        /// <para>当前用户根权限中的有效命令规则。</para>
        /// <para>例如：-@all +get +set 或 +@all。</para>
        /// <para>该规则不一定与创建用户时输入的规则字符串完全一致。</para>
        /// </summary>
        public string Commands { get; private set; } = string.Empty;

        /// <summary>
        /// Key access rules.
        /// <para>用户可以访问的 Key 权限规则。</para>
        /// <para>Redis 7.0+ 原样保留服务端返回的规则字符串。</para>
        /// <para>Redis 6.x 的旧格式数组会转换为规则字符串。</para>
        /// <para>可能包含 ~、%R~、%W~、%RW~ 等规则。</para>
        /// </summary>
        public string Keys { get; private set; } = string.Empty;

        /// <summary>
        /// Pub/Sub channel access rules.
        /// <para>Available since: Redis 6.2.0</para>
        /// <para>允许访问的 Pub/Sub 频道规则。</para>
        /// <para>例如：&amp;news:* 或 &amp;*。</para>
        /// <para>Redis 6.0 不提供此字段，返回空字符串。</para>
        /// </summary>
#if MODERN_NET
        public string? Channels { get; private set; }
#else
        public string Channels { get; private set; }
#endif

        /// <summary>
        /// Additional ACL selectors.
        /// <para>Available since: Redis 7.0.0</para>
        /// <para>附加的 ACL 权限选择器，按服务器返回顺序保存。</para>
        /// <para>Redis 7.0 之前返回空数组。</para>
        /// </summary>
#if MODERN_NET
        public AclSelectorInfo[]? Selectors { get; private set; }
#else
        public AclSelectorInfo[] Selectors { get; private set; }
#endif

        /// <summary>
        /// Original key patterns from the legacy reply format.
        /// <para>Redis 6.x 返回的原始 Key 模式数组。</para>
        /// <para>Redis 7.0+ 使用规则字符串，此属性为空数组。</para>
        /// </summary>
#if MODERN_NET
        public string[]? LegacyKeyPatterns { get; private set; }
#else
        public string[] LegacyKeyPatterns { get; private set; }
#endif

        /// <summary>
        /// Original channel patterns from the legacy reply format.
        /// <para>Redis 6.2.x 返回的原始频道模式数组。</para>
        /// <para>Redis 7.0+ 使用规则字符串，此属性为空数组。</para>
        /// </summary>
#if MODERN_NET
        public string[]? LegacyChannelPatterns { get; private set; }
#else
        public string[] LegacyChannelPatterns { get; private set; }
#endif

        /// <summary>
        /// Whether the ACL user is enabled.
        /// <para>ACL 用户是否启用，即是否包含 on 标记。</para>
        /// </summary>
        public bool IsEnabled => this.HasFlag("on");

        /// <summary>
        /// Whether the user has the nopass flag.
        /// <para>是否具有 nopass 标记，即允许不校验密码进行认证。</para>
        /// <para>此属性不表示用户一定具有其他命令访问权限。</para>
        /// </summary>
        public bool NoPassword => this.HasFlag("nopass");

        /// <summary>
        /// Whether the root rule has the allkeys flag.
        /// <para>根权限规则是否具有 allkeys 标记。</para>
        /// <para>不包含附加 Selector 的权限。</para>
        /// </summary>
        public bool AllKeys => this.HasFlag("allkeys");

        /// <summary>
        /// Whether the root rule has the allcommands flag.
        /// <para>根权限规则是否具有 allcommands 标记。</para>
        /// <para>不包含附加 Selector 的权限。</para>
        /// </summary>
        public bool AllCommands => this.HasFlag("allcommands");

        /// <summary>
        /// Checks whether the user has the specified ACL flag.
        /// <para>判断用户是否具有指定 ACL 标记。</para>
        /// </summary>
        /// <param name="flag">ACL flag. ACL 标记名称。</param>
        /// <returns>True if present. 存在时返回 true。</returns>
        public bool HasFlag(string flag)
        {
            if (this.Flags is null or { Length: 0 }) return false;
            foreach (var item in this.Flags)
            {
                if (string.Equals(item, flag, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parses an existing ACL user.
        /// <para>解析已存在用户的 ACL GETUSER 回复。</para>
        /// </summary>
        internal AclUserInfo(RedisResult value)
        {
            AclReplyReader.ReadFields(value, (field, data) =>
            {
                switch (field.ToLowerInvariant())
                {
                    case "flags":
                        this.Flags = AclReplyReader.ReadStrings(data);
                        break;
                    case "passwords":
                        this.Passwords = AclReplyReader.ReadStrings(data);
                        break;
                    case "commands":
                        this.Commands = data!;
                        break;
                    case "keys":
                        if (data.Type == RespType.Array)
                        {
                            this.LegacyKeyPatterns = AclReplyReader.ReadStrings(data);
                            this.Keys = AclReplyReader.JoinLegacyRules(this.LegacyKeyPatterns, "~");
                        }
                        else
                        {
                            this.Keys = data!;
                        }
                        break;
                    case "channels":
                        if (data.Type == RespType.Array)
                        {
                            this.LegacyChannelPatterns = AclReplyReader.ReadStrings(data);
                            this.Channels = AclReplyReader.JoinLegacyRules(this.LegacyChannelPatterns, "&");
                        }
                        else
                        {
                            this.Channels = data!;
                        }
                        break;
                    case "selectors":
                        this.Selectors = AclReplyReader.ReadSelectors(data);
                        break;
                    default: break;
                }
            });
        }

#if MODERN_NET
        internal static AclUserInfo? Parse(RedisResult value)
#else
        internal static AclUserInfo Parse(RedisResult value)
#endif
        {
            return value.IsNull ? null : new AclUserInfo(value);
        }
    }

    /// <summary>
    /// Additional ACL permission selector.
    /// <para>一个 Selector 包含自己的命令、Key 和频道权限规则。</para>
    /// </summary>
    public sealed class AclSelectorInfo
    {
        /// <summary>
        /// Effective command rules for this selector.
        /// <para>该选择器中的命令权限规则。</para>
        /// </summary>
#if MODERN_NET
        public string? Commands { get; private set; }
#else
        public string Commands { get; private set; }
#endif

        /// <summary>
        /// Key access rules for this selector.
        /// <para>该选择器中的 Key 访问规则。</para>
        /// </summary>
#if MODERN_NET
        public string? Keys { get; private set; }
#else
        public string Keys { get; private set; }
#endif

        /// <summary>
        /// Pub/Sub channel access rules for this selector.
        /// <para>该选择器中的 Pub/Sub 频道访问规则。</para>
        /// </summary>
#if MODERN_NET
        public string? Channels { get; private set; }
#else
        public string Channels { get; private set; }
#endif

        internal AclSelectorInfo(RedisResult value)
        {
            AclReplyReader.ReadFields(value, (field, data) =>
            {
                switch (field.ToLowerInvariant())
                {
                    case "commands":
                        this.Commands = data!;
                        break;

                    case "keys":
                        this.Keys = AclReplyReader.ReadRuleString(data, "~");
                        break;

                    case "channels":
                        this.Channels = AclReplyReader.ReadRuleString(data, "&");
                        break;
                }
            });
        }
    }

    internal static class AclReplyReader
    {
        internal static void ReadFields(RedisResult value, Action<string, RedisResult> fieldReader)
        {
            if (value.Type == RespType.Array && value.RawValue is RedisResult[] values)
            {
                if ((values.Length & 1) != 0)
                {
                    throw new InvalidCastException("Invalid ACL GETUSER field-value array.");
                }
                for (int i = 0; i < values.Length; i += 2)
                {
                    fieldReader(values[i]!, values[i + 1]);
                }
                return;
            }

            if (value.Type == RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
            {
                foreach (var pair in maps)
                {
                    fieldReader(pair.Key!, pair.Value);
                }
                return;
            }
            throw new InvalidCastException("Invalid ACL GETUSER response structure.");
        }

        /// <summary>
        /// Reads strings from RESP2 Array or RESP3 Array/Set.
        /// <para>从 RESP2 Array 或 RESP3 Array/Set 读取字符串集合。</para>
        /// </summary>
        internal static string[] ReadStrings(RedisResult value)
        {
            if ((value.Type != RespType.Array &&
                 value.Type != RespType.Set) ||
                value.RawValue is not RedisResult[] values)
            {
                throw new InvalidCastException("Invalid ACL GETUSER string collection.");
            }

            var result = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = values[i]!;
            }
            return result;
        }

        /// <summary>
        /// Reads a key or channel rule in old or new format.
        /// <para>兼容旧版模式数组和新版 ACL 权限规则字符串。</para>
        /// </summary>
        internal static string ReadRuleString(RedisResult value, string prefix)
        {
            if (value.Type == RespType.Array)
            {
                return AclReplyReader.JoinLegacyRules(AclReplyReader.ReadStrings(value), prefix);
            }

            return value!;
        }

        /// <summary>
        /// Converts legacy pattern arrays to ACL-style rule strings.
        /// <para>将旧版模式数组转换为 ACL 风格的规则字符串。</para>
        /// <para>已包含规则前缀的条目不会重复添加前缀。</para>
        /// </summary>
        internal static string JoinLegacyRules(string[] patterns, string prefix)
        {
            if (patterns.Length == 0) return string.Empty;

            var rules = new string[patterns.Length];
            for (int i = 0; i < patterns.Length; i++)
            {
                var pattern = patterns[i];
                if (pattern.StartsWith(prefix, StringComparison.Ordinal))
                {
                    rules[i] = pattern;
                }
                else
                {
                    rules[i] = prefix + pattern;
                }
            }
            return string.Join(" ", rules);
        }

        /// <summary>
        /// Reads ACL selectors.
        /// <para>解析 Redis 7.0+ 的 ACL Selector 数组。</para>
        /// </summary>
#if MODERN_NET
        internal static AclSelectorInfo[]? ReadSelectors(RedisResult value)
#else
        internal static AclSelectorInfo[] ReadSelectors(RedisResult value)
#endif
        {
            if (value.Type != RespType.Array)
            {
                throw new InvalidCastException("Invalid ACL GETUSER selectors.");
            }
            if (value.IsNullOrEmpty) return null;
            if (value.RawValue is RedisResult[] values)
            {
                var result = new AclSelectorInfo[values.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    result[i] = new AclSelectorInfo(values[i]);
                }
                return result;
            }
            throw new InvalidCastException("Invalid ACL GETUSER selectors.");
        }
    }

    /// <summary>
    /// ACL security log entry.
    /// <para>ACL LOG 命令返回的单条安全审计日志。</para>
    /// <para>记录认证失败、命令权限不足、Key 权限不足以及频道权限不足等事件。</para>
    /// </summary>
    public sealed class AclLogEntry
    {
        /// <summary>
        /// Number of security events represented by this entry.
        /// <para>该日志记录代表的安全事件发生次数。</para>
        /// <para>Redis 会将短时间内相同的安全事件聚合到一条记录中。</para>
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Reason for the ACL security event.
        /// <para>ACL 安全事件发生的原因。</para>
        /// <para>可能为 auth、command、key、channel。</para>
        /// <para>auth：认证失败。</para>
        /// <para>command：命令权限不足。</para>
        /// <para>key：Key 访问权限不足。</para>
        /// <para>channel：Pub/Sub 频道权限不足。</para>
        /// </summary>
        public string Reason { get; private set; } = string.Empty;

        /// <summary>
        /// Execution context of the security event.
        /// <para>发生安全事件时的执行上下文。</para>
        /// <para>可能为 toplevel、multi、lua、module。</para>
        /// </summary>
        public string Context { get; private set; } = string.Empty;

        /// <summary>
        /// Resource associated with the denied operation.
        /// <para>导致 ACL 权限检查失败的资源。</para>
        /// <para>可能是命令名、Key、频道名或认证操作。</para>
        /// </summary>
        public string Object { get; private set; } = string.Empty;

        /// <summary>
        /// Username associated with the security event.
        /// <para>触发安全事件的 ACL 用户名。</para>
        /// <para>认证失败时，可能是客户端尝试认证的用户名。</para>
        /// </summary>
        public string Username { get; private set; } = string.Empty;

        /// <summary>
        /// Age of the log entry, in seconds.
        /// <para>日志距离当前查询时间经过的秒数。</para>
        /// <para>该值可能包含小数。</para>
        /// <para>例如 8.038 表示约 8.038 秒。</para>
        /// </summary>
        public double AgeSeconds { get; private set; }

        /// <summary>
        /// Client information associated with the security event.
        /// <para>发生安全事件时的客户端连接信息。</para>
        /// <para>格式与 CLIENT INFO 命令的返回内容类似。</para>
        /// <para>保留完整原始字符串，不进行字段拆分。</para>
        /// </summary>
        public string ClientInfo { get; private set; } = string.Empty;

        /// <summary>
        /// Unique sequential ID of the ACL log entry.
        /// <para>Available since: Redis 7.2.0</para>
        /// <para>ACL 日志条目的顺序 ID。</para>
        /// <para>可用于识别两次查询之间是否遗漏了日志。</para>
        /// <para>Redis 7.2 之前返回 null。</para>
        /// </summary>
        public long? EntryId { get; private set; }

        /// <summary>
        /// UNIX timestamp in milliseconds when the entry was created.
        /// <para>Available since: Redis 7.2.0</para>
        /// <para>日志首次创建时的 Unix 毫秒时间戳。</para>
        /// <para>Redis 7.2 之前返回 null。</para>
        /// </summary>
        public long? TimestampCreated { get; private set; }

        /// <summary>
        /// UNIX timestamp in milliseconds when the entry was last updated.
        /// <para>Available since: Redis 7.2.0</para>
        /// <para>日志最后一次更新时的 Unix 毫秒时间戳。</para>
        /// <para>当相同安全事件被聚合时，此时间可能更新。</para>
        /// <para>Redis 7.2 之前返回 null。</para>
        /// </summary>
        public long? TimestampLastUpdated { get; private set; }

        /// <summary>
        /// Creation time in UTC.
        /// <para>日志首次创建的 UTC 时间。</para>
        /// <para>Redis 7.2 之前返回 null。</para>
        /// </summary>
        public DateTimeOffset? CreatedAt =>
            this.TimestampCreated.HasValue
                ? DateTimeOffset.FromUnixTimeMilliseconds(
                    this.TimestampCreated.Value)
                : null;

        /// <summary>
        /// Last update time in UTC.
        /// <para>日志最后更新时间（UTC）。</para>
        /// <para>Redis 7.2 之前返回 null。</para>
        /// </summary>
        public DateTimeOffset? LastUpdatedAt =>
            this.TimestampLastUpdated.HasValue
                ? DateTimeOffset.FromUnixTimeMilliseconds(
                    this.TimestampLastUpdated.Value)
                : null;

        /// <summary>
        /// Whether this security event represents an authentication failure.
        /// <para>是否为认证失败事件。</para>
        /// </summary>
        public bool IsAuthenticationFailure =>
            string.Equals(
                this.Reason,
                "auth",
                StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Whether this security event represents a command permission denial.
        /// <para>是否为命令权限不足事件。</para>
        /// </summary>
        public bool IsCommandDenied =>
            string.Equals(
                this.Reason,
                "command",
                StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Whether this security event represents a key permission denial.
        /// <para>是否为 Key 访问权限不足事件。</para>
        /// </summary>
        public bool IsKeyDenied =>
            string.Equals(
                this.Reason,
                "key",
                StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Whether this security event represents a channel permission denial.
        /// <para>是否为频道访问权限不足事件。</para>
        /// </summary>
        public bool IsChannelDenied =>
            string.Equals(
                this.Reason,
                "channel",
                StringComparison.OrdinalIgnoreCase);

        internal AclLogEntry(RedisResult value)
        {
            if (value.Type is RespType.Array && value.RawValue is RedisResult[] values)
            {
                if ((values.Length & 1) != 0)
                {
                    throw new InvalidCastException("Invalid ACL LOG entry array length.");
                }

                for (int i = 0; i < values.Length; i += 2)
                {
                    string name = values[i]!;
                    this.SetField(name, values[i + 1]);
                }
            }
            else if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
            {
                foreach (var pair in maps)
                {
                    string name = pair.Key!;
                    this.SetField(name, pair.Value);
                }
            }
            else
            {
                throw new InvalidCastException("Invalid ACL LOG entry response.");
            }
        }

        /// <summary>
        /// Assigns a field returned by Redis.
        /// <para>根据 Redis 返回的字段名设置对应属性。</para>
        /// </summary>
        private void SetField(string name, RedisResult value)
        {
            switch (name.ToLowerInvariant())
            {
                case "count":
                    this.Count = value;
                    break;

                case "reason":
                    this.Reason = ReadString(value);
                    break;

                case "context":
                    this.Context = ReadString(value);
                    break;

                case "object":
                    this.Object = ReadString(value);
                    break;

                case "username":
                    this.Username = ReadString(value);
                    break;

                case "age-seconds":
                    this.AgeSeconds = double.Parse(
                        ReadString(value),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture);
                    break;

                case "client-info":
                    this.ClientInfo = ReadString(value);
                    break;

                case "entry-id":
                    this.EntryId = value.IsNull
                        ? null
                        : (long)value;
                    break;

                case "timestamp-created":
                    this.TimestampCreated = value.IsNull
                        ? null
                        : (long)value;
                    break;

                case "timestamp-last-updated":
                    this.TimestampLastUpdated = value.IsNull
                        ? null
                        : (long)value;
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Reads a nullable Redis string.
        /// <para>读取 Redis 字符串，Redis Null 转换为空字符串。</para>
        /// </summary>
        private static string ReadString(RedisResult value)
        {
            return value.IsNull ? string.Empty : (string)value!;
        }

        /// <summary>
        /// Parses the complete ACL LOG reply.
        /// <para>解析 ACL LOG 返回的全部安全日志记录。</para>
        /// <para>保留 Redis 返回顺序，即从最新记录到最旧记录。</para>
        /// <para>没有日志时返回空数组。</para>
        /// </summary>
        internal static AclLogEntry[] Parse(RedisResult value)
        {
            if (value.Type is not RespType.Array || value.RawValue is not RedisResult[] values)
            {
                throw new InvalidCastException("Invalid ACL LOG response.");
            }

            var result = new AclLogEntry[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                result[i] = new AclLogEntry(values[i]);
            }

            return result;
        }
    }

    /// <summary>
    /// Hot keys tracking information returned by HOTKEYS GET.
    /// <para>HOTKEYS GET 命令返回的热点 Key 监控结果。</para>
    /// <para>包含监控状态、采样配置、CPU/网络统计和热点 Key 排名。</para>
    /// </summary>
    public sealed class HotKeysInfo
    {
        /// <summary>
        /// Whether hotkeys tracking is active.
        /// <para>热点 Key 监控是否正在运行。</para>
        /// </summary>
        public bool TrackingActive { get; private set; }

        /// <summary>
        /// Command sampling ratio.
        /// <para>监控命令的采样比例。</para>
        /// <para>1 表示每条命令都参与采样。</para>
        /// <para>N 表示大约每 N 条命令采样一次。</para>
        /// </summary>
        public long SampleRatio { get; private set; }

        /// <summary>
        /// Selected hash slot ranges.
        /// <para>监控的哈希槽范围。</para>
        /// <para>未指定 SLOTS 时通常涵盖当前节点负责的所有槽位。</para>
        /// </summary>
#if MODERN_NET
        public HotKeySlotRange[]? SelectedSlots { get; private set; }
#else
        public HotKeySlotRange[] SelectedSlots { get; private set; }
#endif

        /// <summary>
        /// CPU time of all commands on all slots, in microseconds.
        /// <para>所有槽位上的命令累计 CPU 时间，单位为微秒。</para>
        /// </summary>
        public long AllCommandsAllSlotsMicroseconds { get; private set; }

        /// <summary>
        /// Network bytes of all commands on all slots.
        /// <para>所有槽位上命令的累计网络字节数。</para>
        /// </summary>
        public long NetBytesAllCommandsAllSlots { get; private set; }

        /// <summary>
        /// Collection start time as a Unix timestamp in milliseconds.
        /// <para>热点监控开始时间，Unix 毫秒时间戳。</para>
        /// </summary>
        public long CollectionStartTimeUnixMs { get; private set; }

        /// <summary>
        /// Duration of the tracking collection, in milliseconds.
        /// <para>热点监控的采集持续时间，单位毫秒。</para>
        /// </summary>
        public long CollectionDurationMs { get; private set; }

        /// <summary>
        /// User CPU time measured for hotkeys tracking, in milliseconds.
        /// <para>仅启用 CPU 指标时提供。</para>
        /// <para>用户态 CPU 时间，单位毫秒。</para>
        /// <para>没有启用该指标时返回 null。</para>
        /// </summary>
        public long? TotalCpuTimeUserMs { get; private set; }

        /// <summary>
        /// System CPU time measured for hotkeys tracking, in milliseconds.
        /// <para>仅启用 CPU 指标时提供。</para>
        /// <para>内核态 CPU 时间，单位毫秒。</para>
        /// <para>没有启用该指标时返回 null。</para>
        /// </summary>
        public long? TotalCpuTimeSysMs { get; private set; }

        /// <summary>
        /// Total network bytes measured for hotkeys tracking.
        /// <para>仅启用 NET 指标时提供。</para>
        /// <para>采集期间统计的网络总字节数。</para>
        /// <para>没有启用该指标时返回 null。</para>
        /// </summary>
        public long? TotalNetBytes { get; private set; }

        /// <summary>
        /// Hot keys ordered by CPU time in microseconds.
        /// <para>仅启用 CPU 指标时提供。</para>
        /// <para>按照 CPU 消耗时间从大到小排序的热点 Key。</para>
        /// <para>每项 Value 表示该 Key 消耗的 CPU 时间（微秒）。</para>
        /// <para>未启用 CPU 指标时返回 null。</para>
        /// <para>启用了 CPU 指标但没有热点数据时返回空数组。</para>
        /// </summary>
#if MODERN_NET
        public HotKeyMetric[]? ByCpuTimeUs { get; private set; }
#else
        public HotKeyMetric[] ByCpuTimeUs { get; private set; }
#endif

        /// <summary>
        /// Hot keys ordered by network bytes.
        /// <para>仅启用 NET 指标时提供。</para>
        /// <para>按照网络流量从大到小排序的热点 Key。</para>
        /// <para>每项 Value 表示该 Key 的网络字节数。</para>
        /// <para>未启用 NET 指标时返回 null。</para>
        /// <para>启用了 NET 指标但没有热点数据时返回空数组。</para>
        /// </summary>
#if MODERN_NET
        public HotKeyMetric[]? ByNetBytes { get; private set; }
#else
        public HotKeyMetric[] ByNetBytes { get; private set; }
#endif

        /// <summary>
        /// CPU time for sampled commands in selected slots, in microseconds.
        /// <para>在集群模式指定 SLOTS 和 SAMPLE 时提供。</para>
        /// <para>指定槽位中被采样命令的 CPU 时间，单位微秒。</para>
        /// <para>该字段未返回时为 null。</para>
        /// </summary>
        public long? SampledCommandsSelectedSlotsUs { get; private set; }

        /// <summary>
        /// Collection start time in UTC.
        /// <para>热点监控开始的 UTC 时间。</para>
        /// </summary>
        public DateTimeOffset CollectionStartTime => DateTimeOffset.FromUnixTimeMilliseconds(this.CollectionStartTimeUnixMs);

        internal HotKeysInfo(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: 1 } outer)
            {
                throw new InvalidCastException("Invalid HOTKEYS GET outer response.");
            }

            RedisResult data = outer[0];
            if (data.Type == RespType.Array && data.RawValue is RedisResult[] values)
            {
                if ((values.Length & 1) != 0)
                {
                    throw new InvalidCastException("Invalid HOTKEYS GET fields.");
                }

                for (int i = 0; i < values.Length; i += 2)
                {
                    string field = values[i]!;
                    this.SetField(field, values[i + 1]);
                }
            }
            else if (data.Type == RespType.Maps && data.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
            {
                foreach (var pair in maps)
                {
                    string field = pair.Key!;
                    this.SetField(field, pair.Value);
                }
            }
            else
            {
                throw new InvalidCastException("Invalid HOTKEYS GET field structure.");
            }
        }

        /// <summary>
        /// Assigns a field returned by Redis.
        /// <para>根据 Redis 字段名设置对应属性。</para>
        /// </summary>
        private void SetField(string field, RedisResult value)
        {
            switch (field)
            {
                case "tracking-active":
                    this.TrackingActive = value;
                    break;

                case "sample-ratio":
                    this.SampleRatio = value;
                    break;

                case "selected-slots":
                    this.SelectedSlots = HotKeysInfo.ParseSlots(value);
                    break;

                case "all-commands-all-slots-us":
                    this.AllCommandsAllSlotsMicroseconds = value;
                    break;

                case "net-bytes-all-commands-all-slots":
                    this.NetBytesAllCommandsAllSlots = value;
                    break;

                case "collection-start-time-unix-ms":
                    this.CollectionStartTimeUnixMs = value;
                    break;

                case "collection-duration-ms":
                    this.CollectionDurationMs = value;
                    break;

                case "total-cpu-time-user-ms":
                    this.TotalCpuTimeUserMs = value;
                    break;

                case "total-cpu-time-sys-ms":
                    this.TotalCpuTimeSysMs = value;
                    break;

                case "total-net-bytes":
                    this.TotalNetBytes = value;
                    break;

                case "by-cpu-time-us":
                    this.ByCpuTimeUs = HotKeysInfo.ParseMetrics(value);
                    break;

                case "by-net-bytes":
                    this.ByNetBytes = HotKeysInfo.ParseMetrics(value);
                    break;

                case "sampled-commands-selected-slots-us":
                    this.SampledCommandsSelectedSlotsUs = value;
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Parses selected slot ranges.
        /// <para>解析被监控的哈希槽范围。</para>
        /// </summary>
        private static HotKeySlotRange[] ParseSlots(RedisResult value)
        {
            if (value.Type != RespType.Array ||
                value.RawValue is not RedisResult[] slots)
            {
                throw new InvalidCastException(
                    "Invalid HOTKEYS GET selected-slots.");
            }

            var result = new HotKeySlotRange[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                result[i] = new HotKeySlotRange(slots[i]);
            }
            return result;
        }

        /// <summary>
        /// Parses an array of alternating key and metric values.
        /// <para>解析 Key、数值交替排列的热点统计数组。</para>
        /// </summary>
#if MODERN_NET
        private static HotKeyMetric[]? ParseMetrics(RedisResult value)
#else
        private static HotKeyMetric[] ParseMetrics(RedisResult value)
#endif
        {
            if (value.IsNullOrEmpty) return null;
            if (value.Type != RespType.Array ||
                value.RawValue is not RedisResult[] values ||
                (values.Length & 1) != 0)
            {
                throw new InvalidCastException("Invalid HOTKEYS GET metric array.");
            }

            var result = new HotKeyMetric[values.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new HotKeyMetric(
                    values[i * 2],
                    values[i * 2 + 1]);
            }
            return result;
        }

        /// <summary>
        /// Hot key metric entry.
        /// <para>Available since: Redis 8.6.0</para>
        /// <para>单个热点 Key 的资源消耗统计。</para>
        /// <para>用于 by-cpu-time-us 和 by-net-bytes 排名。</para>
        /// </summary>
        public readonly struct HotKeyMetric
        {
            /// <summary>
            /// Name of the hot key.
            /// <para>热点 Key 名称。</para>
            /// <para>保留 RedisResult，避免强制转换成字符串。</para>
            /// <para>Redis 返回的是实际存储的 Key 名称。</para>
            /// </summary>
            public readonly RedisResult Key { get; }

            /// <summary>
            /// Measured resource consumption for the key.
            /// <para>热点 Key 的资源消耗数值。</para>
            /// <para>在 ByCpuTimeUs 中表示微秒。</para>
            /// <para>在 ByNetBytes 中表示字节数。</para>
            /// </summary>
            public readonly long Value { get; }

            /// <summary>
            /// Initializes a key-metric pair.
            /// <para>构造热点 Key 资源消耗记录。</para>
            /// </summary>
            internal HotKeyMetric(RedisResult key, RedisResult value)
            {
                this.Key = key;
                this.Value = value;
            }
        }

        /// <summary>
        /// Selected hash slot range.
        /// <para>Available since: Redis 8.6.0</para>
        /// <para>HOTKEYS 监控选中的哈希槽范围。</para>
        /// </summary>
        public readonly struct HotKeySlotRange
        {
            /// <summary>
            /// Starting hash slot, inclusive.
            /// <para>起始哈希槽，包含此槽位。</para>
            /// </summary>
            public readonly int Start { get; }

            /// <summary>
            /// Ending hash slot, inclusive.
            /// <para>结束哈希槽，包含此槽位。</para>
            /// </summary>
            public readonly int End { get; }

            /// <summary>
            /// Initializes a slot range from an array reply.
            /// <para>从 Redis 返回的两个整数解析槽位范围。</para>
            /// </summary>
            internal HotKeySlotRange(RedisResult value)
            {
                if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: 2 } values)
                {
                    throw new InvalidCastException("Invalid HOTKEYS GET slot range.");
                }

                this.Start = values[0];
                this.End = values[1];

                if (this.Start < 0 || this.End > 16383 || this.Start > this.End)
                {
                    throw new InvalidCastException("Invalid HOTKEYS GET slot boundaries.");
                }
            }

            /// <summary>
            /// Whether the specified hash slot is within this range.
            /// <para>判断指定哈希槽是否在当前范围内。</para>
            /// </summary>
            public bool Contains(int slot)
            {
                return slot >= this.Start && slot <= this.End;
            }
        }
    }

    /// <summary>
    /// Redis backup status information.
    /// <para>BACKUP STATUS 命令返回的备份状态信息。</para>
    /// <para>包含备份状态、错误信息、开始时间和封存时间。</para>
    /// </summary>
    public readonly struct BackupStatusInfo
    {
        /// <summary>
        /// Current backup state.
        /// <para>当前备份状态。</para>
        /// <para>可能为 Idle、Pending、Snapshotting、Incrementing、Sealed、Failed。</para>
        /// <para>无法识别的状态返回 Unknown。</para>
        /// </summary>
        public readonly BackupState State { get; }

        /// <summary>
        /// Original backup state returned by Redis.
        /// <para>Redis 返回的原始备份状态字符串。</para>
        /// <para>例如 idle、pending、snapshotting、incrementing、sealed、failed。</para>
        /// <para>用于兼容未来 Redis 新增的备份状态。</para>
        /// </summary>
        public readonly string StateName { get; }

        /// <summary>
        /// Error message associated with the backup.
        /// <para>备份失败或被取消时的错误信息。</para>
        /// <para>没有错误时为空字符串。</para>
        /// </summary>
        public readonly string Error { get; }

        /// <summary>
        /// Unix timestamp when the backup started, in seconds.
        /// <para>备份开始的 Unix 时间戳，单位为秒。</para>
        /// <para>如果没有开始过备份，返回 0。</para>
        /// </summary>
        public readonly long StartTime { get; }

        /// <summary>
        /// Unix timestamp when the backup was sealed, in seconds.
        /// <para>备份完成封存时的 Unix 时间戳，单位为秒。</para>
        /// <para>未执行 BACKUP SEAL 或尚未封存时返回 0。</para>
        /// </summary>
        public readonly long EndTime { get; }

        /// <summary>
        /// Backup start time as DateTimeOffset.
        /// <para>备份开始时间。</para>
        /// <para>StartTime 为 0 时返回 null。</para>
        /// </summary>
        public readonly DateTimeOffset? StartedAt =>
            this.StartTime > 0
                ? DateTimeOffset.FromUnixTimeSeconds(this.StartTime)
                : null;

        /// <summary>
        /// Backup sealing time as DateTimeOffset.
        /// <para>备份完成封存的时间。</para>
        /// <para>EndTime 为 0 时返回 null。</para>
        /// </summary>
        public readonly DateTimeOffset? SealedAt =>
            this.EndTime > 0
                ? DateTimeOffset.FromUnixTimeSeconds(this.EndTime)
                : null;

        /// <summary>
        /// Whether a backup is currently in progress.
        /// <para>是否存在正在执行的备份。</para>
        /// <para>Pending、Snapshotting、Incrementing 状态返回 true。</para>
        /// </summary>
        public readonly bool IsRunning =>
            this.State is BackupState.Pending
                or BackupState.Snapshotting
                or BackupState.Incrementing;

        /// <summary>
        /// Whether the backup is sealed.
        /// <para>备份是否已经完成封存。</para>
        /// </summary>
        public readonly bool IsSealed => this.State == BackupState.Sealed;

        /// <summary>
        /// Whether the backup has failed or been aborted.
        /// <para>备份是否失败或已被取消。</para>
        /// </summary>
        public readonly bool IsFailed => this.State == BackupState.Failed;

        internal BackupStatusInfo(RedisResult value)
        {
#if MODERN_NET
            string? state = null;
            string? error = null;
#else
            string state = null;
            string error = null;
#endif
            long startTime = 0;
            long endTime = 0;

            bool hasState = false;
            bool hasError = false;
            bool hasStartTime = false;
            bool hasEndTime = false;

            if (value.Type is RespType.Array && value.RawValue is RedisResult[] values && (values.Length & 1) == 0)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    string field = values[i]!;
                    switch (field)
                    {
                        case "state":
                            state = values[i + 1];
                            hasState = true;
                            break;

                        case "error":
                            error = values[i + 1];
                            hasError = true;
                            break;

                        case "start_time":
                            startTime = values[i + 1];
                            hasStartTime = true;
                            break;

                        case "end_time":
                            endTime = values[i + 1];
                            hasEndTime = true;
                            break;
                    }
                }
            }
            else if (value.Type is RespType.Maps && value.RawValue is KeyValuePair<RedisResult, RedisResult>[] maps)
            {
                foreach (var item in maps)
                {
                    string field = item.Key!;
                    switch (field)
                    {
                        case "state":
                            state = item.Value;
                            hasState = true;
                            break;

                        case "error":
                            error = item.Value;
                            hasError = true;
                            break;

                        case "start_time":
                            startTime = item.Value;
                            hasStartTime = true;
                            break;

                        case "end_time":
                            endTime = item.Value;
                            hasEndTime = true;
                            break;
                    }
                }
            }
            else
            {
                throw new InvalidCastException("Invalid BACKUP STATUS response.");
            }

            if (!hasState || !hasError ||
                !hasStartTime || !hasEndTime ||
                state is null || error is null)
            {
                throw new InvalidCastException("Missing required BACKUP STATUS fields.");
            }

            this.StateName = state;
            this.State = ParseState(state);
            this.Error = error;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        /// <summary>
        /// Converts the Redis backup state string to an enum.
        /// <para>将 Redis 返回的备份状态字符串转换为枚举。</para>
        /// <para>无法识别的状态返回 Unknown。</para>
        /// </summary>
        private static BackupState ParseState(string state)
        {
            return state switch
            {
                "idle" => BackupState.Idle,
                "pending" => BackupState.Pending,
                "snapshotting" => BackupState.Snapshotting,
                "incrementing" => BackupState.Incrementing,
                "sealed" => BackupState.Sealed,
                "failed" => BackupState.Failed,
                _ => BackupState.Unknown,
            };
        }

        /// <summary>
        /// Redis backup state.
        /// <para>Redis BACKUP STATUS 命令返回的备份状态。</para>
        /// </summary>
        public enum BackupState : byte
        {
            /// <summary>
            /// Unknown backup state.
            /// <para>未知备份状态，通常表示当前 Redis 版本返回了尚未适配的状态。</para>
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// No backup is in progress.
            /// <para>空闲状态，当前没有正在执行的备份。</para>
            /// </summary>
            Idle,

            /// <summary>
            /// A backup was requested but is waiting for an AOF rewrite.
            /// <para>备份已请求，正在等待 AOF 重写操作可用。</para>
            /// </summary>
            Pending,

            /// <summary>
            /// Redis is creating the base snapshot.
            /// <para>正在创建基础快照（BASE）。</para>
            /// </summary>
            Snapshotting,

            /// <summary>
            /// Base snapshot is complete and incremental writes are being accumulated.
            /// <para>基础快照已完成，正在累积增量写入，等待执行 BACKUP SEAL。</para>
            /// </summary>
            Incrementing,

            /// <summary>
            /// The backup has been sealed and its files are immutable.
            /// <para>备份已封存，备份文件集合完整且不可修改，等待消费和清理。</para>
            /// </summary>
            Sealed,

            /// <summary>
            /// The backup failed or was aborted.
            /// <para>备份失败或已被 BACKUP ABORT 取消。</para>
            /// <para>可以重新执行 BACKUP START。</para>
            /// </summary>
            Failed,
        }
    }

    /// <summary>
    /// Command metadata returned by COMMAND and COMMAND INFO.
    /// <para>命令元数据，旧版 Redis 未提供的集合字段返回空数组。</para>
    /// </summary>
    public sealed class CommandMetadata
    {
        /// <summary>
        /// Command name.
        /// <para>命令名称。</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Argument count including the command name. A negative value denotes a minimum.
        /// <para>包括命令名在内的参数个数，负值表示最小个数。</para>
        /// </summary>
        public int Arity { get; }

        /// <summary>
        /// Command flags.
        /// <para>命令标记。</para>
        /// </summary>
        public string[] Flags { get; }

        /// <summary>
        /// Position of the first key.
        /// <para>第一个 key 的参数位置。</para>
        /// </summary>
        public int FirstKey { get; }

        /// <summary>
        /// Position of the last key.
        /// <para>最后一个 key 的参数位置。</para>
        /// </summary>
        public int LastKey { get; }

        /// <summary>
        /// Distance between successive key arguments.
        /// <para>相邻 key 的参数位置间隔。</para>
        /// </summary>
        public int KeyStep { get; }

        /// <summary>
        /// ACL categories.
        /// <para>命令所属 ACL 分类。</para>
        /// </summary>
        public string[] AclCategories { get; }

        /// <summary>
        /// Command tips.
        /// <para>命令使用提示。</para>
        /// </summary>
        public string[] Tips { get; }

        /// <summary>
        /// Key specification records, including begin_search and find_keys.
        /// <para>各 key 规范记录，包括 begin_search 和 find_keys。</para>
        /// </summary>
        public CommandKeySpecification[] KeySpecifications { get; }

        /// <summary>
        /// Subcommand metadata.
        /// <para>子命令元数据。</para>
        /// </summary>
        public CommandMetadata[] Subcommands { get; }

        internal CommandMetadata(RedisResult value)
        {
            var row = (RedisResult[])value.RawValue;
            this.Name = row[0]!;
            this.Arity = row[1];
            this.Flags = ReadCommandStrings(row[2]);
            this.FirstKey = (int)row[3];
            this.LastKey = (int)row[4];
            this.KeyStep = (int)row[5];
            this.AclCategories = row.Length > 6 ? ReadCommandStrings(row[6]) : [];
            this.Tips = row.Length > 7 ? ReadCommandStrings(row[7]) : [];
            this.KeySpecifications = row.Length > 8 && row[8].RawValue is RedisResult[] specs
                ? Array.ConvertAll(specs, static spec => new CommandKeySpecification(spec)) : [];
            this.Subcommands = row.Length > 9 && row[9].RawValue is RedisResult[] children
                ? Array.ConvertAll(children, static child => new CommandMetadata(child)) : [];
        }
    }

    /// <summary>
    /// Documentation for one command.
    /// <para>单个命令的文档信息。</para>
    /// </summary>
    public sealed class CommandDocumentation
    {
        /// <summary>
        /// Command name.
        /// <para>命令名称。</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Short description.
        /// <para>命令摘要。</para>
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// Redis version that introduced the command.
        /// <para>引入该命令的 Redis 版本。</para>
        /// </summary>
        public string Since { get; }

        /// <summary>
        /// Command group.
        /// <para>命令分组。</para>
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Time complexity, or an empty string when omitted.
        /// <para>时间复杂度，服务端未提供时为空字符串。</para>
        /// </summary>
        public string Complexity { get; }

        /// <summary>
        /// Documented arguments in declaration order.
        /// <para>按声明顺序排列的参数文档。</para>
        /// </summary>
        public CommandArgument[] Arguments { get; }

        /// <summary>
        /// Documentation indexed by subcommand name.
        /// <para>按子命令名称索引的文档。</para>
        /// </summary>
        public Dictionary<string, CommandDocumentation> Subcommands { get; }

        /// <summary>
        /// All fields supplied by Redis, including optional history and deprecation details.
        /// <para>服务端返回的全部字段，包含可选的版本历史及弃用信息。</para>
        /// </summary>
        public Dictionary<string, RedisResult> Fields { get; }

        internal CommandDocumentation(string name, RedisResult value)
        {
            this.Name = name;
            this.Fields = ReadCommandFields(value);
            this.Summary = CommandText(this.Fields, "summary");
            this.Since = CommandText(this.Fields, "since");
            this.Group = CommandText(this.Fields, "group");
            this.Complexity = CommandText(this.Fields, "complexity");
            this.Arguments = ReadCommandArguments(this.Fields);
            this.Subcommands = this.Fields.TryGetValue("subcommands", out var children) ? children.ToCommandDocumentation() : [];
        }
    }

    /// <summary>
    /// A documented command argument.
    /// <para>命令文档中的参数，支持嵌套参数。</para>
    /// </summary>
    public sealed class CommandArgument
    {
        /// <summary>
        /// Argument name.
        /// <para>参数名。</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Redis argument type.
        /// <para>Redis 参数类型。</para>
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Nested arguments of a block or oneof.
        /// <para>block 或 oneof 中的子参数。</para>
        /// </summary>
        public CommandArgument[] Arguments { get; }

        /// <summary>
        /// All argument attributes supplied by Redis.
        /// <para>服务端返回的全部参数属性。</para>
        /// </summary>
        public Dictionary<string, RedisResult> Fields { get; }

        internal CommandArgument(RedisResult value)
        {
            this.Fields = ReadCommandFields(value);
            this.Name = CommandText(this.Fields, "name");
            this.Type = CommandText(this.Fields, "type");
            this.Arguments = ReadCommandArguments(this.Fields);
        }
    }

    /// <summary>
    /// A key and its access flags.
    /// <para>命令中的 key 及其访问标记。</para>
    /// </summary>
    public readonly struct CommandKeyFlags
    {
        /// <summary>
        /// Key as returned by Redis.
        /// <para>Redis 返回的 key。</para>
        /// </summary>
        public readonly RedisResult Key { get; }

        /// <summary>
        /// Access flags for this key.
        /// <para>该 key 的访问标记。</para>
        /// </summary>
        public readonly string[] Flags { get; }

        internal CommandKeyFlags(RedisResult value)
        {
            var pair = (RedisResult[])value.RawValue;
            this.Key = pair[0];
            this.Flags = ReadCommandStrings(pair[1]);
        }
    }

    private static string[] ReadCommandStrings(RedisResult value)
        => value.IsNull ? [] : AclReplyReader.ReadStrings(value);

    private static Dictionary<string, RedisResult> ReadCommandFields(RedisResult value)
    {
        var fields = new Dictionary<string, RedisResult>(StringComparer.Ordinal);
        if (!value.IsNull) AclReplyReader.ReadFields(value, fields.Add);
        return fields;
    }

    private static string CommandText(Dictionary<string, RedisResult> fields, string key)
        => fields.TryGetValue(key, out var value) ? value! : string.Empty;

    private static CommandArgument[] ReadCommandArguments(Dictionary<string, RedisResult> fields)
        => fields.TryGetValue("arguments", out var arguments) && arguments.RawValue is RedisResult[] rows
            ? Array.ConvertAll(rows, static row => new CommandArgument(row)) : [];

    /// <summary>
    /// A key specification from COMMAND or COMMAND INFO.
    /// <para>命令的 key 定位规则，包含起点、查找方式和访问标记。</para>
    /// </summary>
    public readonly struct CommandKeySpecification
    {
        /// <summary>
        /// Key access flags.
        /// <para>此规则对应的 key 访问标记。</para>
        /// </summary>
        public readonly string[] Flags { get; }

        /// <summary>
        /// Additional notes, or an empty string if omitted.
        /// <para>附加说明，未提供时为空字符串。</para>
        /// </summary>
        public readonly string Notes { get; }

        /// <summary>
        /// How Redis finds the first argument to inspect.
        /// <para>如何定位开始查找 key 的参数。</para>
        /// </summary>
        public readonly CommandKeySearch BeginSearch { get; }

        /// <summary>
        /// How Redis locates the keys from that position.
        /// <para>从起点查找各个 key 的方式。</para>
        /// </summary>
        public readonly CommandKeySearch FindKeys { get; }

        internal CommandKeySpecification(RedisResult value)
        {
            var fields = ReadCommandFields(value);
            this.Flags = fields.TryGetValue("flags", out var flags) ? ReadCommandStrings(flags) : [];
            this.Notes = CommandText(fields, "notes");
            this.BeginSearch = new(fields["begin_search"]);
            this.FindKeys = new(fields["find_keys"]);
        }
    }

    /// <summary>
    /// A key search method and its type-specific parameters.
    /// <para>key 定位方式及对应参数，例如 index、keyword、range 或 keynum。</para>
    /// </summary>
    public readonly struct CommandKeySearch
    {
        /// <summary>
        /// Search method name.
        /// <para>定位方式名称。</para>
        /// </summary>
        public readonly string Type { get; }

        /// <summary>
        /// Parameters supplied for this method.
        /// <para>此定位方式的参数，如 index 或 lastkey。</para>
        /// </summary>
        public readonly Dictionary<string, RedisResult> Specification { get; }

        internal CommandKeySearch(RedisResult value)
        {
            var fields = ReadCommandFields(value);
            this.Type = CommandText(fields, "type");
            this.Specification = ReadCommandFields(fields["spec"]);
        }
    }

    /// <summary>
    /// Information about a module loaded on the Redis server.
    /// <para>Redis 服务端已加载模块的信息。</para>
    /// </summary>
    public readonly struct ModuleInfo
    {
        /// <summary>
        /// Registered module name.
        /// <para>模块注册名称，可传给 MODULE UNLOAD。</para>
        /// </summary>
        public readonly string Name { get; }

        /// <summary>
        /// Version number reported by the module.
        /// <para>模块声明的整数版本号。</para>
        /// </summary>
        public readonly int Version { get; }

        /// <summary>
        /// Module library path, or null when omitted by the server.
        /// <para>模块库路径，服务端未提供时为 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? Path { get; }
#else
        public readonly string Path { get; }
#endif

        /// <summary>
        /// Arguments passed to the module on load, in their original order.
        /// <para>按原始顺序返回加载模块时传入的参数，无参数或服务端未提供时为空数组。</para>
        /// </summary>
        public readonly RedisResult[] Args { get; }

        internal ModuleInfo(RedisResult value)
        {
            var fields = ReadCommandFields(value);
            this.Name = (string)fields["name"]!;
            this.Version = (int)fields["ver"];
            this.Path = fields.TryGetValue("path", out var path) ? path.ToString() : null;
            this.Args = fields.TryGetValue("args", out var args) && args.RawValue is RedisResult[] items ? items : [];
        }
    }

    /// <summary>
    /// An entry returned by SLOWLOG GET.
    /// <para>SLOWLOG GET 返回的慢查询日志条目。</para>
    /// </summary>
    public readonly struct SlowLogEntry
    {
        /// <summary>
        /// Unique identifier of the entry.
        /// <para>日志条目的唯一标识。</para>
        /// </summary>
        public readonly long Id { get; }

        /// <summary>
        /// Unix timestamp in seconds.
        /// <para>命令执行时的 Unix 时间戳，单位为秒。</para>
        /// </summary>
        public readonly long Timestamp { get; }

        /// <summary>
        /// Command execution time in microseconds.
        /// <para>命令执行耗时，单位为微秒。</para>
        /// </summary>
        public readonly long DurationMicroseconds { get; }

        /// <summary>
        /// Logged command and arguments, possibly truncated by Redis.
        /// <para>记录的命令名和参数，Redis 可能截断内容。</para>
        /// </summary>
        public readonly RedisResult[] Arguments { get; }

        /// <summary>
        /// Client address, or null when omitted.
        /// <para>客户端地址，服务端未提供时为 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? ClientAddress { get; }
#else
        public readonly string ClientAddress { get; }
#endif

        /// <summary>
        /// Client name, or null when omitted.
        /// <para>客户端名称，未命名时为空字符串，服务端未提供时为 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? ClientName { get; }
#else
        public readonly string ClientName { get; }
#endif

        /// <summary>
        /// Original argument count before truncation, when supplied.
        /// <para>截断前的原始参数总数，服务端未提供时为 null。</para>
        /// </summary>
        public readonly long? TotalArgumentCount { get; }

        internal SlowLogEntry(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] array)
            {
                throw new InvalidCastException("Invalid SLOWLOG GET entry array length.");
            }
            this.Id = array[0];
            this.Timestamp = array[1];
            this.DurationMicroseconds = array[2];
            this.Arguments = array[3]!;
            this.ClientAddress = array.Length > 4 ? array[4].ToString() : null;
            this.ClientName = array.Length > 5 ? array[5].ToString() : null;
            this.TotalArgumentCount = array.Length > 6 ? (long)array[6] : null;
        }
    }

    /// <summary>
    /// Server time returned by TIME.
    /// <para>TIME 返回的服务端时间。</para>
    /// </summary>
    public readonly struct TimeResult
    {
        /// <summary>
        /// Server Unix timestamp in whole seconds, expressed in the client's local time zone.
        /// <para>服务端的整秒时间戳，已转换为客户端本地时区。</para>
        /// </summary>
        public readonly DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Microseconds elapsed within the reported second (0 to 999999).
        /// <para>当前秒内已过去的微秒数，范围为 0 到 999999。</para>
        /// </summary>
        public readonly int Microseconds { get; }

        internal TimeResult(RedisResult value)
        {
            if (value.Type != RespType.Array || value.RawValue is not RedisResult[] { Length: 2 } array)
            {
                throw new InvalidCastException("Invalid TIME entry array length.");
            }
            this.Timestamp = DateTimeOffset.FromUnixTimeSeconds(array[0]).ToLocalTime();
            this.Microseconds = array[1];
        }
    }

    /// <summary>
    /// A connected replica reported by a primary.
    /// <para>主节点报告的已连接从节点。</para>
    /// </summary>
    public readonly struct RoleReplica
    {
        /// <summary>
        /// Replica address.
        /// <para>从节点地址。</para>
        /// </summary>
        public readonly string Host { get; }

        /// <summary>
        /// Replica listening port.
        /// <para>从节点监听端口。</para>
        /// </summary>
        public readonly int Port { get; }

        /// <summary>
        /// Last replication offset acknowledged by the replica.
        /// <para>从节点最近确认的复制偏移量。</para>
        /// </summary>
        public readonly long ReplicationOffset { get; }

        internal RoleReplica(RedisResult value)
        {
            if (value.RawValue is not RedisResult[] { Length: 3 } row)
                throw new InvalidCastException("Invalid ROLE replica entry.");
            this.Host = (string)row[0]!;
            this.Port = row[1];
            this.ReplicationOffset = row[2];
        }
    }

    /// <summary>
    /// Role and replication details of the instance that answered ROLE.
    /// <para>执行 ROLE 的实例角色及其复制信息。</para>
    /// </summary>
    public readonly struct RoleResult
    {
        /// <summary>
        /// Role name as returned by Redis: master, slave or sentinel.
        /// <para>Redis 返回的角色名：master、slave 或 sentinel。</para>
        /// </summary>
        public readonly string Role { get; }

        /// <summary>
        /// Replication offset for a primary or replica.
        /// <para>主节点或从节点的复制偏移量，哨兵返回 null。</para>
        /// </summary>
        public readonly long? ReplicationOffset { get; }

        /// <summary>
        /// Connected replicas reported by a primary.
        /// <para>主节点报告的已连接从节点，其他角色返回空数组。</para>
        /// </summary>
        public readonly RoleReplica[] Replicas { get; }

        /// <summary>
        /// Primary address reported by a replica.
        /// <para>从节点报告的主节点地址，其他角色返回 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? MasterHost { get; }
#else
        public readonly string MasterHost { get; }
#endif

        /// <summary>
        /// Primary listening port reported by a replica.
        /// <para>从节点报告的主节点端口，其他角色返回 null。</para>
        /// </summary>
        public readonly int? MasterPort { get; }

        /// <summary>
        /// Replication link state reported by a replica.
        /// <para>从节点报告的复制连接状态，其他角色返回 null。</para>
        /// </summary>
#if MODERN_NET
        public readonly string? ReplicationState { get; }
#else
        public readonly string ReplicationState { get; }
#endif

        /// <summary>
        /// Names of primaries monitored by a Sentinel.
        /// <para>哨兵监控的主节点名称，其他角色返回空数组。</para>
        /// </summary>
        public readonly string[] MonitoredMasters { get; }

        internal RoleResult(RedisResult value)
        {
            if (value.RawValue is not RedisResult[] { Length: > 0 } row)
                throw new InvalidCastException("Invalid ROLE response.");
            this.Role = (string)row[0]!;
            this.Replicas = [];
            this.MonitoredMasters = [];
            switch (this.Role)
            {
                case "master" when row.Length >= 3:
                    this.ReplicationOffset = (long)row[1];
                    if (!row[2].IsNull)
                        this.Replicas = Array.ConvertAll((RedisResult[])row[2].RawValue, static replica => new RoleReplica(replica));
                    break;
                case "slave" when row.Length >= 5:
                    this.MasterHost = row[1].ToString();
                    this.MasterPort = (int)row[2];
                    this.ReplicationState = row[3].ToString();
                    this.ReplicationOffset = (long)row[4];
                    break;
                case "sentinel" when row.Length >= 2:
                    this.MonitoredMasters = ReadCommandStrings(row[1]);
                    break;
                default:
                    throw new InvalidCastException("Invalid ROLE response for " + this.Role);
            }
        }
    }
}
