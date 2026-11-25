using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Provider.Standard;

/// <summary>
/// Redis transaction interface. This is not safe in a multi-threaded environment.
/// <para>Redis事务接口, 在多线程中是非安全的</para>
/// </summary>
public interface IRedisTranClient : IRedis
{
    /// <summary>
    /// The return value gets from executing the transaction
    /// <para>执行事务获得的返回值</para>
    /// </summary>
    public readonly struct TranResult
    {
#if MODERN_NET
        private readonly RedisResult[]? _result;
        private readonly RedisResult[]? _queueResult;
#else
        private readonly RedisResult[] _result;
        private readonly RedisResult[] _queueResult;
#endif

        /// <summary>
        /// Gets the return value of the command added to the queue
        /// <para>获取每个命令加入事务队列时的返回值</para>
        /// </summary>
#if MODERN_NET
        public readonly RedisResult[]? Queued => this._queueResult;
#else
        public readonly RedisResult[] Queued => this._queueResult;
#endif

        /// <summary>
        /// Gets the return value of each command in the transaction
        /// <para>获取事务中, 每个命令执行后的返回值</para>
        /// </summary>
#if MODERN_NET
        public readonly RedisResult[]? Result => this._result;
#else
        public readonly RedisResult[] Result => this._result;
#endif

        /// <summary>
        /// Indicates whether the value should be considered a null value
        /// <para>当前值是不是应该视为null</para>
        /// </summary>
        public readonly bool IsNull
        {
            get
            {
                if (this._result is null) return true;
                return false;
            }
        }

        internal TranResult(RedisResult result, RedisResult[] queueResult)
        {
            this._queueResult = queueResult;

            if (result.RawValue is RedisResult[] array)
            {
                this._result = array;
            }
            else if (result.IsNull)
            {
                this._result = null;
            }
            else
            {
                throw new System.FormatException("Unparsable transaction return value");
            }
        }
    }

    /// <summary>
    /// Flushes all previously queued commands in a transaction and restores the connection state to normal.
    /// <para>If [WATCH] was used, DISCARD unwatches all keys watched by the connection.</para>
    /// <para>Available since: 2.0.0</para>
    /// <para>清除并取消执行当前事务中所有的命令. 如果你之前调用了[WATCH], 则会取消所有之前所有监视的Key</para>
    /// <para>支持的Redis版本: 2.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    bool Discard(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all previously queued commands in a transaction and restores the connection state to normal.
    /// <para>If [WATCH] was used, DISCARD unwatches all keys watched by the connection.</para>
    /// <para>Available since: 2.0.0</para>
    /// <para>清除并取消执行当前事务中所有的命令. 如果你之前调用了[WATCH], 则会取消所有之前所有监视的Key</para>
    /// <para>支持的Redis版本: 2.0.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    Task<bool> DiscardAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the start of a transaction block. Subsequent commands will be queued for atomic execution using EXEC.
    /// <para>Available since: 1.2.0</para>
    /// <para>开启一个事务块</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    bool Multi(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the start of a transaction block. Subsequent commands will be queued for atomic execution using EXEC.
    /// <para>Available since: 1.2.0</para>
    /// <para>开启一个事务块</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    Task<bool> MultiAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes all previously queued commands in a transaction and restores the connection state to normal.
    /// <para>Available since: 1.2.0</para>
    /// <para>执行事务中的所有命令</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    TranResult Exec(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes all previously queued commands in a transaction and restores the connection state to normal.
    /// <para>Available since: 1.2.0</para>
    /// <para>执行事务中的所有命令</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    Task<TranResult> ExecAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the given keys to be watched for conditional execution of a transaction.
    /// <para>Available since: 2.2.0</para>
    /// <para>监视指定的Key, 可以监视多个</para>
    /// <para>支持此命令的Redis版本: 2.2.0+</para>
    /// </summary>
    /// <param name="key">Key to watch
    /// <para>要监视的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    bool Watch(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the given keys to be watched for conditional execution of a transaction.
    /// <para>Available since: 2.2.0</para>
    /// <para>监视指定的Key, 可以监视多个</para>
    /// <para>支持此命令的Redis版本: 2.2.0+</para>
    /// </summary>
    /// <param name="keys">Keys to watch
    /// <para>要监视的Key集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    bool Watch(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the given keys to be watched for conditional execution of a transaction.
    /// <para>Available since: 2.2.0</para>
    /// <para>监视指定的Key, 可以监视多个</para>
    /// <para>支持此命令的Redis版本: 2.2.0+</para>
    /// </summary>
    /// <param name="key">Key to watch
    /// <para>要监视的Key</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    Task<bool> WatchAsync(SharpValue key, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the given keys to be watched for conditional execution of a transaction.
    /// <para>Available since: 2.2.0</para>
    /// <para>监视指定的Key, 可以监视多个</para>
    /// <para>支持此命令的Redis版本: 2.2.0+</para>
    /// </summary>
    /// <param name="keys">Keys to watch
    /// <para>要监视的Key集合</para>
    /// </param>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    Task<bool> WatchAsync(SharpValueSpan keys, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all the previously watched keys for a transaction.
    /// <para>If you call [EXEC] or [DISCARD], there's no need to manually call [UNWATCH].</para>
    /// <para>Available since: 2.2.0</para>
    /// <para>取消当前事务中之前所有监视的Key. 如你已经执行了[EXEC]或[DISCARD]命令, 则无需手动调用此方法取消监视</para>
    /// <para>支持此命令的Redis版本: 2.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    bool Unwatch(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all the previously watched keys for a transaction.
    /// <para>If you call [EXEC] or [DISCARD], there's no need to manually call [UNWATCH].</para>
    /// <para>Available since: 2.2.0</para>
    /// <para>取消当前事务中之前所有监视的Key. 如你已经执行了[EXEC]或[DISCARD]命令, 则无需手动调用此方法取消监视</para>
    /// <para>支持此命令的Redis版本: 2.2.0+</para>
    /// </summary>
    /// <param name="flags">The flags to use for this operation. In general, no settings are required
    /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    Task<bool> UnwatchAsync(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default);
}
