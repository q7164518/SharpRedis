using SharpRedis.Provider.Standard;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis;

public static class TransactionExtensions
{
    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = redis.Multi())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static IRedisTranClient Multi(this IRedisTran redis, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransaction(true, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = await redis.MultiAsync())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static Task<IRedisTranClient> MultiAsync(this IRedisTran redis, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransactionAsync(true, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = redis.BeginTransaction())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static IRedisTranClient BeginTransaction(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransaction(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = await redis.BeginTransactionAsync())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static Task<IRedisTranClient> BeginTransactionAsync(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransactionAsync(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = redis.StartTransaction())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static IRedisTranClient StartTransaction(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransaction(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = await redis.StartTransactionAsync())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static Task<IRedisTranClient> StartTransactionAsync(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransactionAsync(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = redis.NewTransaction())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static IRedisTranClient NewTransaction(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransaction(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = await redis.NewTransactionAsync())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static Task<IRedisTranClient> NewTransactionAsync(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransactionAsync(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = redis.UseTransaction())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static IRedisTranClient UseTransaction(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransaction(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = await redis.UseTransactionAsync())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static Task<IRedisTranClient> UseTransactionAsync(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        return redis.CreateTransactionAsync(multi, cancellationToken);
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = redis.CreateTransaction())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static IRedisTranClient CreateTransaction(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        var tranRedis = redis.CreateTranRedis();
        if (multi)
        {
            _ = tranRedis.Multi(cancellationToken: cancellationToken);
        }
        return tranRedis;
    }

    /// <summary>
    /// Create a Redis transaction block. Subsequently, execute the transaction using the [EXEC] command and obtain the return value.
    /// <para>Available since: 1.2.0</para>
    /// <para>创建一个Redis事务块, 后续使用[EXEC]命令执行事务并获得返回值</para>
    /// <para>支持此命令的Redis版本, 1.2.0+</para>
    /// </summary>
    /// <param name="redis">redis</param>
    /// <param name="multi">Whether the [MULTI] command is automatically executed, default is true
    /// <para>是否自动执行[MULTI]命令, 默认是自动执行的</para>
    /// </param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
    /// <para>取消Token, 有效值会覆盖系统默认的</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var tran = await redis.CreateTransactionAsync())
    /// {
    ///     _ = tran.String.Set("key", "value");
    ///     _ = await tran.String.SetAsync("key", "value");
    ///     var result = tran.Exec(); //[OR] await tran.ExecAsync();
    /// }
    /// </code>
    /// </example>
    public static async Task<IRedisTranClient> CreateTransactionAsync(this IRedisTran redis, bool multi = true, CancellationToken cancellationToken = default)
    {
        var tranRedis = redis.CreateTranRedis();
        if (multi)
        {
            _ = await tranRedis.MultiAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        return tranRedis;
    }
}
