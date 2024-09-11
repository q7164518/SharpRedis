#if !NET30
using System.Linq;
#endif
#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using SharpRedis.Commands;
using SharpRedis.Extensions;
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;

namespace SharpRedis
{
    /// <summary>
    /// Redis string type
    /// </summary>
    public sealed partial class RedisString : BaseType
    {
        internal RedisString(BaseCall call)
            : base(call)
        {
        }

        #region Set methods
        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.
        /// <para>Available since:1.0.0</para>
        /// <para>根据Key设置一个Value. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool Set(string key, string value, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.Set(key, value), "OK", cancellationToken);
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
        public bool Set(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.Set(key, value), "OK", cancellationToken);
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
        public bool Set(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.Set(key, value, keepTtl: keepTtl), "OK", cancellationToken);
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
        public bool Set(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.Set(key, value, keepTtl: keepTtl), "OK", cancellationToken);
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
        public bool Set(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, milliseconds: seconds * 1000), "OK", cancellationToken);
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
        public bool Set(string key, ulong milliseconds, string value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, milliseconds: milliseconds), "OK", cancellationToken);
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
        public bool Set(string key, ulong milliseconds, byte[] value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, milliseconds: milliseconds), "OK", cancellationToken);
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
        public bool Set(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, milliseconds: seconds * 1000), "OK", cancellationToken);
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
        public bool SetEx(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.SetEx(key, seconds, value), "OK", cancellationToken);
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
        public bool SetEx(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.SetEx(key, seconds, value), "OK", cancellationToken);
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
        public bool PSetEx(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.PSetEx(key, milliseconds, value), "OK", cancellationToken);
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
        public bool PSetEx(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.PSetEx(key, milliseconds, value), "OK", cancellationToken);
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
        public bool Set(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.Set(key, (ulong)expireTime.TotalMilliseconds, value, cancellationToken);
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
        public bool Set(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.Set(key, (ulong)expireTime.TotalMilliseconds, value, cancellationToken);
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
        public bool Set(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, expireTime: expireTime), "OK", cancellationToken);
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
        public bool Set(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, expireTime: expireTime), "OK", cancellationToken);
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
        public bool SetKeepTtl(string key, string value, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value, true, cancellationToken);
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
        public bool SetKeepTtl(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            return this.Set(key, value, true, cancellationToken);
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
        public bool SetNx(string key, string value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Nx), "OK", cancellationToken);
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
        public bool SetNx(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Nx), "OK", cancellationToken);
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
        public bool SetNx_Old(string key, string value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.SetNx(key, value), "OK", cancellationToken);
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
        public bool SetNx_Old(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.SetNx(key, value), "OK", cancellationToken);
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
        public bool SetNx(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetNxPx(key, value, seconds * 1000, cancellationToken);
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
        public bool SetNx(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetNxPx(key, value, seconds * 1000, cancellationToken);
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
        public bool SetNxPx(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Nx, milliseconds: milliseconds), "OK", cancellationToken);
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
        public bool SetNxPx(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Nx, milliseconds: milliseconds), "OK", cancellationToken);
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
        public bool SetNx(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetNxPx(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
        public bool SetNx(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetNxPx(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
        public bool SetNx(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Nx, expireTime: expireTime), "OK", cancellationToken);
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
        public bool SetNx(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Nx, expireTime: expireTime), "OK", cancellationToken);
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
        public bool SetXx(string key, string value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx), "OK", cancellationToken);
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
        public bool SetXx(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx), "OK", cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, string value, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, string value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, byte[] value, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, byte[] value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, get: true), cancellationToken);
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
        public bool SetXx(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx, keepTtl: keepTtl), "OK", cancellationToken);
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
        public bool SetXx(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx, keepTtl: keepTtl), "OK", cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, keepTtl: keepTtl, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, keepTtl: keepTtl, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetXxPxGet(key, value, seconds * 1000, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetXxPxGet(key, value, seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, milliseconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
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
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxPxGet(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public string SetXxPxGet(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, milliseconds: milliseconds, get: true), cancellationToken);
        }

        /// <summary>
        /// Set key to hold the string value. Only set the key if it already exists. Set the specified expire time, milliseconds.
        /// <para>Return the old string stored at key, or nil if key did not exist. An error is returned and SET aborted if the value stored at key is not a string.</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>根据Key设置一个Value. 仅在Key存在的时候才设置值, 并返回Key的旧值. 且可设置过期时间</para>
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
        /// <returns>The old string stored at key
        /// <para>Key的旧值</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxPxGet(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public string SetXxPxGet(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, milliseconds: milliseconds, get: true), cancellationToken);
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
        public bool SetXx(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPx(key, value, seconds * 1000, cancellationToken);
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
        public bool SetXx(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
        {
            return this.SetXxPx(key, value, seconds * 1000, cancellationToken);
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
        public bool SetXxPx(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx, milliseconds: milliseconds), "OK", cancellationToken);
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
        public bool SetXxPx(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx, milliseconds: milliseconds), "OK", cancellationToken);
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
        public bool SetXx(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxPx(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
        public bool SetXx(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
        {
            return this.SetXxPx(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetXxPxGet(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetXxPxGet(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
        public bool SetXx(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx, expireTime: expireTime), "OK", cancellationToken);
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
        public bool SetXx(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(StringCommands.Set(key, value, nxx: NxXx.Xx, expireTime: expireTime), "OK", cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, expireTime: expireTime, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetXxGet(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public string SetXxGet(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, nxx: NxXx.Xx, expireTime: expireTime, get: true), cancellationToken);
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
        public bool SetXxKeepTtl(string key, string value, CancellationToken cancellationToken = default)
        {
            return this.SetXx(key, value, true, cancellationToken);
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
        public bool SetXxKeepTtl(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            return this.SetXx(key, value, true, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, string value, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, string value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, string value, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, string value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, byte[] value, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, byte[] value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, byte[] value, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, byte[] value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true, keepTtl: keepTtl), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, string value, bool keepTtl, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true, keepTtl: keepTtl), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true, keepTtl: keepTtl), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, byte[] value, bool keepTtl, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true, keepTtl: keepTtl), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPx(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, string value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPxBytes(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPx(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, byte[] value, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPxBytes(key, value, (ulong)expireTime.TotalMilliseconds, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPx(key, value, seconds * 1000, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, string value, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPxBytes(key, value, seconds * 1000, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPx(key, value, seconds * 1000, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, byte[] value, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetPxBytes(key, value, seconds * 1000, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true, expireTime: expireTime), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, string value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true, expireTime: expireTime), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGet(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public string SetGet(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true, expireTime: expireTime), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetBytes(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetBytes(string key, byte[] value, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true, expireTime: expireTime), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGetPx(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public string SetGetPx(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true, milliseconds: milliseconds), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetPxBytes(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetPxBytes(string key, string value, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true, milliseconds: milliseconds), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGetPx(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public string SetGetPx(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.Set(key, value, get: true, milliseconds: milliseconds), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetPxBytes(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetPxBytes(string key, byte[] value, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Set(key, value, get: true, milliseconds: milliseconds), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGetKeepTtl(string key, string value, CancellationToken cancellationToken = default)
#else
        public string SetGetKeepTtl(string key, string value, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGet(key, value, true, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetKeepTtlBytes(string key, string value, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetKeepTtlBytes(string key, string value, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetBytes(key, value, true, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SetGetKeepTtl(string key, byte[] value, CancellationToken cancellationToken = default)
#else
        public string SetGetKeepTtl(string key, byte[] value, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGet(key, value, true, cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? SetGetKeepTtlBytes(string key, byte[] value, CancellationToken cancellationToken = default)
#else
        public byte[] SetGetKeepTtlBytes(string key, byte[] value, CancellationToken cancellationToken = default)
#endif
        {
            return this.SetGetBytes(key, value, true, cancellationToken);
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSET replaces existing values with new values, just as regular SET.
        /// <para>It is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置多个Key Value, 和普通的Set一样, 只不过该方法是批量设置的. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValueArray">Array of key-value pairs
        /// <para>键值对数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSet(KeyValuePair<string, string>[] keyValueArray, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.MSet(keyValueArray), "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSET replaces existing values with new values, just as regular SET.
        /// <para>It is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置多个Key Value, 和普通的Set一样, 只不过该方法是批量设置的. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValueArray">Array of key-value pairs
        /// <para>键值对数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSet(KeyValuePair<string, byte[]>[] keyValueArray, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.MSet(keyValueArray), "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSET replaces existing values with new values, just as regular SET.
        /// <para>It is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置多个Key Value, 和普通的Set一样, 只不过该方法是批量设置的. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValues">Dictionary string
        /// <para>键值对字典集合</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSet(Dictionary<string, string> keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues is null || keyValues.Count <= 0) throw new InvalidOperationException("Make sure there is at least one element");
#if !NET30
            return this.MSet(keyValues.ToArray(), cancellationToken);
#else
            var kvs = new List<KeyValuePair<string, string>>();
            foreach (var item in keyValues)
            {
                kvs.Add(new KeyValuePair<string, string>(item.Key, item.Value));
            }
            return this.MSet(kvs.ToArray(), cancellationToken);
#endif
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSET replaces existing values with new values, just as regular SET.
        /// <para>It is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置多个Key Value, 和普通的Set一样, 只不过该方法是批量设置的. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValues">Dictionary string
        /// <para>键值对字典集合</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSet(Dictionary<string, byte[]> keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues is null || keyValues.Count <= 0) throw new InvalidOperationException("Make sure there is at least one element");
#if !NET30
            return this.MSet(keyValues.ToArray(), cancellationToken);
#else
            var kvs = new List<KeyValuePair<string, byte[]>>();
            foreach (var item in keyValues)
            {
                kvs.Add(new KeyValuePair<string, byte[]>(item.Key, item.Value));
            }
            return this.MSet(kvs.ToArray(), cancellationToken);
#endif
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSET replaces existing values with new values, just as regular SET.
        /// <para>It is overwritten, regardless of its type. Any previous time to live associated with the key is discarded on successful SET operation.</para>
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置多个Key Value, 和普通的Set一样, 只不过该方法是批量设置的. 如果Key已存在, 将会覆盖以前的值(无论以前的Key是什么类型). 并覆盖以前设置的过期时间, 设置为不过期</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="kvArray">An array of key-value pairs, starting at 0, with even digits being Key and odd digits being Value
        /// <para>键值对的数组，从0开始，偶数为Key，奇数为Value</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSet(string[] kvArray, CancellationToken cancellationToken = default)
        {
            if (kvArray is null || kvArray.Length <= 0) throw new InvalidOperationException("Make sure you have at least one KV");
            if (kvArray.Length % 2 != 0) throw new InvalidOperationException("Make sure the array is the correct key-value pair");
            var command = StringCommands.MSet();
            for (uint i = 0; i < kvArray.Length; i += 2)
            {
                command.WriteKey(kvArray[i])
                    .WriteValue(kvArray[i + 1]);
            }
            return this._call.CallCondition(command, "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSETNX will not perform any operation at all even if just a single key already exists.
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置Key的值. 只有当所有的Key不存在的时候才执行. 如果有一个或一个以上的Key存在, 不会执行任何设置值操作</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValueArray">Array of key-value pairs
        /// <para>键值对数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSetNx(KeyValuePair<string, string>[] keyValueArray, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.MSetNx(keyValueArray), "1", cancellationToken);
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSETNX will not perform any operation at all even if just a single key already exists.
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置Key的值. 只有当所有的Key不存在的时候才执行. 如果有一个或一个以上的Key存在, 不会执行任何设置值操作</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValueArray">Array of key-value pairs
        /// <para>键值对数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSetNx(KeyValuePair<string, byte[]>[] keyValueArray, CancellationToken cancellationToken = default)
        {
            return this._call.CallCondition(StringCommands.MSetNx(keyValueArray), "1", cancellationToken);
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSETNX will not perform any operation at all even if just a single key already exists.
        /// <para>Available since:1.0.1</para>
        /// <para>批量设置Key的值. 只有当所有的Key不存在的时候才执行. 如果有一个或一个以上的Key存在, 不会执行任何设置值操作</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValues">Dictionary string
        /// <para>键值对字典集合</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSetNx(Dictionary<string, string> keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues is null || keyValues.Count <= 0) throw new InvalidOperationException("Make sure there is at least one element");
#if !NET30
            return this.MSetNx(keyValues.ToArray(), cancellationToken);
#else
            var kvs = new List<KeyValuePair<string, string>>();
            foreach (var item in keyValues)
            {
                kvs.Add(new KeyValuePair<string, string>(item.Key, item.Value));
            }
            return this.MSetNx(kvs.ToArray(), cancellationToken);
#endif
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSETNX will not perform any operation at all even if just a single key already exists.
        /// <para>Available since:1.0.1</para>
        /// <para>批量设置Key的值. 只有当所有的Key不存在的时候才执行. 如果有一个或一个以上的Key存在, 不会执行任何设置值操作</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="keyValues">Dictionary string
        /// <para>键值对字典集合</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSetNx(Dictionary<string, byte[]> keyValues, CancellationToken cancellationToken = default)
        {
            if (keyValues is null || keyValues.Count <= 0) throw new InvalidOperationException("Make sure there is at least one element");
#if !NET30
            return this.MSetNx(keyValues.ToArray(), cancellationToken);
#else
            var kvs = new List<KeyValuePair<string, byte[]>>();
            foreach (var item in keyValues)
            {
                kvs.Add(new KeyValuePair<string, byte[]>(item.Key, item.Value));
            }
            return this.MSetNx(kvs.ToArray(), cancellationToken);
#endif
        }

        /// <summary>
        /// Sets the given keys to their respective values. MSETNX will not perform any operation at all even if just a single key already exists.
        /// <para>Available since: 1.0.1</para>
        /// <para>批量设置Key的值. 只有当所有的Key不存在的时候才执行. 如果有一个或一个以上的Key存在, 不会执行任何设置值操作</para>
        /// <para>支持此命令的Redis版本, 1.0.1+</para>
        /// </summary>
        /// <param name="kvArray">An array of key-value pairs, starting at 0, with even digits being Key and odd digits being Value
        /// <para>键值对的数组，从0开始，偶数为Key，奇数为Value</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool MSetNx(string[] kvArray, CancellationToken cancellationToken = default)
        {
            if (kvArray is null || kvArray.Length <= 0) throw new InvalidOperationException("Make sure you have at least one KV");
            if (kvArray.Length % 2 != 0) throw new InvalidOperationException("Make sure the array is the correct key-value pair");
            var command = StringCommands.MSetNx();
            for (uint i = 0; i < kvArray.Length; i += 2)
            {
                command.WriteKey(kvArray[i])
                    .WriteValue(kvArray[i + 1]);
            }
            return this._call.CallCondition(command, "1", cancellationToken);
        }
        #endregion

        /// <summary>
        /// Get the value of key. If the key does not exist the special value null is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// <para>Available since: 1.0.0</para>
        /// <para>获取 key 的值。如果键不存在，则返回null 。如果存储在 key 的值不是字符串，则返回错误，因为 GET 只处理字符串值</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? Get(string key, CancellationToken cancellationToken = default)
#else
        public string Get(string key, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallString(StringCommands.Get(key), cancellationToken);
        }

        /// <summary>
        /// The value of a key was obtained by byte[]. If the key does not exist the special value null is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// <para>Available since: 1.0.0</para>
        /// <para>用byte[]获取 key 的值。如果键不存在，则返回null 。如果存储在 key 的值不是字符串，则返回错误，因为 GET 只处理字符串值</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetBytes(string key, CancellationToken cancellationToken = default)
#else
        public byte[] GetBytes(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.Get(key), cancellationToken);
        }

        /// <summary>
        /// If key already exists and is a string, this command appends the value at the end of the string. If key does not exist it is created and set as an empty string, so APPEND will be similar to SET in this special case.
        /// <para>Available since: 2.0.0</para>
        /// <para>如果 key 已经存在并且是一个字符串，则此命令会在字符串的末尾追加 value 。如果 key 不存在，它将被创建并设置为空字符串，因此在这种特殊情况下， APPEND 将类似于 SET 。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="appendValue">append value
        /// <para>要追加的值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Integer reply: the length of the string after the append operation.
        /// <para>追加操作后的字符串字节长度</para>
        /// </returns>
        public long Append(string key, string appendValue, CancellationToken cancellationToken = default)
        {
            return this._call.CallNumber<long>(StringCommands.Append(key, appendValue), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// If key already exists and is a string, this command appends the value at the end of the string. If key does not exist it is created and set as an empty string, so APPEND will be similar to SET in this special case.
        /// <para>Available since: 2.0.0</para>
        /// <para>如果 key 已经存在并且是一个字符串，则此命令会在字符串的末尾追加 value 。如果 key 不存在，它将被创建并设置为空字符串，因此在这种特殊情况下， APPEND 将类似于 SET 。</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="appendValue">append value
        /// <para>要追加的值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Integer reply: the length of the string after the append operation.
        /// <para>追加操作后的字符串字节长度</para>
        /// </returns>
        public long Append(string key, byte[] appendValue, CancellationToken cancellationToken = default)
        {
            return this._call.CallNumber<long>(StringCommands.Append(key, appendValue), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Decrements the number stored at key by one. If the key does not exist, it is set to 0 before performing the operation. 
        /// <para>An error is returned if the key contains a value of the wrong type or contains a string that can not be represented as integer.</para>
        /// <para>This operation is limited to 64 bit signed integers.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>将Key里面的数字递减1。如果Key不存在，则在执行操作之前将其设置为0 。如果Key包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作限于Int64。</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key after the decrement
        /// <para>递减之后的值</para>
        /// </returns>
        public NumberValue Decr(string key, CancellationToken cancellationToken = default)
        {
            return this.DecrBy(key, 1, cancellationToken);
        }

        /// <summary>
        /// Decrements the number stored at key by decrement. If the key does not exist, it is set to 0 before performing the operation.
        /// <para>An error is returned if the key contains a value of the wrong type or contains a string that can not be represented as integer.</para>
        /// <para>This operation is limited to 64 bit signed integers.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>将Key里面的数字减少decrement。如果Key不存在，则在执行操作之前将其设置为0 。如果Key包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作限于Int64。</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <typeparam name="TDecrement">The integer value type to decrement
        /// <para>要递减的整型数值类型</para>
        /// </typeparam>
        /// <param name="key">Key</param>
        /// <param name="decrement">decrement value
        /// <para>要减少的值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key after the decrement
        /// <para>递减之后的值</para>
        /// </returns>
        public NumberValue DecrBy<TDecrement>(string key, TDecrement decrement, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TDecrement : struct, System.Numerics.INumber<TDecrement>
#else
            where TDecrement : struct, IEquatable<TDecrement>
#endif
        {
#if NET7_0_OR_GREATER
            if (decrement == TDecrement.Zero)
#else
            if (decrement.Equals(default))
#endif
            {
                throw new InvalidOperationException("The decrease value is 0, which is meaningless");
            }
            Extend.CheckIntegerType(in decrement, "The [DECRBY] command supports only integer value types");

            var command = StringCommands.DecrBy(key, decrement);
            return base._call.CallNumber(command, cancellationToken);
        }

        /// <summary>
        /// Increments the number stored at key by one. If the key does not exist, it is set to 0 before performing the operation.
        /// <para>An error is returned if the key contains a value of the wrong type or contains a string that can not be represented as integer.</para>
        /// <para>This operation is limited to 64 bit signed integers.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>将Key里面的数字递增1。如果Key不存在，则在执行操作之前将其设置为 0 。如果Key包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作限于Int64。</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key after the increment
        /// <para>递增之后的值</para>
        /// </returns>
        public NumberValue Incr(string key, CancellationToken cancellationToken = default)
        {
            return this.IncrBy(key, 1, cancellationToken);
        }

        /// <summary>
        /// Increments the number stored at key by increment. If the key does not exist, it is set to 0 before performing the operation.
        /// <para>An error is returned if the key contains a value of the wrong type or contains a string that can not be represented as integer.</para>
        /// <para>This operation is limited to 64 bit signed integers.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>将Key里面的数字增加increment。如果Key不存在，则在执行操作之前将其设置为0 。如果Key包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作限于Int64。</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <typeparam name="TIncrement">The integer value type to increment
        /// <para>要递增的整型数值类型</para>
        /// </typeparam>
        /// <param name="key">Key</param>
        /// <param name="increment">Increment value
        /// <para>要增加的值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key after the increment
        /// <para>递增之后的值</para>
        /// </returns>
        public NumberValue IncrBy<TIncrement>(string key, TIncrement increment, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, IEquatable<TIncrement>
#endif
        {
#if NET7_0_OR_GREATER
            if (increment == TIncrement.Zero)
#else
            if (increment.Equals(default))
#endif
            {
                throw new InvalidOperationException("The increment value is 0, which is meaningless");
            }
            Extend.CheckIntegerType(increment, "The [INCRBY] command supports only integer value types");
            return base._call.CallNumber(StringCommands.IncrBy(key, increment), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and delete the key. This command is similar to GET, except for the fact that it also deletes the key on success (if and only if the key's value type is a string).
        /// <para>Available since: 6.2.0</para>
        /// <para>获取key的值并删除该Key. 仅在Key的类型是String才会进行删除, 否则异常. 如果Key不存在返回null</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, nil when key does not exist, or an error if the key's value type isn't a string.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetDel(string key, CancellationToken cancellationToken = default)
#else
        public string GetDel(string key, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallString(StringCommands.GetDel(key), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and delete the key. This command is similar to GET, except for the fact that it also deletes the key on success (if and only if the key's value type is a string).
        /// <para>Available since: 6.2.0</para>
        /// <para>获取key的值并删除该Key. 仅在Key的类型是String才会进行删除, 否则异常. 如果Key不存在返回null</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, nil when key does not exist, or an error if the key's value type isn't a string.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetDelBytes(string key, CancellationToken cancellationToken = default)
#else
        public byte[] GetDelBytes(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.GetDel(key), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="seconds">Set the specified expire time, seconds.
        /// <para>设置过期时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetEx(string key, ulong seconds, CancellationToken cancellationToken = default)
#else
        public string GetEx(string key, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.GetExPx(key, seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="seconds">Set the specified expire time, seconds.
        /// <para>设置过期时间, 单位: 秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetExBytes(string key, ulong seconds, CancellationToken cancellationToken = default)
#else
        public byte[] GetExBytes(string key, ulong seconds, CancellationToken cancellationToken = default)
#endif
        {
            return this.GetExPxBytes(key, seconds * 1000, cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="milliseconds">Set the specified expire time, milliseconds.
        /// <para>设置过期时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetExPx(string key, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public string GetExPx(string key, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.GetEx(key, milliseconds), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="milliseconds">Set the specified expire time, milliseconds.
        /// <para>设置过期时间, 单位: 毫秒</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetExPxBytes(string key, ulong milliseconds, CancellationToken cancellationToken = default)
#else
        public byte[] GetExPxBytes(string key, ulong milliseconds, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.GetEx(key, milliseconds), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="persist">Remove the time to live associated with the key.
        /// <para>是否删除Key的过期时间, 设置为不过期</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetEx(string key, bool persist, CancellationToken cancellationToken = default)
#else
        public string GetEx(string key, bool persist, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.GetEx(key, persist: persist), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="persist">Remove the time to live associated with the key.
        /// <para>是否删除Key的过期时间, 设置为不过期</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetExBytes(string key, bool persist, CancellationToken cancellationToken = default)
#else
        public byte[] GetExBytes(string key, bool persist, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.GetEx(key, persist: persist), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="expireTime">Expire time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetEx(string key, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public string GetEx(string key, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.GetExPx(key, (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="expireTime">Expire time
        /// <para>过期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetExBytes(string key, TimeSpan expireTime, CancellationToken cancellationToken = default)
#else
        public byte[] GetExBytes(string key, TimeSpan expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return this.GetExPxBytes(key, (ulong)expireTime.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="expireTime">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetEx(string key, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public string GetEx(string key, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.GetEx(key, expireTime: expireTime), cancellationToken);
        }

        /// <summary>
        /// Get the value of key and set its expiration. GETEX is similar to GET, but is a write command with additional options.
        /// <para>Available since: 6.2.0</para>
        /// <para>获取该Key的值, 并设置Key的过期时间. Key的类型非String会异常</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="expireTime">Expiration time
        /// <para>到期时间</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key, or nil when key does not exist.
        /// <para>Key里面的值. 如果Key不存在返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetExBytes(string key, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#else
        public byte[] GetExBytes(string key, DateTimeOffset expireTime, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.GetEx(key, expireTime: expireTime), cancellationToken);
        }

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
        /// <para>Negative offsets can be used in order to provide an offset starting from the end of the string.</para>
        /// <para>So -1 means the last character, -2 the penultimate and so forth.</para>
        /// <para>Available since: 2.4.0</para>
        /// <para>获得Key里面指定范围的部分字符串. 下标-1表示最后一个字符, -2表示倒数第二个字符串, 以此类推</para>
        /// <para>如果字符串是中文, 或包含表情符号, 请自行根据设置的编码计算好下标, 否则可能会乱码</para>
        /// <para>比如默认UTF-8编码, 一个汉字占3个字节, 如果要截取前面两个汉字, 则需要start传0, end传6</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="start">start
        /// <para>开始的下标, 返回的数据包含此下标</para>
        /// </param>
        /// <param name="end">end
        /// <para>结束的下标, 返回的数据包含此下标</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive)
        /// <para>截取后的字符串</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetRange(string key, long start, long end, CancellationToken cancellationToken = default)
#else
        public string GetRange(string key, long start, long end, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallString(StringCommands.GetRange(key, start, end), cancellationToken);
        }

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
        /// <para>Negative offsets can be used in order to provide an offset starting from the end of the string.</para>
        /// <para>So -1 means the last character, -2 the penultimate and so forth.</para>
        /// <para>Available since: 2.4.0</para>
        /// <para>获得Key里面指定范围的部分字符串. 下标-1表示最后一个字符, -2表示倒数第二个字符串, 以此类推</para>
        /// <para>如果字符串是中文, 或包含表情符号, 请自行根据设置的编码计算好下标, 否则可能会乱码</para>
        /// <para>比如默认UTF-8编码, 一个汉字占3个字节, 如果要截取前面两个汉字, 则需要start传0, end传6</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="start">start
        /// <para>开始的下标, 返回的数据包含此下标</para>
        /// </param>
        /// <param name="end">end
        /// <para>结束的下标, 返回的数据包含此下标</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive)
        /// <para>截取后的字符串</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetRangeBytes(string key, long start, long end, CancellationToken cancellationToken = default)
#else
        public byte[] GetRangeBytes(string key, long start, long end, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallBytes(StringCommands.GetRange(key, start, end), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetSet(string key, string value, CancellationToken cancellationToken = default)
#else
        public string GetSet(string key, string value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.GetSet(key, value), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? GetSet(string key, byte[] value, CancellationToken cancellationToken = default)
#else
        public string GetSet(string key, byte[] value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(StringCommands.GetSet(key, value), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetSetBytes(string key, string value, CancellationToken cancellationToken = default)
#else
        public byte[] GetSetBytes(string key, string value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.GetSet(key, value), cancellationToken);
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
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? GetSetBytes(string key, byte[] value, CancellationToken cancellationToken = default)
#else
        public byte[] GetSetBytes(string key, byte[] value, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(StringCommands.GetSet(key, value), cancellationToken);
        }

        /// <summary>
        /// Increment the string representing a floating point number stored at key by the specified increment.
        /// <para>If the key does not exist, it is set to 0 before performing the operation. An error is returned if one of the following conditions occur:</para>
        /// <para>1. The key contains a value of the wrong type (not a string).</para>
        /// <para>2. The current key content or the specified increment are not parsable as a double precision floating point number.</para>
        /// <para>Available since: 2.6.0</para>
        /// <para>将Key里面的值以小数方式递增</para>
        /// <para>1. 如果Key的类型不是String, 引发异常</para>
        /// <para>2. 如果Key的值不能转换为小数, 引发异常</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <typeparam name="TIncrement">The type of floating point value to be incremented
        /// <para>要递增的浮点数值类型</para>
        /// </typeparam>
        /// <param name="key">Key</param>
        /// <param name="increment">increment value
        /// <para>要增加的数值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key after the increment.
        /// <para>递增之后的值</para>
        /// </returns>
        public NumberValue IncrByFloat<TIncrement>(string key, TIncrement increment, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TIncrement : struct, System.Numerics.INumber<TIncrement>
#else
            where TIncrement : struct, IEquatable<TIncrement>
#endif
        {
#if NET7_0_OR_GREATER
            if (increment == TIncrement.Zero)
#else
            if (increment.Equals(default))
#endif
            {
                throw new InvalidOperationException("The increment value is 0, which is meaningless");
            }
            Extend.CheckNumberType(increment, "The [INCRBYFLOAT] command supports only Floating-point value types");

            var command = StringCommands.IncrByFloat(key, increment);
            return base._call.CallNumber(command, cancellationToken);
        }

        /// <summary>
        /// Decrement the string representing a floating point number stored at key by the specified decrement.
        /// <para>If the key does not exist, it is set to 0 before performing the operation. An error is returned if one of the following conditions occur:</para>
        /// <para>1. The key contains a value of the wrong type (not a string).</para>
        /// <para>2. The current key content or the specified increment are not parsable as a double precision floating point number.</para>
        /// <para>Available since: 2.6.0</para>
        /// <para>将Key里面的值以小数方式递减</para>
        /// <para>1. 如果Key的类型不是String, 引发异常</para>
        /// <para>2. 如果Key的值不能转换为小数, 引发异常</para>
        /// <para>支持此命令的Redis版本, 2.6.0+</para>
        /// </summary>
        /// <typeparam name="TDecrement">The type of floating point value to be decremented
        /// <para>要递减的浮点数值类型</para>
        /// </typeparam>
        /// <param name="key">Key</param>
        /// <param name="decrement">decrement value
        /// <para>要减少的数值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of key after the decrement.
        /// <para>递减之后的值</para>
        /// </returns>
        public NumberValue DecrByFloat<TDecrement>(string key, TDecrement decrement, CancellationToken cancellationToken = default)
#if NET7_0_OR_GREATER
            where TDecrement : struct, System.Numerics.INumber<TDecrement>
#else
            where TDecrement : struct, IEquatable<TDecrement>
#endif
        {
#if NET7_0_OR_GREATER
            return this.IncrByFloat<TDecrement>(key, -decrement, cancellationToken);
#else
            TDecrement number = Extend.GetOppositeValue(decrement);
            return this.IncrByFloat<TDecrement>(key, number, cancellationToken);
#endif
        }

        /// <summary>
        /// Returns the values of all specified keys. For every key that does not hold a string value or does not exist, the special value null is returned. Because of this, the operation never fails.
        /// <para>Available since: 1.0.0</para>
        /// <para>批量获得多个Key的值. 如果Key不是String类型或者Key不存在, 返回null</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">Keys
        /// <para>要获得的Key数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>List of values at the specified keys.
        /// <para>指定Key的值数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string?[] MGet(string[] keys, CancellationToken cancellationToken = default)
#else
        public string[] MGet(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallClassArray<string>(StringCommands.MGet(keys), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the values of all specified keys. For every key that does not hold a string value or does not exist, the special value null is returned. Because of this, the operation never fails.
        /// <para>Available since: 1.0.0</para>
        /// <para>批量获得多个Key的值. 如果Key不是String类型或者Key不存在, 返回null</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="keys">Keys
        /// <para>要获得的Key数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>List of values at the specified keys.
        /// <para>指定Key的值数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]?[] MGetBytes(string[] keys, CancellationToken cancellationToken = default)
#else
        public byte[][] MGetBytes(string[] keys, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallClassArray<byte[]>(StringCommands.MGet(keys), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Overwrites part of the string stored at key, starting at the specified offset, for the entire length of value.
        /// <para>If the offset is larger than the current length of the string at key, the string is padded with zero-bytes to make offset fit.</para>
        /// <para>Non-existing keys are considered as empty strings, so this command will make sure it holds a string large enough to be able to set value at offset.</para>
        /// <para>Retain the time to live associated with the key.</para>
        /// <para>Available since: 2.2.0</para>
        /// <para>将一个Key的值从指定下标开始, 替换成新值. 传入的新值有多长, 就替换到哪</para>
        /// <para>请自行计算好下标. 编码不同, 长度不一样. 比如UTF-8一个汉字占三个字节, 否则替换后可能会造成乱码</para>
        /// <para>此操作不会覆盖Key原有的过期时间</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="offset">offset
        /// <para>开始的下标, 包含此下标</para>
        /// </param>
        /// <param name="value">value
        /// <para>替换的新值</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the string after it was modified by the command.
        /// <para>替换之后的字符串字节长度. 编码不同, 返回不一样</para>
        /// </returns>
        public long SetRange(string key, long offset, string value, CancellationToken cancellationToken = default)
        {
            return this._call.CallNumber<long>(StringCommands.SetRange(key, offset, value), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the length of the string value stored at key. An error is returned when key holds a non-string value.
        /// <para>Available since: 2.2.0</para>
        /// <para>返回Key里面字符串的字节长度. 不同编码会导致不同结果. 比如UTF-8编码一个汉字是3个字节</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the string at key, or 0 when key does not exist.
        /// <para>Key里面字符串的字节长度.</para>
        /// </returns>
        public long StrLen(string key, CancellationToken cancellationToken = default)
        {
            return this._call.CallNumber<long>(StringCommands.StrLen(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive).
        /// <para>Negative offsets can be used in order to provide an offset starting from the end of the string.</para>
        /// <para>So -1 means the last character, -2 the penultimate and so forth.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>获得Key里面指定范围的部分字符串. 下标-1表示最后一个字符, -2表示倒数第二个字符串, 以此类推</para>
        /// <para>如果字符串是中文, 或包含表情符号, 请自行根据设置的编码计算好下标, 否则可能会乱码</para>
        /// <para>比如默认UTF-8编码, 一个汉字占3个字节, 如果要截取前面两个汉字, 则需要start传0, end传6</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="start">start
        /// <para>开始的下标, 返回的数据包含此下标</para>
        /// </param>
        /// <param name="end">end
        /// <para>结束的下标, 返回的数据包含此下标</para></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Returns the substring of the string value stored at key, determined by the offsets start and end (both are inclusive)
        /// <para>截取后的字符串</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? SubStr(string key, long start, long end, CancellationToken cancellationToken = default)
#else
        public string SubStr(string key, long start, long end, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallString(StringCommands.SubStr(key, start, end), cancellationToken);
        }

        /// <summary>
        /// The LCS command implements the longest common subsequence algorithm. Note that this is different than the longest common string algorithm, since matching characters in the string does not need to be contiguous.
        /// <para>Available since: 7.0.0</para>
        /// <para>LCS命令实现了最长的公共子序列算法。请注意，这与最长公共字符串算法不同，因为字符串中的匹配字符不需要是连续的。</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The string representing the longest common substring is returned.
        /// <para>最长公共子字符串的字符串</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? Lcs(string key1, string key2, CancellationToken cancellationToken = default)
#else
        public string Lcs(string key1, string key2, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallString(StringCommands.Lcs(key1, key2), cancellationToken);
        }

        /// <summary>
        /// The LCS command implements the longest common subsequence algorithm. Note that this is different than the longest common string algorithm, since matching characters in the string does not need to be contiguous.
        /// <para>Available since: 7.0.0</para>
        /// <para>LCS命令实现了最长的公共子序列算法。请注意，这与最长公共字符串算法不同，因为字符串中的匹配字符不需要是连续的。</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the longest common substring.
        /// <para>最长公共子字符串的字节长度</para>
        /// </returns>
        public long LcsLen(string key1, string key2, CancellationToken cancellationToken = default)
        {
            return this._call.CallNumber<long>(StringCommands.Lcs(key1, key2, len: true), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// The LCS command implements the longest common subsequence algorithm. Note that this is different than the longest common string algorithm, since matching characters in the string does not need to be contiguous.
        /// <para>Available since: 7.0.0</para>
        /// <para>LCS命令实现了最长的公共子序列算法。请注意，这与最长公共字符串算法不同，因为字符串中的匹配字符不需要是连续的。</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="withMatchLen">Finally to also have the match len
        /// <para>返回数据是否包含匹配的长度</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Returns an array with the LCS length and all the ranges in both the strings, start and end offset for each string, where there are matches.
        /// <para>When WITHMATCHLEN is given each array representing a match will also have the length of the match (see examples).</para>
        /// <para>返回一个数组，其中包含LCS长度和两个字符串中的所有范围，每个字符串的开始和结束偏移量，其中存在匹配。当给定 WITHMATCHLEN 时，表示匹配的每个数组也将具有匹配的长度</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public LcsValue? LcsIdx(string key1, string key2, bool withMatchLen = false, CancellationToken cancellationToken = default)
#else
        public LcsValue LcsIdx(string key1, string key2, bool withMatchLen = false, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallLcs(StringCommands.Lcs(key1, key2, len: false, idx: true, minMatchLen: 0, withMatchLen: withMatchLen), cancellationToken);
        }

        /// <summary>
        /// The LCS command implements the longest common subsequence algorithm. Note that this is different than the longest common string algorithm, since matching characters in the string does not need to be contiguous.
        /// <para>Available since: 7.0.0</para>
        /// <para>LCS命令实现了最长的公共子序列算法。请注意，这与最长公共字符串算法不同，因为字符串中的匹配字符不需要是连续的。</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="key1">key1</param>
        /// <param name="key2">key2</param>
        /// <param name="withMatchLen">Finally to also have the match len
        /// <para>是否包含每组匹配的长度</para>
        /// </param>
        /// <param name="minMatchLen">To restrict the list of matches to the ones of a given minimal length
        /// <para>匹配列表限制为给定的最小长度</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Returns an array with the LCS length and all the ranges in both the strings, start and end offset for each string, where there are matches.
        /// <para>When WITHMATCHLEN is given each array representing a match will also have the length of the match (see examples).</para>
        /// <para>返回一个数组，其中包含LCS长度和两个字符串中的所有范围，每个字符串的开始和结束偏移量，其中存在匹配。当给定 WITHMATCHLEN 时，表示匹配的每个数组也将具有匹配的长度</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public LcsValue? LcsIdx(string key1, string key2, uint minMatchLen, bool withMatchLen = false, CancellationToken cancellationToken = default)
#else
        public LcsValue LcsIdx(string key1, string key2, uint minMatchLen, bool withMatchLen = false, CancellationToken cancellationToken = default)
#endif
        {
            return this._call.CallLcs(StringCommands.Lcs(key1, key2, len: false, idx: true, minMatchLen: minMatchLen, withMatchLen: withMatchLen), cancellationToken);
        }
    }
}
