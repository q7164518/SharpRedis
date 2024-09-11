#if !LOW_NET
using SharpRedis.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisServer
    {
        /// <summary>
        /// Delete all the keys of all the existing databases, not just the currently selected one. This command never fails.
        /// <para>Available since: 1.0.0 | 4.0.0 | 6.2.0</para>
        /// <para>删除Redis中所有数据库中所有的Key</para>
        /// <para>支持此命令的Redis版本, 1.0.0 | 4.0.0 | 6.2.0</para>
        /// </summary>
        /// <param name="mode">Flushing mode modifier.
        /// <para>刷新模式</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> FlushAllAsync(FlushingMode mode = FlushingMode.None, CancellationToken cancellationToken = default)
        {
            return this._call.CallConditionAsync(ServerCommands.FlushAll(mode), "OK", cancellationToken);
        }

        /// <summary>
        /// Delete all the keys of the currently selected DB. This command never fails.
        /// <para>Available since: 1.0.0 | 4.0.0 | 6.2.0</para>
        /// <para>删除当前选择的数据库中的所有Key.</para>
        /// <para>支持此命令的Redis版本, 1.0.0 | 4.0.0 | 6.2.0</para>
        /// </summary>
        /// <param name="mode">Flushing mode modifier
        /// <para>刷新模式</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> FlushDbAsync(FlushingMode mode = FlushingMode.None, CancellationToken cancellationToken = default)
        {
            return this._call.CallConditionAsync(ServerCommands.FlushDb(mode), "OK", cancellationToken);
        }

        /// <summary>
        /// The SAVE commands performs a synchronous save of the dataset producing a point in time snapshot of all the data inside the Redis instance, in the form of an RDB file.
        /// <para>Available since: 1.0.0</para>
        /// <para>将Redis中所有的数据同步持久化到RDB文件</para>
        /// <para>生产环境中, 不建议你使用此命令. 因为SAVE命令是同步执行的, 如果你的数据较大, 将会阻塞后续命令</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            return this._call.CallConditionAsync(ServerCommands.Save(), "OK", cancellationToken);
        }

        /// <summary>
        /// Save the DB in background
        /// <para>Available since: 1.0.0 | 3.2.2</para>
        /// <para>在后台执行数据持久化</para>
        /// <para>支持此命令的Redis版本, 1.0.0+ | 3.2.2+</para>
        /// </summary>
        /// <param name="schedule">Available since: 3.2.2. If BGSAVE SCHEDULE is used, the command will immediately return OK when an AOF rewrite is in progress and schedule the background save to run at the next opportunity
        /// <para>Redis 3.2.2+才支持设置为true. 当 AOF 重写正在进行时，该命令将立即返回 OK ，并安排后台保存在下一次​​机会运行</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> BgSaveAsync(bool schedule = false, CancellationToken cancellationToken = default)
#else
        public Task<string> BgSaveAsync(bool schedule = false, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStringAsync(ServerCommands.BgSave(schedule), cancellationToken);
        }

        /// <summary>
        /// Return the number of keys in the currently-selected database
        /// <para>Available since: 1.0.0</para>
        /// <para>获得当前数据库Key的数量</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of keys in the currently-selected database
        /// <para>当前数据库Key数量</para>
        /// </returns>
        public Task<long> DbSizeAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(ServerCommands.DbSize(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The INFO command returns information and statistics about the server in a format that is simple to parse by computers and easy to read by humans
        /// <para>Available since: 1.0.0</para>
        /// <para>获得Redis服务信息</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="sections"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> InfoAsync(string[]? sections = null, CancellationToken cancellationToken = default)
#else
        public Task<string> InfoAsync(string[] sections = null, CancellationToken cancellationToken = default)
#endif
        {
#if NET8_0_OR_GREATER
            return base._call.CallStringAsync(ServerCommands.Info(sections ?? []), cancellationToken);
#else
            return base._call.CallStringAsync(ServerCommands.Info(sections ?? Array.Empty<string>()), cancellationToken);
#endif
        }

        /// <summary>
        /// Return the unix second timestamp of the last DB save executed with success
        /// <para>Available since: 1.0.0</para>
        /// <para>获得最后一次持久化的时间戳, 秒级时间戳</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<long> LastSaveAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(ServerCommands.LastSave(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Instruct Redis to start an Append Only File rewrite process. The rewrite will create a small optimized version of the current Append Only File
        /// <para>Available since: 1.0.0</para>
        /// <para>指示 Redis 启动仅追加文件重写过程</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?> BgRewriteAofAsync(CancellationToken cancellationToken = default)
#else
        public Task<string> BgRewriteAofAsync(CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStringAsync(ServerCommands.BgRewriteAof(), cancellationToken);
        }

        /// <summary>
        /// Returns Integer reply of number of total commands in this Redis server
        /// <para>Available since: 2.8.13</para>
        /// <para>获得Redis服务支持的命令总数</para>
        /// <para>支持此命令的Redis版本, 2.8.13+</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<long> CommandCountAsync(CancellationToken cancellationToken = default)
        {
            return base._call.CallNumberAsync<long>(ServerCommands.CommandCount(), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns Array reply of keys from a full Redis command
        /// <para>Available since: 2.8.13</para>
        /// <para>从一个Redis命令中提取所有Key</para>
        /// <para>支持此命令的Redis版本, 2.8.13+</para>
        /// </summary>
        /// <param name="command">Redis command
        /// <para>Redis命令</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>List of keys from the given command
        /// <para>提取的Key数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<string?[]?> CommandGetKeysAsync(string command, CancellationToken cancellationToken = default)
#else
        public Task<string[]> CommandGetKeysAsync(string command, CancellationToken cancellationToken = default)
#endif
        {
            if (string.IsNullOrEmpty(command)) throw new ArgumentNullException(nameof(command));
#if NET8_0_OR_GREATER
            return base._call.CallClassArrayAsync<string>(ServerCommands.CommandGetKeys(command.Split([' '], StringSplitOptions.RemoveEmptyEntries)), ResultType.Array | ResultType.String, cancellationToken);
#else
            return base._call.CallClassArrayAsync<string>(ServerCommands.CommandGetKeys(command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)), ResultType.Array | ResultType.String, cancellationToken);
#endif
        }
    }
}
#endif
