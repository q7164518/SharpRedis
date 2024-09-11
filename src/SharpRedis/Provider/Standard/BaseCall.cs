#if !LOW_NET
using System.Threading.Tasks;
using System.Threading;
#endif
using SharpRedis.Extensions;
using SharpRedis.Models;
using System;
using System.Collections.Generic;
using SharpRedis.Network.Standard;
using System.Text;

namespace SharpRedis.Provider.Standard
{
    internal abstract class BaseCall : IDisposable
    {
        private IConnectionPool _connectionPool;
        private protected bool _disposedValue;

        internal abstract bool SubUsable { get; }

        internal abstract string CallMode { get; }

        internal IConnectionPool ConnectionPool => this._connectionPool;

        internal Encoding Encoding => this._connectionPool.Encoding;

        internal BaseCall(IConnectionPool connectionPool)
        {
            this._connectionPool = connectionPool;
        }

        #region Sync
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal abstract object? Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken);

        internal abstract object?[]? Calls(CommandPacket[] commands, CancellationToken cancellationToken);
#else
        internal abstract object Call(CommandPacket command, ResultType resultType, CancellationToken cancellationToken);

        internal abstract object[] Calls(CommandPacket[] commands, CancellationToken cancellationToken);
#endif

        internal virtual void CallWithoutResult(CommandPacket command, CancellationToken cancellationToken)
        {
            this.Call(command, ResultType.Object, cancellationToken);
        }

        internal virtual bool CallCondition(CommandPacket command, string condition, CancellationToken cancellationToken)
        {
            var data = this.Call(command, ResultType.String, cancellationToken);
            if (data is null || data is DBNull) return false;
            var dataString = ConvertExtensions.To<string>(data, ResultType.String, this.Encoding);
            return dataString == condition;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual bool[]? CallConditionArray(CommandPacket command, string condition, CancellationToken cancellationToken)
#else
        internal virtual bool[] CallConditionArray(CommandPacket command, string condition, CancellationToken cancellationToken)
#endif
        {
            var data = this.Call(command, ResultType.Object, cancellationToken);
            if (data is null || data is DBNull) return null;
            if (data is object[] objs && objs.Length > 0)
            {
                var result = new bool[objs.Length];
                for (uint i = 0; i < objs.Length; i++)
                {
                    result[i] = ConvertExtensions.To<string>(objs[i], ResultType.String, this.Encoding) == condition;
                }
                return result;
            }
            return null;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual string? CallString(CommandPacket command, CancellationToken cancellationToken)
#else
        internal virtual string CallString(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            var data = this.Call(command, ResultType.String, cancellationToken);
            return ConvertExtensions.To<string>(data, ResultType.String, this.Encoding);
        }

        internal virtual bool CallBoolean(CommandPacket command, CancellationToken cancellationToken)
        {
            var data = this.Call(command, ResultType.Boolean, cancellationToken);
            return ConvertExtensions.To<bool>(data, ResultType.Boolean, this.Encoding);
        }

        internal virtual NumberValue CallNumber(CommandPacket command, CancellationToken cancellationToken)
        {
            var data = this.Call(command, ResultType.Number, cancellationToken);
            return ConvertExtensions.To<NumberValue>(data, ResultType.Number, this.Encoding) ?? NumberValue.Null;
        }

        internal virtual TNumber CallNumber<TNumber>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#if NET7_0_OR_GREATER
            where TNumber : struct, System.Numerics.INumber<TNumber>
#else
            where TNumber : struct, IEquatable<TNumber>
#endif
        {
            var data = this.Call(command, type, cancellationToken);
            return ConvertExtensions.To<TNumber>(data, type, this.Encoding);
        }

        internal virtual TNumber? CallNullNumber<TNumber>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#if NET7_0_OR_GREATER
            where TNumber : struct, System.Numerics.INumber<TNumber>
#else
            where TNumber : struct, IEquatable<TNumber>
#endif
        {
            var _type = type;
            if ((type & ResultType.Nullable) != ResultType.Nullable) _type |= ResultType.Nullable;
            var data = this.Call(command, _type, cancellationToken);
            return ConvertExtensions.To<TNumber?>(data, type, this.Encoding);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual TModel?[]? CallClassArray<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal virtual TModel[] CallClassArray<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : class
        {
            var _type = type;
            if ((type & ResultType.Array) != ResultType.Array) _type |= ResultType.Array;
            var data = this.Call(command, _type, cancellationToken);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<TModel?[]?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<TModel[]>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual TModel? CallClass<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal virtual TModel CallClass<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : class
        {
            var data = this.Call(command, type, cancellationToken);
            return ConvertExtensions.To<TModel>(data, type, this.Encoding);
        }

        internal virtual TModel CallStruct<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
            where TModel : struct
        {
            var data = this.Call(command, type, cancellationToken);
            return ConvertExtensions.To<TModel>(data, type, this.Encoding);
        }

        internal virtual TModel? CallNullableStruct<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
            where TModel : struct
        {
            var data = this.Call(command, type, cancellationToken);
            return ConvertExtensions.To<TModel?>(data, type, this.Encoding);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual TModel[]? CallStructArray<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal virtual TModel[] CallStructArray<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : struct
        {
            var _type = type;
            if ((type & ResultType.Array) != ResultType.Array) _type |= ResultType.Array;
            var data = this.Call(command, _type, cancellationToken);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<TModel[]?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<TModel[]>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual TModel?[]? CallNullableStructArray<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal virtual TModel?[] CallNullableStructArray<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : struct
        {
            var _type = type;
            if ((type & ResultType.Array) != ResultType.Array) _type |= ResultType.Array;
            var data = this.Call(command, _type, cancellationToken);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<TModel?[]?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<TModel?[]>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual Dictionary<string, TValue?>? CallDictionaryClassValue<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal virtual Dictionary<string, TValue> CallDictionaryClassValue<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TValue : class
        {
            var _type = type;
            if ((type & ResultType.Dictionary) != ResultType.Dictionary) _type |= ResultType.Dictionary;
            var data = this.Call(command, _type, cancellationToken);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<Dictionary<string, TValue?>?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<Dictionary<string, TValue>>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual Dictionary<string, TValue>? CallDictionaryStructValue<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal virtual Dictionary<string, TValue> CallDictionaryStructValue<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TValue : struct
        {
            var _type = type;
            if ((type & ResultType.Dictionary) != ResultType.Dictionary) _type |= ResultType.Dictionary;
            var data = this.Call(command, _type, cancellationToken);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<Dictionary<string, TValue>?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<Dictionary<string, TValue>>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual LcsValue? CallLcs(CommandPacket command, CancellationToken cancellationToken)
#else
        internal virtual LcsValue CallLcs(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            var data = this.Call(command, ResultType.Lcs, cancellationToken);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<LcsValue?>(data, ResultType.Lcs, this.Encoding);
#else
            return ConvertExtensions.To<LcsValue>(data, ResultType.Lcs, this.Encoding);
#endif
        }

        unsafe internal virtual TEnum CallEnum<TEnum>(CommandPacket command, CancellationToken cancellationToken)
            where TEnum : unmanaged, Enum
        {
            var data = this.Call(command, ResultType.Enum, cancellationToken);
            var status = ConvertExtensions.To<int>(data, ResultType.Enum, this.Encoding);
            if (!Enum.IsDefined(typeof(TEnum), status))
            {
                throw new FormatException($"Value {status} is not defined in enum {typeof(TEnum).FullName}");
            }
            return *(TEnum*)&status;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual TEnum[]? CallEnumArray<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#else
        internal virtual TEnum[] CallEnumArray<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#endif
            where TEnum : unmanaged, Enum
        {
            var data = this.Call(command, ResultType.Array | ResultType.Enum, cancellationToken);
            var statusArray = ConvertExtensions.To<int[]>(data, ResultType.Array | ResultType.Enum, this.Encoding);
            if (statusArray is null || statusArray.Length == 0) return null;
            return Extend.IntsToEnums<TEnum>(statusArray);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual ScanValue<TData>? CallScan<TData>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal virtual ScanValue<TData> CallScan<TData>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TData : class, System.Collections.ICollection
        {
            var data = this.Call(command, type, cancellationToken);
            return ConvertExtensions.To<ScanValue<TData>>(data, type, this.Encoding);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual byte[]? CallBytes(CommandPacket command, CancellationToken cancellationToken)
#else
        internal virtual byte[] CallBytes(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            var data = this.Call(command, ResultType.Bytes, cancellationToken);
            return ConvertExtensions.To<byte[]>(data, ResultType.Bytes, this.Encoding);
        }
#endregion

        #region Async
#if !LOW_NET
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal abstract Task<object?> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken);

        internal abstract Task<object?[]?> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken);
#else
        internal abstract Task<object> CallAsync(CommandPacket command, ResultType resultType, CancellationToken cancellationToken);

        internal abstract Task<object[]> CallsAsync(CommandPacket[] commands, CancellationToken cancellationToken);
#endif

        internal virtual Task CallWithoutResultAsync(CommandPacket command, CancellationToken cancellationToken)
        {
            return this.CallAsync(command, ResultType.Object, cancellationToken);
        }

        internal virtual async Task<bool> CallConditionAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
        {
            var data = await this.CallAsync(command, ResultType.String, cancellationToken).ConfigureAwait(false);
            if (data is null || data is DBNull) return false;
            var dataString = ConvertExtensions.To<string>(data, ResultType.String, this.Encoding);
            return dataString == condition;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<bool[]?> CallConditionArrayAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
#else
        async internal virtual Task<bool[]> CallConditionArrayAsync(CommandPacket command, string condition, CancellationToken cancellationToken)
#endif
        {
            var data = await this.CallAsync(command, ResultType.Object, cancellationToken).ConfigureAwait(false);
            if (data is null || data is DBNull) return null;
            if (data is object[] objs && objs.Length > 0)
            {
                var result = new bool[objs.Length];
                for (uint i = 0; i < objs.Length; i++)
                {
                    result[i] = ConvertExtensions.To<string>(objs[i], ResultType.String, this.Encoding) == condition;
                }
                return result;
            }
            return null;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual async Task<string?> CallStringAsync(CommandPacket command, CancellationToken cancellationToken)
#else
        internal virtual async Task<string> CallStringAsync(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            var data = await this.CallAsync(command, ResultType.String, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<string>(data, ResultType.String, this.Encoding);
        }

        internal virtual async Task<bool> CallBooleanAsync(CommandPacket command, CancellationToken cancellationToken)
        {
            var data = await this.CallAsync(command, ResultType.Boolean, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<bool>(data, ResultType.Boolean, this.Encoding);
        }

        async internal virtual Task<NumberValue> CallNumberAsync(CommandPacket command, CancellationToken cancellationToken)
        {
            var data = await this.CallAsync(command, ResultType.Number, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<NumberValue>(data, ResultType.Number, this.Encoding) ?? NumberValue.Null;
        }

        internal async virtual Task<TNumber> CallNumberAsync<TNumber>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#if NET7_0_OR_GREATER
            where TNumber : struct, System.Numerics.INumber<TNumber>
#else
            where TNumber : struct, IEquatable<TNumber>
#endif
        {
            var data = await this.CallAsync(command, type, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<TNumber>(data, type, this.Encoding);
        }

        internal async virtual Task<TNumber?> CallNullNumberAsync<TNumber>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#if NET7_0_OR_GREATER
            where TNumber : struct, System.Numerics.INumber<TNumber>
#else
            where TNumber : struct, IEquatable<TNumber>
#endif
        {
            var _type = type;
            if ((type & ResultType.Nullable) != ResultType.Nullable) _type |= ResultType.Nullable;
            var data = await this.CallAsync(command, _type, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<TNumber?>(data, type, this.Encoding);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<TModel?[]?> CallClassArrayAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        async internal virtual Task<TModel[]> CallClassArrayAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : class
        {
            var _type = type;
            if ((type & ResultType.Array) != ResultType.Array) _type |= ResultType.Array;
            var data = await this.CallAsync(command, _type, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<TModel?[]?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<TModel[]>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal async virtual Task<TModel?> CallClassAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        internal async virtual Task<TModel> CallClassAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : class
        {
            var data = await this.CallAsync(command, type, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<TModel>(data, type, this.Encoding);
        }

        async internal virtual Task<TModel> CallStructAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
            where TModel : struct
        {
            var data = await this.CallAsync(command, type, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<TModel>(data, type, this.Encoding);
        }

        async internal virtual Task<TModel?> CallNullableStructAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
            where TModel : struct
        {
            var data = await this.CallAsync(command, type, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<TModel?>(data, type, this.Encoding);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<TModel[]?> CallStructArrayAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        async internal virtual Task<TModel[]> CallStructArrayAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : struct
        {
            var _type = type;
            if ((type & ResultType.Array) != ResultType.Array) _type |= ResultType.Array;
            var data = await this.CallAsync(command, _type, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<TModel[]?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<TModel[]>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<TModel?[]?> CallNullableStructArrayAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        async internal virtual Task<TModel?[]> CallNullableStructArrayAsync<TModel>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TModel : struct
        {
            var _type = type;
            if ((type & ResultType.Array) != ResultType.Array) _type |= ResultType.Array;
            var data = await this.CallAsync(command, _type, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<TModel?[]?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<TModel?[]>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<Dictionary<string, TValue?>?> CallDictionaryClassValueAsync<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        async internal virtual Task<Dictionary<string, TValue>> CallDictionaryClassValueAsync<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TValue : class
        {
            var _type = type;
            if ((type & ResultType.Dictionary) != ResultType.Dictionary) _type |= ResultType.Dictionary;
            var data = await this.CallAsync(command, _type, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<Dictionary<string, TValue?>?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<Dictionary<string, TValue>>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<Dictionary<string, TValue>?> CallDictionaryStructValueAsync<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        async internal virtual Task<Dictionary<string, TValue>> CallDictionaryStructValueAsync<TValue>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TValue : struct
        {
            var _type = type;
            if ((type & ResultType.Dictionary) != ResultType.Dictionary) _type |= ResultType.Dictionary;
            var data = await this.CallAsync(command, _type, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<Dictionary<string, TValue>?>(data, type, this.Encoding);
#else
            return ConvertExtensions.To<Dictionary<string, TValue>>(data, type, this.Encoding);
#endif
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal virtual async Task<LcsValue?> CallLcsAsync(CommandPacket command, CancellationToken cancellationToken)
#else
        internal virtual async Task<LcsValue> CallLcsAsync(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            var data = await this.CallAsync(command, ResultType.Lcs, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return ConvertExtensions.To<LcsValue?>(data, ResultType.Lcs, this.Encoding);
#else
            return ConvertExtensions.To<LcsValue>(data, ResultType.Lcs, this.Encoding);
#endif
        }

        async internal virtual Task<TEnum> CallEnumAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
            where TEnum : unmanaged, Enum
        {
            var data = await this.CallAsync(command, ResultType.Enum, cancellationToken).ConfigureAwait(false);
            var status = ConvertExtensions.To<int>(data, ResultType.Enum, this.Encoding);
            return Extend.IntToEnum<TEnum>(status);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal async virtual Task<TEnum[]?> CallEnumArrayAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#else
        internal async virtual Task<TEnum[]> CallEnumArrayAsync<TEnum>(CommandPacket command, CancellationToken cancellationToken)
#endif
            where TEnum : unmanaged, Enum
        {
            var data = await this.CallAsync(command, ResultType.Array | ResultType.Enum, cancellationToken).ConfigureAwait(false);
            var statusArray = ConvertExtensions.To<int[]>(data, ResultType.Array | ResultType.Enum, this.Encoding);
            if (statusArray is null || statusArray.Length == 0) return null;
            return Extend.IntsToEnums<TEnum>(statusArray);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<ScanValue<TData>?> CallScanAsync<TData>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#else
        async internal virtual Task<ScanValue<TData>> CallScanAsync<TData>(CommandPacket command, ResultType type, CancellationToken cancellationToken)
#endif
            where TData : class, System.Collections.ICollection
        {
            var data = await this.CallAsync(command, type, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<ScanValue<TData>>(data, type, this.Encoding);
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        async internal virtual Task<byte[]?> CallBytesAsync(CommandPacket command, CancellationToken cancellationToken)
#else
        async internal virtual Task<byte[]> CallBytesAsync(CommandPacket command, CancellationToken cancellationToken)
#endif
        {
            var data = await this.CallAsync(command, ResultType.Bytes, cancellationToken).ConfigureAwait(false);
            return ConvertExtensions.To<byte[]>(data, ResultType.Bytes, this.Encoding);
        }
#endif
#endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                }
            }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
            this._connectionPool = null;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
