#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisString
    {
        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.
        /// <para>Available since:1.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Set(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.
        /// <para>Available since:1.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">byte[] value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type
        /// <para>Available since: 1.0.0 | 6.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型)</para>
        /// <para>支持此命令的Redis版本, 1.0.0 | 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="keepTtl">Retain the time to live associated with the key, If this parameter is set to true, Redis6.0.0+ is required
        /// <para>是否保留Key的过期时间, 如果为true需要Redis6.0.0+才支持</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Set(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type
        /// <para>Available since: 1.0.0 | 6.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型)</para>
        /// <para>支持此命令的Redis版本, 1.0.0 | 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="keepTtl">Retain the time to live associated with the key, If this parameter is set to true, Redis6.0.0+ is required
        /// <para>是否保留Key的过期时间, 如果为true需要Redis6.0.0+才支持</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetAsync(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 设置过期时间, 秒为单位</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds
        /// <para>过期时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Set(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value.SpanToBytes(base._call.Encoding), seconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 设置过期时间, 秒为单位</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds
        /// <para>过期时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetAsync(key, value.SpanToBytes(base._call.Encoding), seconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 设置过期时间, 毫秒为单位</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds
        /// <para>过期时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Set(string key, ulong milliseconds, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.Set(key, milliseconds, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 设置过期时间, 毫秒为单位</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds
        /// <para>过期时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, ulong milliseconds, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetAsync(key, milliseconds, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, seconds.
        /// <para>Available since: 2.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 设置过期时间, 秒为单位</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds
        /// <para>过期时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetEx(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetEx(key, value.SpanToBytes(base._call.Encoding), seconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, seconds.
        /// <para>Available since: 2.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 设置过期时间, 秒为单位</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds
        /// <para>过期时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetExAsync(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetExAsync(key, value.SpanToBytes(base._call.Encoding), seconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, milliseconds.
        /// <para>Available since: 2.6.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool PSetEx(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.PSetEx(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Set the specified expire time, milliseconds.
        /// <para>Available since: 2.6.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> PSetExAsync(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.PSetExAsync(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. You can also set the expiration time
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Set(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. You can also set the expiration time
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetAsync(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Set(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetAsync(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Retain the time to live associated with the key.
        /// <para>Available since: 6.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 将保留以前该Key的过期时间</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetKeepTtl(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Retain the time to live associated with the key.
        /// <para>Available since: 6.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 将保留以前该Key的过期时间</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetKeepTtlAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetAsync(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist.
        /// <para>This method uses the SET command to set the NX parameters. For native SETNX commands, use the SetNx_Old method</para>
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值</para>
        /// <para>此方法使用SET命令, 设置NX参数实现, 如需要原生SETNX命令, 请使用SetNx_Old方法</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetNx(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetNx(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist.
        /// <para>This method uses the SET command to set the NX parameters. For native SETNX commands, use the SetNx_Old method</para>
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值</para>
        /// <para>此方法使用SET命令, 设置NX参数实现, 如需要原生SETNX命令, 请使用SetNx_Old方法</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetNxAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetNxAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist.
        /// <para>Available since: 1.0.0</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetNx_Old(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetNx_Old(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist.
        /// <para>Available since: 1.0.0</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetNx_OldAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetNx_OldAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetNx(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetNxPx(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetNxAsync(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetNxPxAsync(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. Set the specified expire time, milliseconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds ">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetNxPx(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetNxPx(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. Set the specified expire time, milliseconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds ">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetNxPxAsync(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetNxPxAsync(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. You can also set the expiration time
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetNx(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetNxPx(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. You can also set the expiration time
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetNxAsync(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetNxPxAsync(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetNx(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetNx(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it does not already exist. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key不存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetNxAsync(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetNxAsync(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值</para>
        /// <para>并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetXx(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetXx(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值</para>
        /// <para>并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetXxAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetXxAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值</para>
        /// <para>并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? SetXxGet(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetXxGet(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值</para>
        /// <para>并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> SetXxGetAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetXxGetAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Available since: 2.6.12 | 6.0.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值</para>
        /// <para>支持此命令的Redis版本, 2.6.12 | 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="keepTtl">Retain the time to live associated with the key, If this parameter is set to true, Redis6.0.0+ is required
        /// <para>是否保留Key的过期时间, 如果为true需要Redis6.0.0+才支持</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetXx(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetXx(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Available since: 2.6.12 | 6.0.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值</para>
        /// <para>支持此命令的Redis版本, 2.6.12 | 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="keepTtl">Retain the time to live associated with the key, If this parameter is set to true, Redis6.0.0+ is required
        /// <para>是否保留Key的过期时间, 如果为true需要Redis6.0.0+才支持</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetXxAsync(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetXxAsync(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值</para>
        /// <para>并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="keepTtl">Retain the time to live associated with the key.
        /// <para>是否保留Key的过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? SetXxGet(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetXxGet(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值</para>
        /// <para>并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="keepTtl">Retain the time to live associated with the key.
        /// <para>是否保留Key的过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> SetXxGetAsync(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetXxGetAsync(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? SetXxGet(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPxGet(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> SetXxGetAsync(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPxGetAsync(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetXx(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPx(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetXxAsync(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPxAsync(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, milliseconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetXxPx(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPx(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, milliseconds.
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetXxPxAsync(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPxAsync(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. You can also set the expiration time
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetXx(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxPx(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. You can also set the expiration time
        /// <para>Available since: 2.6.12</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 2.6.12+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetXxAsync(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxPxAsync(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? SetXxGet(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxPxGet(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> SetXxGetAsync(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxPxGetAsync(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetXx(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXx(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetXxAsync(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxAsync(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? SetXxGet(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxGet(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, seconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> SetXxGetAsync(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxGetAsync(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Retain the time to live associated with the key.
        /// <para>Available since: 6.0.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 保留Key原有的有效时间</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool SetXxKeepTtl(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetXx(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Retain the time to live associated with the key.
        /// <para>Available since: 6.0.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值. 保留Key原有的有效时间</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<bool> SetXxKeepTtlAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetXxAsync(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? SetGet(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGet(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public byte[]? SetGetBytes(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytes(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> SetGetAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGetAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<byte[]?> SetGetBytesAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytesAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="keepTtl">Retain the time to live associated with the key.
        /// <para>是否保留Key的过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? SetGet(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetGet(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="keepTtl">Retain the time to live associated with the key.
        /// <para>是否保留Key的过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public byte[]? SetGetBytes(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytes(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="keepTtl">Retain the time to live associated with the key.
        /// <para>是否保留Key的过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> SetGetAsync(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetGetAsync(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value<para>设置的新值</para></param>
        /// <param name="keepTtl">Retain the time to live associated with the key.
        /// <para>是否保留Key的过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<byte[]?> SetGetBytesAsync(string key, ReadOnlySpan<char> value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytesAsync(key, value.SpanToBytes(base._call.Encoding), keepTtl, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public string? SetGet(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGetPx(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public byte[]? SetGetBytes(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxBytes(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<string?> SetGetAsync(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxAsync(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<byte[]?> SetGetBytesAsync(string key, ReadOnlySpan<char> value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxBytesAsync(key, value.SpanToBytes(base._call.Encoding), (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public string? SetGet(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPx(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public byte[]? SetGetBytes(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxBytes(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<string?> SetGetAsync(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxAsync(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">
        /// Set the specified expire time, seconds.
        /// <para>有效时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<byte[]?> SetGetBytesAsync(string key, ReadOnlySpan<char> value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxBytesAsync(key, value.SpanToBytes(base._call.Encoding), seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public string? SetGet(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGet(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public byte[]? SetGetBytes(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytes(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<string?> SetGetAsync(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGetAsync(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. You can also set the expiration time
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        /// Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<byte[]?> SetGetBytesAsync(string key, ReadOnlySpan<char> value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytesAsync(key, value.SpanToBytes(base._call.Encoding), expireTime, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Set the specified expire time, milliseconds.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public string? SetGetPx(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPx(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Set the specified expire time, milliseconds.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public byte[]? SetGetPxBytes(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxBytes(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Set the specified expire time, milliseconds.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<string?> SetGetPxAsync(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxAsync(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Set the specified expire time, milliseconds.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 可同时设置过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="milliseconds">
        /// Set the specified expire time, milliseconds.
        /// <para>有效时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<byte[]?> SetGetPxBytesAsync(string key, ReadOnlySpan<char> value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return this.SetGetPxBytesAsync(key, value.SpanToBytes(base._call.Encoding), milliseconds, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Retain the time to live associated with the key.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 保留原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public string? SetGetKeepTtl(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGet(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Retain the time to live associated with the key.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 保留原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public byte[]? SetGetKeepTtlBytes(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytes(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Retain the time to live associated with the key.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 保留原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<string?> SetGetKeepTtlAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGetAsync(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string. Retain the time to live associated with the key.
        /// <para>Available since: 6.2.0</para>
        /// <para>设置并返回该Key的旧值. 如果Key对应的数据不是String类型, 则会终止操作. 保留原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public Task<byte[]?> SetGetKeepTtlBytesAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.SetGetBytesAsync(key, value.SpanToBytes(base._call.Encoding), true, cancellationToken);
        }

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key. Returns an error when key exists but does not hold a string value.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>获取Key的旧值, 并设置一个新的值. 此操作会删除Key原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value
        /// <para>要设置的新值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public string? GetSet(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.GetSet(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key. Returns an error when key exists but does not hold a string value.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>获取Key的旧值, 并设置一个新的值. 此操作会删除Key原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value
        /// <para>要设置的新值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<string?> GetSetAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.GetSetAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key. Returns an error when key exists but does not hold a string value.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>获取Key的旧值, 并设置一个新的值. 此操作会删除Key原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value
        /// <para>要设置的新值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public byte[]? GetSetBytes(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.GetSetBytes(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key. Returns an error when key exists but does not hold a string value.
        /// <para>Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>获取Key的旧值, 并设置一个新的值. 此操作会删除Key原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value
        /// <para>要设置的新值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
        public Task<byte[]?> GetSetBytesAsync(string key, ReadOnlySpan<char> value, CancellationToken cancellationToken = default)
        {
            return this.GetSetBytesAsync(key, value.SpanToBytes(base._call.Encoding), cancellationToken);
        }
    }
}
#endif
