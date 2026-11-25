using SharpRedis.Commands;
using SharpRedis.Entities.Command;
using SharpRedis.Network;
using System;
using System.Threading;

namespace SharpRedis;

public sealed partial class RedisHash
{
    /// <summary>
    /// The client for the HIMPORT operation. The "<b>HIMPORT SET</b>" command corresponds to the "Set" method.
    /// <para>It is strongly recommended that when using this type, the Dispose method should be called promptly after the operation is completed to release the resources. The using statement can be used to automatically release the resources.</para>
    /// <para>HIMPORT操作客户端, <b>HIMPORT SET</b>命令对应Set方法</para>
    /// <para>强烈建议, 使用该类时, 在完成操作后及时调用Dispose方法释放资源. 可以借助using语句自动释放</para>
    /// </summary>
    /// <example>
    /// <code>
    /// using (var himport1 = redis.Hash.HImport())
    /// {
    ///     _ = himport1.Prepare("fieldSetName", ["name", "age", "email"]);
    ///     himport1.Set("hash1", "fieldSetName", ["John", "30", "john@example.com"]);
    /// }
    /// 
    /// //async
    /// using (var himport2 = await redis.Hash.HImportAsync())
    /// {
    ///     _ = himport2.PrepareAsync("fieldSetName", ["name", "age", "email"]);
    ///     await himport2.SetAsync("hash2-1", "fieldSetName", ["John", "30", "john@example.com"]);
    ///     await himport2.SetAsync("hash2-2", "fieldSetName", ["John", "30", "john@example.com"]);
    /// }
    /// 
    /// //async disposable
    /// await using (var himport3 = await redis.Hash.HImportAsync())
    /// {
    ///     _ = himport3.PrepareAsync("fieldSetName", ["name", "age", "email"]);
    ///     await himport3.SetAsync("hash3", "fieldSetName", ["John", "30", "john@example.com"]);
    /// }
    /// </code>
    /// </example>
    public sealed partial class HImportClient : IDisposable
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            , IAsyncDisposable
#endif
    {
        private bool _disposedValue;
        private RedisHash _redisHash;
        private BaseConnection _connection;

        internal HImportClient(RedisHash redisHash)
        {
            this._redisHash = redisHash;
            this._connection = this._redisHash.GetCurrentMasterConnection();
        }

        #region HIMPORT SET
        /// <summary>
        /// Creates a hash from a fieldset previously defined with HIMPORT PREPARE, pairing the fieldset's field names with the supplied values by position.
        /// <para>Available since: 8.10.0</para>
        /// <para>根据先前使用 HIMPORT PREPARE 定义的field字段集合，按顺序给field赋值. 值的顺序与定义field字段集合时的顺序一致, 自动匹配. 会创建一个新的Hash结构的数据</para>
        /// <para>支持此命令的Redis版本, 8.10.0+</para>
        /// </summary>
        /// <param name="key">The key to create. If the key already exists, it is overwritten.
        /// <para>要创建的Hash key。如果Key已存在，则会被覆盖。</para>
        /// </param>
        /// <param name="fieldSetName">The name of a fieldset previously defined with HIMPORT PREPARE on the same connection.
        /// <para>之前使用HIMPORT PREPARE 定义的field字段集合名称</para>
        /// </param>
        /// <param name="value">The field values, given in the same order as the field names were declared in the fieldset. The number of values must equal the number of fields in the fieldset.
        /// <para>field的值, 按定义field字段集合时的field顺序自动匹配. 值的数量必须等于fieldset中的field数量</para>
        /// </param>
        /// <param name="flags">The flags to use for this operation. In general, no settings are required
        /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// <see href="https://redis.io/commands/HImport-Set">Command documentation</see>
        /// <para><see href="https://redis.io/commands/HImport-Set">命令文档</see></para>
        /// </remarks>
        public bool Set(SharpValue key, SharpValue fieldSetName, SharpValue value, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HImportSet(key, fieldSetName, value) | flags;
            return this.Invoke(command, cancellationToken);
        }

        /// <summary>
        /// Creates a hash from a fieldset previously defined with HIMPORT PREPARE, pairing the fieldset's field names with the supplied values by position.
        /// <para>Available since: 8.10.0</para>
        /// <para>根据先前使用 HIMPORT PREPARE 定义的field字段集合，按顺序给field赋值. 值的顺序与定义field字段集合时的顺序一致, 自动匹配. 会创建一个新的Hash结构的数据</para>
        /// <para>支持此命令的Redis版本, 8.10.0+</para>
        /// </summary>
        /// <param name="key">The key to create. If the key already exists, it is overwritten.
        /// <para>要创建的Hash key。如果Key已存在，则会被覆盖。</para>
        /// </param>
        /// <param name="fieldSetName">The name of a fieldset previously defined with HIMPORT PREPARE on the same connection.
        /// <para>之前使用HIMPORT PREPARE 定义的field字段集合名称</para>
        /// </param>
        /// <param name="values">The field values, given in the same order as the field names were declared in the fieldset. The number of values must equal the number of fields in the fieldset.
        /// <para>field的值, 按定义field字段集合时的field顺序自动匹配. 值的数量必须等于fieldset中的field数量</para>
        /// </param>
        /// <param name="flags">The flags to use for this operation. In general, no settings are required
        /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// <see href="https://redis.io/commands/HImport-Set">Command documentation</see>
        /// <para><see href="https://redis.io/commands/HImport-Set">命令文档</see></para>
        /// </remarks>
        public bool Set(SharpValue key, SharpValue fieldSetName, SharpValueSpan values, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HImportSet(key, fieldSetName, values) | flags;
            return this.Invoke(command, cancellationToken);
        }
        #endregion

        #region HIMPORT PREPARE
        /// <summary>
        /// Defines a session-local fieldset that maps a name to an ordered list of field names, to be used by later HIMPORT SET commands on the same connection.
        /// <para>Available since: 8.10.0</para>
        /// <para>定义一组field集合, 后续可使用 HIMPORT SET 顺序赋值</para>
        /// <para>支持此命令的Redis版本, 8.10.0+</para>
        /// </summary>
        /// <param name="fieldSetName">The name to give the fieldset. The fieldset is scoped to the current connection. Preparing a fieldset with a name that already exists on the connection replaces the previous definition.
        /// <para>field集合名称, 如果当前连接上已存在该名称, 则会替换之前的定义</para>
        /// </param>
        /// <param name="field">One or more field names to store under the fieldset, in the order you intend to supply their values to HIMPORT SET. A field name must not appear more than once in the same fieldset.
        /// <para>一个或多个field名称, 顺序和HIMPORT SET命令中的值保存一致. 同一个field不能在同一个fieldset中出现多次</para>
        /// </param>
        /// <param name="flags">The flags to use for this operation. In general, no settings are required
        /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// <see href="https://redis.io/commands/HImport-Prepare">Command documentation</see>
        /// <para><see href="https://redis.io/commands/HImport-Prepare">命令文档</see></para>
        /// </remarks>
        public bool Prepare(SharpValue fieldSetName, SharpValue field, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HImportPrepare(fieldSetName, field) | flags;
            return this.Invoke(command, cancellationToken);
        }

        /// <summary>
        /// Defines a session-local fieldset that maps a name to an ordered list of field names, to be used by later HIMPORT SET commands on the same connection.
        /// <para>Available since: 8.10.0</para>
        /// <para>定义一组field集合, 后续可使用 HIMPORT SET 顺序赋值</para>
        /// <para>支持此命令的Redis版本, 8.10.0+</para>
        /// </summary>
        /// <param name="fieldSetName">The name to give the fieldset. The fieldset is scoped to the current connection. Preparing a fieldset with a name that already exists on the connection replaces the previous definition.
        /// <para>field集合名称, 如果当前连接上已存在该名称, 则会替换之前的定义</para>
        /// </param>
        /// <param name="fields">One or more field names to store under the fieldset, in the order you intend to supply their values to HIMPORT SET. A field name must not appear more than once in the same fieldset.
        /// <para>一个或多个field名称, 顺序和HIMPORT SET命令中的值保存一致. 同一个field不能在同一个fieldset中出现多次</para>
        /// </param>
        /// <param name="flags">The flags to use for this operation. In general, no settings are required
        /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// <see href="https://redis.io/commands/HImport-Prepare">Command documentation</see>
        /// <para><see href="https://redis.io/commands/HImport-Prepare">命令文档</see></para>
        /// </remarks>
        public bool Prepare(SharpValue fieldSetName, SharpValueSpan fields, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HImportPrepare(fieldSetName, fields) | flags;
            return this.Invoke(command, cancellationToken);
        }
        #endregion

        /// <summary>
        /// Removes a single session-local fieldset, previously defined with HIMPORT PREPARE, from the current connection.
        /// <para>Available since: 8.10.0</para>
        /// <para>移除一个自定义field集合</para>
        /// <para>支持此命令的Redis版本, 8.10.0+</para>
        /// </summary>
        /// <param name="fieldSetName">The name of the fieldset to remove from the current connection.
        /// <para>要移除的自定义field集合名称</para>
        /// </param>
        /// <param name="flags">The flags to use for this operation. In general, no settings are required
        /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>True if the fieldset was removed, or False if no fieldset with that name existed on the connection.
        /// <para>True: 成功移除, 如果不存在自定义field集合, 则返回False</para>
        /// </returns>
        /// <remarks>
        /// <see href="https://redis.io/commands/HImport-Discard">Command documentation</see>
        /// <para><see href="https://redis.io/commands/HImport-Discard">命令文档</see></para>
        /// </remarks>
        public bool Discard(SharpValue fieldSetName, CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HImportDiscard(fieldSetName) | flags;
            return this.Invoke(command, cancellationToken);
        }

        /// <summary>
        /// Removes every fieldset held by the current connection, discarding all definitions previously created with HIMPORT PREPARE.
        /// <para>Available since: 8.10.0</para>
        /// <para>移除当前域中, 所有定义的field集合</para>
        /// <para>支持此命令的Redis版本, 8.10.0+</para>
        /// </summary>
        /// <param name="flags">The flags to use for this operation. In general, no settings are required
        /// <para>此次操作使用的行为标记, 一般情况下无需设置</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of fieldsets removed.
        /// <para>成功移除的自定义field集合数量</para>
        /// </returns>
        /// <remarks>
        /// <see href="https://redis.io/commands/HImport-DiscardAll">Command documentation</see>
        /// <para><see href="https://redis.io/commands/HImport-DiscardAll">命令文档</see></para>
        /// </remarks>
        public int DiscardAll(CommandFlags flags = CommandFlags.None, CancellationToken cancellationToken = default)
        {
            var command = HashCommands.HImportDiscardAll() | flags;
            return this.Invoke(command, cancellationToken);
        }

        private RedisResult Invoke(CommandPacket command, CancellationToken cancellationToken)
        {
            return this._redisHash.Invoke(command, this._connection, cancellationToken);
        }

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                try
                {
                    if (disposing) _ = this.DiscardAll();
                }
                finally
                {
                    (this._connection as TempConnection)?.Dispose();
                    this._redisHash = null!;
                    this._connection = null!;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    /// <summary>
    /// Create the <b>HIMPORT</b> command client.
    /// <para>Available since: 8.10.0</para>
    /// <para>创建<b>HImport</b>命令客户端</para>
    /// <para>支持此命令的Redis版本, 8.10.0+</para>
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// <see href="https://redis.io/commands/HImport">Command documentation</see>
    /// <para><see href="https://redis.io/commands/HImport">命令文档</see></para>
    /// </remarks>
    public HImportClient HImport()
    {
        return new HImportClient(this);
    }
}
