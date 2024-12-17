#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRedis.Extensions
{
    internal static class ConvertExtensions
    {
#if NET5_0_OR_GREATER
        internal static T? To<T>(object? data, in ResultType type, Encoding encoding)
#elif NETSTANDARD2_1_OR_GREATER
        internal static T To<T>(object? data, in ResultType type, Encoding encoding)
#else
        internal static T To<T>(object data, in ResultType type, Encoding encoding)
#endif
        {
            if (data is null || data is DBNull)
            {
#if NETSTANDARD2_1_OR_GREATER
                return default!;
#else
                return default;
#endif
            }
            if (data is Exception ex) throw ex;

            ResultType _type = type;
            if ((type & (type - 1)) != 0)
            {
                if ((type & ResultType.Scan) == ResultType.Scan)
                {
                    _type = ResultType.Scan;
                }
                else if ((type & ResultType.Array) == ResultType.Array
                    && (type & ResultType.KeyValuePair) != ResultType.KeyValuePair
                    && (type & (ResultType.Dictionary | ResultType.Stream)) != ((ResultType.Dictionary | ResultType.Stream)))
                {
                    _type = ResultType.Array;
                }
                else if ((type & ResultType.KeyValuePairArray) == ResultType.KeyValuePairArray)
                {
                    _type = ResultType.Array;
                }
                else if ((type & ResultType.Dictionary) == ResultType.Dictionary)
                {
                    _type = ResultType.Dictionary;
                }
                else if ((type & ResultType.MemberScoreValue) == ResultType.MemberScoreValue && (type & ResultType.KeyValuePair) != ResultType.KeyValuePair)
                {
                    _type = ResultType.MemberScoreValue;
                }
                else if ((type & ResultType.KeyValuePair) == ResultType.KeyValuePair)
                {
                    _type = ResultType.KeyValuePair;
                }
                else if ((type & ResultType.Nullable) == ResultType.Nullable)
                {
                    _type = type & ~ResultType.Nullable;
                }
                else if ((type & ResultType.XAutoClaimValue) == ResultType.XAutoClaimValue)
                {
                    _type = ResultType.XAutoClaimValue;
                }
                else if (type == (ResultType.Stream | ResultType.String) || type == (ResultType.Stream | ResultType.Bytes))
                {
                    _type = ResultType.Stream;
                }
                else if ((type & ResultType.XInfoStreamValue) == ResultType.XInfoStreamValue)
                {
                    _type = ResultType.XInfoStreamValue;
                }
                else if ((type & ResultType.XInfoStreamFullValue) == ResultType.XInfoStreamFullValue)
                {
                    _type = ResultType.XInfoStreamFullValue;
                }
                else
                {
                    throw new NotSupportedException("Unsupported type");
                }
            }

            switch (_type)
            {
                case ResultType.String:
                    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                        string? stringValue;
#else
                        string stringValue;
#endif
                        if (data is byte[] bytes)
                        {
                            if (bytes.Length is 0) stringValue = string.Empty;
                            else
                            {
                                stringValue = encoding.GetString(bytes);
                            }
                        }
                        else if (data is object[] array && array.Length is 1)
                        {
                            stringValue = ConvertExtensions.To<string>(array[0], ResultType.String, encoding);
                        }
                        else
                        {
                            stringValue = data.ToString();
                        }
                        if (stringValue is null)
                        {
#if NETSTANDARD2_1_OR_GREATER
                            return default!;
#else
                            return default;
#endif
                        }
                        if (stringValue is T res) return res;
                        throw new FormatException($"The data is not a valid String, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Number:
                    {
                        if (data is NumberValue number && number is T result)
                        {
                            return result;
                        }
                        if (data is object[] objArray && objArray.Length is 1)
                        {
                            return ConvertExtensions.To<T>(objArray[0], ResultType.Number, encoding);
                        }
                        if (data is null || data is DBNull)
                        {
                            var nv = NumberValue.Null;
                            if (nv is T res) return res;
                        }
                        if (data is byte[] bytes)
                        {
                            var numberString = encoding.GetString(bytes);
                            var nv = new NumberValue(numberString, -1);
                            if (nv is T res) return res;
                        }
                        throw new FormatException($"The data is not a valid number, The actual type is {data?.GetType().FullName}");
                    }
                case ResultType.Int32:
                    {
                        int numberValue;
                        if (data is NumberValue number)
                        {
                            numberValue = number.ToInt32();
                        }
                        else if (data is byte[] bytes)
                        {
                            numberValue = int.Parse(encoding.GetString(bytes));
                        }
                        else if (data is object[] objArray && objArray.Length is 1)
                        {
                            return ConvertExtensions.To<T>(objArray[0], ResultType.Int32, encoding);
                        }
                        else if (data is string numberString)
                        {
                            numberValue = int.Parse(numberString);
                        }
                        else if (data is int intNumber)
                        {
                            numberValue = intNumber;
                        }
                        else
                        {
                            throw new FormatException($"The data is not a valid Int32, The actual type is {data.GetType().FullName}");
                        }

                        if ((type & ResultType.Nullable) == ResultType.Nullable)
                        {
                            int? nullableInt = numberValue;
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (nullableInt is T res) return res;
                                throw new FormatException($"The data is not a valid Int32, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<int?, T>(ref nullableInt);
#else
                            if (nullableInt is T res) return res;
                            throw new FormatException($"The data is not a valid Int32, The actual type is {data.GetType().FullName}");
#endif
                        }
                        else
                        {
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (numberValue is T res) return res;
                                throw new FormatException($"The data is not a valid Int32, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<int, T>(ref numberValue);
#else
                            if (numberValue is T res) return res;
                            throw new FormatException($"The data is not a valid Int32, The actual type is {data.GetType().FullName}");
#endif
                        }
                    }
                case ResultType.Int64:
                    {
                        long numberValue;
                        if (data is NumberValue number)
                        {
                            numberValue = number.ToInt64();
                        }
                        else if (data is byte[] bytes)
                        {
                            numberValue = long.Parse(encoding.GetString(bytes));
                        }
                        else if (data is object[] objArray && objArray.Length is 1)
                        {
                            return ConvertExtensions.To<T>(objArray[0], ResultType.Int64, encoding);
                        }
                        else if (data is string numberString)
                        {
                            numberValue = long.Parse(numberString);
                        }
                        else if (data is long longNumber)
                        {
                            numberValue = longNumber;
                        }
                        else
                        {
                            throw new FormatException($"The data is not a valid Int64, The actual type is {data.GetType().FullName}");
                        }
                        if ((type & ResultType.Nullable) == ResultType.Nullable)
                        {
                            long? nullableLong = numberValue;
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (nullableLong is T res) return res;
                                throw new FormatException($"The data is not a valid Int64, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<long?, T>(ref nullableLong);
#else
                            if (nullableLong is T res) return res;
                            throw new FormatException($"The data is not a valid Int64, The actual type is {data.GetType().FullName}");
#endif
                        }
                        else
                        {
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (numberValue is T res) return res;
                                throw new FormatException($"The data is not a valid Int64, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<long, T>(ref numberValue);
#else
                            if (numberValue is T res) return res;
                            throw new FormatException($"The data is not a valid Int64, The actual type is {data.GetType().FullName}");
#endif
                        }
                    }
                case ResultType.UInt64:
                    {
                        ulong numberValue;
                        if (data is NumberValue number)
                        {
                            numberValue = number.ToUInt64();
                        }
                        else if (data is byte[] bytes)
                        {
                            numberValue = ulong.Parse(encoding.GetString(bytes));
                        }
                        else if (data is object[] objArray && objArray.Length is 1)
                        {
                            return ConvertExtensions.To<T>(objArray[0], ResultType.UInt64, encoding);
                        }
                        else if (data is string numberString)
                        {
                            numberValue = ulong.Parse(numberString);
                        }
                        else if (data is ulong ulongNumber)
                        {
                            numberValue = ulongNumber;
                        }
                        else
                        {
                            throw new FormatException($"The data is not a valid UInt64, The actual type is {data.GetType().FullName}");
                        }
                        if ((type & ResultType.Nullable) == ResultType.Nullable)
                        {
                            ulong? nullableULong = numberValue;
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (nullableULong is T res) return res;
                                throw new FormatException($"The data is not a valid UInt64, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<ulong?, T>(ref nullableULong);
#else
                            if (nullableULong is T res) return res;
                            throw new FormatException($"The data is not a valid UInt64, The actual type is {data.GetType().FullName}");
#endif
                        }
                        else
                        {
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (numberValue is T res) return res;
                                throw new FormatException($"The data is not a valid UInt64, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<ulong, T>(ref numberValue);
#else
                            if (numberValue is T res) return res;
                            throw new FormatException($"The data is not a valid UInt64, The actual type is {data.GetType().FullName}");
#endif
                        }
                    }
                case ResultType.Double:
                    {
                        double numberValue;
                        if (data is NumberValue number)
                        {
                            numberValue = number.ToDouble();
                        }
                        else if (data is byte[] bytes)
                        {
                            numberValue = double.Parse(encoding.GetString(bytes));
                        }
                        else if (data is object[] objArray && objArray.Length is 1)
                        {
                            return ConvertExtensions.To<T>(objArray[0], ResultType.Double, encoding);
                        }
                        else if (data is string numberString)
                        {
                            numberValue = double.Parse(numberString);
                        }
                        else if (data is double doubleNumber)
                        {
                            numberValue = doubleNumber;
                        }
                        else
                        {
                            throw new FormatException($"The data is not a valid Double, The actual type is {data.GetType().FullName}");
                        }
                        if ((type & ResultType.Nullable) == ResultType.Nullable)
                        {
                            double? nullableDouble = numberValue;
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (nullableDouble is T res) return res;
                                throw new FormatException($"The data is not a valid Double, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<double?, T>(ref nullableDouble);
#else
                            if (nullableDouble is T res) return res;
                            throw new FormatException($"The data is not a valid Double, The actual type is {data.GetType().FullName}");
#endif
                        }
                        else
                        {
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (numberValue is T res) return res;
                                throw new FormatException($"The data is not a valid Double, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<double, T>(ref numberValue);
#else
                            if (numberValue is T res) return res;
                            throw new FormatException($"The data is not a valid Double, The actual type is {data.GetType().FullName}");
#endif
                        }
                    }
                case ResultType.Boolean:
                    {
                        if (data is BooleanValue boolean)
                        {
                            var boolValue = boolean.ToBoolean();
#if NET5_0_OR_GREATER
                            if (typeof(T).FullName == "System.Object")
                            {
                                if (boolValue is T res) return res;
                                throw new FormatException($"The data is not a valid Boolean, The actual type is {data.GetType().FullName}");
                            }
                            return Unsafe.As<bool, T>(ref boolValue);
#else
                            if (boolValue is T res) return res;
                            throw new FormatException($"The data is not a valid Boolean, The actual type is {data.GetType().FullName}");
#endif
                        }
                        throw new FormatException($"The data is not a valid Boolean, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Coordinate:
                    {
                        if (data is object[] array && array.Length > 0)
                        {
                            if (array[0] is null || array[0] is DBNull)
                            {
#if NETSTANDARD2_1_OR_GREATER
                                return default!;
#else
                                return default;
#endif
                            }

                            var result = new CoordinateValue(array, encoding);
                            if ((type & ResultType.Nullable) == ResultType.Nullable)
                            {
                                CoordinateValue? nullableCoordinate = result;
#if NET5_0_OR_GREATER
                                if (typeof(T).FullName == "System.Object")
                                {
                                    if (nullableCoordinate is T res) return res;
                                    throw new FormatException($"The data is not a valid CoordinateValue, The actual type is {data.GetType().FullName}");
                                }
                                return Unsafe.As<CoordinateValue?, T>(ref nullableCoordinate);
#else
                                if (nullableCoordinate is T res) return res;
                                throw new FormatException($"The data is not a valid CoordinateValue, The actual type is {data.GetType().FullName}");
#endif
                            }
                            else
                            {
#if NET5_0_OR_GREATER
                                if (typeof(T).FullName == "System.Object")
                                {
                                    if (result is T res) return res;
                                    throw new FormatException($"The data is not a valid CoordinateValue, The actual type is {data.GetType().FullName}");
                                }
                                return Unsafe.As<CoordinateValue, T>(ref result);
#else
                                if (result is T res) return res;
                                throw new FormatException($"The data is not a valid CoordinateValue, The actual type is {data.GetType().FullName}");
#endif
                            }
                        }
                        throw new FormatException($"The data is not a valid CoordinateValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Dictionary:
                    {
                        var valueType = type & ~ResultType.Dictionary;
                        if (valueType is ResultType.String)
                        {
                            if (data is Dictionary<string, object> dictObj)
                            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                var hashResult = new Dictionary<string, string?>(dictObj.Count);
#else
                                var hashResult = new Dictionary<string, string>(dictObj.Count);
#endif
                                foreach (var item in dictObj)
                                {
                                    hashResult[item.Key] = ConvertExtensions.To<string>(item.Value, ResultType.String, encoding);
                                }
                                if (hashResult is T hashVal) return hashVal;
                            }
                            else if (data is object[] hashArray)
                            {
                                var hashResult = new Dictionary<string, string>();
                                if (hashArray[0] is object[])
                                {
                                    for (uint i = 0; i < hashArray.Length; i++)
                                    {
                                        if (hashArray[i] is object[] itemArray && itemArray.Length == 2)
                                        {
                                            var key = ConvertExtensions.To<string>(itemArray[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                            var value = ConvertExtensions.To<string>(itemArray[1], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                            hashResult[key] = value;
                                        }
                                        else
                                        {
                                            throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        }
                                    }
                                    if (hashResult is T arrayResult) return arrayResult;
                                }
                                else
                                {
                                    if (hashArray.Length % 2 != 0)
                                    {
                                        throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                    }
                                    for (uint i = 0; i < hashArray.Length; i += 2)
                                    {
                                        var key = ConvertExtensions.To<string>(hashArray[i], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        var value = ConvertExtensions.To<string>(hashArray[i + 1], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        hashResult[key] = value;
                                    }
                                    if (hashResult is T hashVal) return hashVal;
                                }
                            }
                        }
                        else if (valueType is ResultType.Bytes)
                        {
                            if (data is Dictionary<string, object> dictObj)
                            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                var hashResult = new Dictionary<string, byte[]?>(dictObj.Count);
#else
                                var hashResult = new Dictionary<string, byte[]>(dictObj.Count);
#endif
                                foreach (var item in dictObj)
                                {
                                    hashResult[item.Key] = ConvertExtensions.To<byte[]>(item.Value, ResultType.Bytes, encoding);
                                }
                                if (hashResult is T hashVal) return hashVal;
                            }
                            else if (data is object[] hashArray)
                            {
                                var hashResult = new Dictionary<string, byte[]>();
                                if (hashArray[0] is object[])
                                {
                                    for (uint i = 0; i < hashArray.Length; i++)
                                    {
                                        if (hashArray[i] is object[] itemArray && itemArray.Length == 2)
                                        {
                                            var key = ConvertExtensions.To<string>(itemArray[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                            var value = ConvertExtensions.To<byte[]>(itemArray[1], ResultType.Bytes, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                            hashResult[key] = value;
                                        }
                                        else
                                        {
                                            throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        }
                                    }
                                    if (hashResult is T arrayResult) return arrayResult;
                                }
                                else
                                {
                                    if (hashArray.Length % 2 != 0)
                                    {
                                        throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                    }
                                    for (uint i = 0; i < hashArray.Length; i += 2)
                                    {
                                        var key = ConvertExtensions.To<string>(hashArray[i], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        var value = ConvertExtensions.To<byte[]>(hashArray[i + 1], ResultType.Bytes, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        hashResult[key] = value;
                                    }
                                    if (hashResult is T hashVal) return hashVal;
                                }
                            }
                        }
                        else if (valueType is ResultType.Object)
                        {
                            if (data is object[] hashArray)
                            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                var hashResult = new Dictionary<string, object?>();
#else
                                var hashResult = new Dictionary<string, object>();
#endif
                                if (hashArray[0] is object[])
                                {
                                    for (uint i = 0; i < hashArray.Length; i++)
                                    {
                                        if (hashArray[i] is object[] itemArray && itemArray.Length == 2)
                                        {
                                            var key = ConvertExtensions.To<string>(itemArray[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                            var value = ConvertExtensions.To<object>(itemArray[1], ResultType.Object, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                            hashResult[key] = value;
                                        }
                                        else
                                        {
                                            throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        }
                                    }
                                    if (hashResult is T arrayResult) return arrayResult;
                                }
                                else
                                {
                                    if (hashArray.Length % 2 != 0)
                                    {
                                        throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                    }
                                    for (uint i = 0; i < hashArray.Length; i += 2)
                                    {
                                        var key = ConvertExtensions.To<string>(hashArray[i], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                                        var value = ConvertExtensions.To<object>(hashArray[i + 1], ResultType.Object, encoding);
                                        hashResult[key] = value;
                                    }
                                    if (hashResult is T hashVal) return hashVal;
                                }
                            }
                        }
                        else if (valueType == (ResultType.Array | ResultType.Stream | ResultType.String))
                        {
                            if (data is object[] array)
                            {
                                if (array.Length is 0) return ConvertExtensions.To<T>(null, type, encoding);
                                var result = new Dictionary<string, StreamValue<string>[]>();
                                for (int i = 0; i < array.Length; i++)
                                {
                                    if (array[i] is object[] item && item.Length is 2)
                                    {
                                        var key = ConvertExtensions.To<string>(item[0], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                            !
#endif
                                            ;
                                        var streamValue = ConvertExtensions.To<StreamValue<string>[]>(item[1], valueType, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                            !
#endif
                                            ;
                                        result.Add(key, streamValue);
                                    }
                                }
                                if (result is T _res) return _res;
                            }
                            throw new FormatException($"The data is not a valid Dictionary<string, StreamValue>, The actual type is {data.GetType().FullName}");
                        }
                        else if (valueType == (ResultType.Array | ResultType.Stream | ResultType.Bytes))
                        {
                            if (data is object[] array)
                            {
                                if (array.Length is 0) return ConvertExtensions.To<T>(null, type, encoding);
                                var result = new Dictionary<string, StreamValue<byte[]>[]>();
                                for (int i = 0; i < array.Length; i++)
                                {
                                    if (array[i] is object[] item && item.Length is 2)
                                    {
                                        var key = ConvertExtensions.To<string>(item[0], ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                            !
#endif
                                            ;
                                        var streamValue = ConvertExtensions.To<StreamValue<byte[]>[]>(item[1], valueType, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                            !
#endif
                                            ;
                                        result.Add(key, streamValue);
                                    }
                                }
                                if (result is T _res) return _res;
                            }
                            throw new FormatException($"The data is not a valid Dictionary<string, StreamValue>, The actual type is {data.GetType().FullName}");
                        }
                        else if (data is T _hashVal) return _hashVal;
                        else
                        {
                            throw new NotSupportedException($"Dictionary value {valueType} type is not supported");
                        }
                        if (data is T res) return res;
                        throw new FormatException($"The data is not a valid Dictionary, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Array:
                    {
                        var valueType = type & ~ResultType.Array;
                        if (valueType == ResultType.String)
                        {
                            if (data is string[] strArray && strArray is T res) return res;
                            if (data is object[] arrayObject)
                            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                var result = new string?[arrayObject.Length];
#else
                                var result = new string[arrayObject.Length];
#endif
                                for (uint i = 0; i < arrayObject.Length; i++)
                                {
                                    result[i] = ConvertExtensions.To<string>(arrayObject[i], ResultType.String, encoding);
                                }
                                if (result is T arrayResult) return arrayResult;
                            }
                            else if (data is byte[])
                            {
                                var str = ConvertExtensions.To<string>(data, ResultType.String, encoding);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                var result = new string?[] { str };
#else
                                var result = new string[] { str };
#endif
                                if (result is T arrayResult) return arrayResult;
                            }
                        }
                        else if ((valueType & ResultType.Int64) == ResultType.Int64)
                        {
                            if (data is object[] arrayObject)
                            {
                                if ((valueType & ResultType.Nullable) == ResultType.Nullable)
                                {
                                    var result = new long?[arrayObject.Length];
                                    for (uint i = 0; i < arrayObject.Length; i++)
                                    {
                                        result[i] = ConvertExtensions.To<long?>(arrayObject[i], valueType, encoding);
                                    }
                                    if (result is T arrayResult) return arrayResult;
                                }
                                else
                                {
                                    var result = new long[arrayObject.Length];
                                    for (uint i = 0; i < arrayObject.Length; i++)
                                    {
                                        result[i] = ConvertExtensions.To<long>(arrayObject[i], valueType, encoding);
                                    }
                                    if (result is T arrayResult) return arrayResult;
                                }
                            }
                            else
                            {
                                if ((valueType & ResultType.Nullable) == ResultType.Nullable)
                                {
                                    var number = ConvertExtensions.To<long?>(data, valueType, encoding);
                                    var result = new long?[] { number };
                                    if (result is T arrayResult) return arrayResult;
                                }
                                else
                                {
                                    var number = ConvertExtensions.To<long>(data, valueType, encoding);
                                    var result = new long[] { number };
                                    if (result is T arrayResult) return arrayResult;
                                }
                            }
                        }
                        else if (valueType == ResultType.Number)
                        {
                            if (data is object[] arrayObject)
                            {
                                var result = new NumberValue[arrayObject.Length];
                                for (uint i = 0; i < arrayObject.Length; i++)
                                {
                                    result[i] = ConvertExtensions.To<NumberValue>(arrayObject[i], ResultType.Number, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                        !
#endif
                                        ;
                                    if (result[i] is null) result[i] = NumberValue.Null;
                                }
                                if (result is T arrayResult) return arrayResult;
                            }
                            else
                            {
                                var number = ConvertExtensions.To<NumberValue>(data, ResultType.Number, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                        !
#endif
                                        ;
                                var result = new NumberValue[] { number };
                                if (result is T arrayResult) return arrayResult;
                            }
                        }
                        else if (valueType is ResultType.KeyValuePair)
                        {
                            if (data is object[] array)
                            {
                                if (array[0] is object[])
                                {
                                    var result = new KeyValuePair<string, string>[array.Length];
                                    for (uint i = 0; i < array.Length; i++)
                                    {
                                        if (array[i] is object[] itemArray && itemArray.Length == 2)
                                        {
                                            var field = ConvertExtensions.To<string>(itemArray[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair array, The actual type is {data.GetType().FullName}");
                                            var value = ConvertExtensions.To<string>(itemArray[1], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair array, The actual type is {data.GetType().FullName}");
                                            result[i] = new KeyValuePair<string, string>(field, value);
                                        }
                                        else
                                        {
                                            throw new FormatException($"The data is not a valid KeyValuePair array, The actual type is {data.GetType().FullName}");
                                        }
                                    }
                                    if (result is T arrayResult) return arrayResult;
                                }
                                else
                                {
                                    if (array.Length % 2 != 0)
                                    {
                                        throw new FormatException($"The data is not a valid KeyValuePair[], The actual type is {data.GetType().FullName}");
                                    }
                                    var result = new KeyValuePair<string, string>[array.Length / 2];
                                    for (uint i = 0, z = 0; i < array.Length; i += 2, z++)
                                    {
                                        var field = ConvertExtensions.To<string>(array[i], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair array, The actual type is {data.GetType().FullName}");
                                        var value = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair array, The actual type is {data.GetType().FullName}");
                                        result[z] = new KeyValuePair<string, string>(field, value);
                                    }
                                    if (result is T arrayResult) return arrayResult;
                                }
                            }
                        }
                        else if (valueType is ResultType.Enum)
                        {
                            if (data is object[] arrayObject)
                            {
                                var result = new int[arrayObject.Length];
                                for (uint i = 0; i < arrayObject.Length; i++)
                                {
                                    result[i] = ConvertExtensions.To<int>(arrayObject[i], ResultType.Int32, encoding);
                                }
                                if (result is T arrayResult) return arrayResult;
                            }
                        }
                        else if (valueType is ResultType.Bytes)
                        {
                            if (data is object[] arrayObject)
                            {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                byte[]?[] bytesArray = new byte[arrayObject.Length][];
#else
                                byte[][] bytesArray = new byte[arrayObject.Length][];
#endif
                                for (uint i = 0; i < arrayObject.Length; i++)
                                {
                                    bytesArray[i] = ConvertExtensions.To<byte[]>(arrayObject[i], ResultType.Bytes, encoding);
                                }
                                if (bytesArray is T result) return result;
                            }
                            else if (data is byte[])
                            {
                                var bytes = ConvertExtensions.To<byte[]>(data, ResultType.Bytes, encoding);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                var bytesArray = new byte[]?[] { bytes };
#else
                                var bytesArray = new byte[][] { bytes };
#endif
                                if (bytesArray is T result) return result;
                            }
                        }
                        else if ((valueType & ResultType.MemberScoreValue) == ResultType.MemberScoreValue)
                        {
                            if (data is object[] array)
                            {
                                var memberType = valueType & ~ResultType.MemberScoreValue;
                                if (memberType == ResultType.String)
                                {
                                    if (array[0] is object[]) //resp3
                                    {
                                        var result = new MemberScoreValue<string>[array.Length];
                                        for (int i = 0; i < array.Length; i++)
                                        {
                                            result[i] = ConvertExtensions.To<MemberScoreValue<string>>(array[i], valueType, encoding);
                                        }
                                        if (result is T res) return res;
                                    }
                                    else
                                    {
                                        //resp2
                                        if (array.Length % 2 != 0)
                                        {
                                            throw new FormatException($"The data is not a valid MemberScoreValue array, The actual type is {data.GetType().FullName}");
                                        }
                                        var itemArray = new object[2];
                                        var result = new MemberScoreValue<string>[array.Length / 2];
                                        for (int i = 0, index = 0; i < array.Length; i += 2, index++)
                                        {
                                            itemArray[0] = array[i];
                                            itemArray[1] = array[i + 1];
                                            result[index] = ConvertExtensions.To<MemberScoreValue<string>>(itemArray, valueType, encoding);
                                        }
                                        itemArray = null;
                                        if (result is T res) return res;
                                    }
                                }
                                else if (memberType == ResultType.Bytes)
                                {
                                    if (array[0] is object[]) //resp3
                                    {
                                        var result = new MemberScoreValue<byte[]>[array.Length];
                                        for (int i = 0; i < array.Length; i++)
                                        {
                                            result[i] = ConvertExtensions.To<MemberScoreValue<byte[]>>(array[i], valueType, encoding);
                                        }
                                        if (result is T res) return res;
                                    }
                                    else
                                    {
                                        //resp2
                                        if (array.Length % 2 != 0)
                                        {
                                            throw new FormatException($"The data is not a valid MemberScoreValue array, The actual type is {data.GetType().FullName}");
                                        }
                                        var itemArray = new object[2];
                                        var result = new MemberScoreValue<byte[]>[array.Length / 2];
                                        for (int i = 0, index = 0; i < array.Length; i += 2, index++)
                                        {
                                            itemArray[0] = array[i];
                                            itemArray[1] = array[i + 1];
                                            result[index] = ConvertExtensions.To<MemberScoreValue<byte[]>>(itemArray, valueType, encoding);
                                        }
                                        itemArray = null;
                                        if (result is T res) return res;
                                    }
                                }
                            }

                        }
                        else if ((valueType & ResultType.KeyValuePairArray) == ResultType.KeyValuePairArray)
                        {
                            if (data is object[] array)
                            {
                                valueType &= ~ResultType.KeyValuePairArray;
                                if (valueType is ResultType.String)
                                {
                                    if (array[0] is object[])
                                    {
                                        var result = new KeyValuePair<string, string>[array.Length];
                                        for (uint i = 0; i < array.Length; i++)
                                        {
                                            var item = ConvertExtensions.To<KeyValuePair<string, string>>(array[i], ResultType.KeyValuePair | ResultType.String, encoding);
                                            result[i] = item;
                                        }
                                        if (result is T res) return res;
                                    }
                                    else
                                    {
                                        if (array.Length % 2 != 0)
                                        {
                                            throw new FormatException($"The data is not a valid KeyValuePair[], The actual type is {data.GetType().FullName}");
                                        }
                                        var result = new KeyValuePair<string, string>[array.Length / 2];
                                        for (uint i = 0; i < array.Length; i += 2)
                                        {
                                            var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                            var value = ConvertExtensions.To<string>(array[i + 1], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                            result[i / 2] = new KeyValuePair<string, string>(key, value);
                                        }
                                        if (result is T res) return res;
                                    }
                                }
                                else if (valueType is ResultType.Bytes)
                                {
                                    if (array[0] is object[])
                                    {
                                        var result = new KeyValuePair<string, byte[]>[array.Length];
                                        for (uint i = 0; i < array.Length; i++)
                                        {
                                            var item = ConvertExtensions.To<KeyValuePair<string, byte[]>>(array[i], ResultType.KeyValuePair | ResultType.Bytes, encoding);
                                            result[i] = item;
                                        }
                                        if (result is T res) return res;
                                    }
                                    else
                                    {
                                        if (array.Length % 2 != 0)
                                        {
                                            throw new FormatException($"The data is not a valid KeyValuePair[], The actual type is {data.GetType().FullName}");
                                        }
                                        var result = new KeyValuePair<string, byte[]>[array.Length / 2];
                                        for (uint i = 0; i < array.Length; i += 2)
                                        {
                                            var key = ConvertExtensions.To<string>(array[i], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                            var value = ConvertExtensions.To<byte[]>(array[i + 1], ResultType.Bytes, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                            result[i / 2] = new KeyValuePair<string, byte[]>(key, value);
                                        }
                                        if (result is T res) return res;
                                    }
                                }
                            }
                        }
                        else if ((valueType & ResultType.Coordinate) == ResultType.Coordinate)
                        {
                            if (data is object[] arrayObject)
                            {
                                if ((valueType & ResultType.Nullable) == ResultType.Nullable)
                                {
                                    var result = new CoordinateValue?[arrayObject.Length];
                                    for (uint i = 0; i < arrayObject.Length; i++)
                                    {
                                        result[i] = ConvertExtensions.To<CoordinateValue?>(arrayObject[i], valueType, encoding);
                                    }
                                    if (result is T arrayResult) return arrayResult;
                                }
                                else
                                {
                                    var result = new CoordinateValue[arrayObject.Length];
                                    for (uint i = 0; i < arrayObject.Length; i++)
                                    {
                                        result[i] = ConvertExtensions.To<CoordinateValue>(arrayObject[i], valueType, encoding);
                                    }
                                    if (result is T arrayResult) return arrayResult;
                                }
                            }
                        }
                        else if ((valueType & ResultType.GeoRadiusValue) == ResultType.GeoRadiusValue)
                        {
                            var memberType = valueType & ~ResultType.GeoRadiusValue;
                            if ((memberType is ResultType.String || memberType is ResultType.Bytes) && data is object[] array)
                            {
                                object[] result;
                                if (memberType is ResultType.String) result = new GeoRadiusValue<string>[array.Length];
                                else result = new GeoRadiusValue<byte[]>[array.Length];

                                for (uint i = 0; i < array.Length; i++)
                                {
                                    if (array[i] is object[] item)
                                    {
                                        var member = ConvertExtensions.To<object>(item[0], memberType, encoding);
                                        ulong? hash = null;
                                        double? dist = null;
                                        CoordinateValue? coordinate = null;
                                        for (uint z = 1; z < item.Length; z++)
                                        {
                                            if (item[z] is NumberValue nv)
                                            {
                                                if(nv.NumberType == 58)
                                                {
                                                    hash = nv.ToUInt64();
                                                    continue;
                                                }
                                                if (nv.NumberType == 44)
                                                {
                                                    dist = nv.ToDouble();
                                                    continue;
                                                }
                                            }

                                            if (item[z] is byte[] distBytes)
                                            {
                                                dist = double.Parse(encoding.GetString(distBytes));
                                                continue;
                                            }

                                            if (item[z] is object[] coordinateArray)
                                            {
                                                coordinate = ConvertExtensions.To<CoordinateValue>(item[z], ResultType.Coordinate, encoding);
                                                continue;
                                            }

                                            if (item[z] is string distString)
                                            {
                                                dist = double.Parse(distString);
                                                continue;
                                            }
                                        }

                                        if (memberType is ResultType.String && member is string memberString)
                                        {
                                            result[i] = new GeoRadiusValue<string>(memberString, dist, hash, coordinate);
                                            continue;
                                        }

                                        if (memberType is ResultType.Bytes && member is byte[] memberBytes)
                                        {
                                            result[i] = new GeoRadiusValue<byte[]>(memberBytes, dist, hash, coordinate);
                                            continue;
                                        }
                                    }

                                    if (array[i] is byte[])
                                    {
                                        var member = ConvertExtensions.To<object>(array[i], memberType, encoding);

                                        if (memberType is ResultType.String && member is string memberString)
                                        {
                                            result[i] = new GeoRadiusValue<string>(memberString, null, null, null);
                                            continue;
                                        }

                                        if (memberType is ResultType.Bytes && member is byte[] memberBytes)
                                        {
                                            result[i] = new GeoRadiusValue<byte[]>(memberBytes, null, null, null);
                                            continue;
                                        }
                                    }
                                }
                                if (result is T res) return res;
                            }
                            throw new FormatException($"The data is not a valid GeoRadiusValue, The actual type is {data.GetType().FullName}");
                        }
                        else if ((valueType & ResultType.Stream) == ResultType.Stream)
                        {
                            valueType &= ~ResultType.Stream;
                            if (data is object[] array)
                            {
                                if (array.Length is 0) return ConvertExtensions.To<T>(null, type, encoding);

                                if (valueType is ResultType.String)
                                {
                                    var result = new StreamValue<string>[array.Length];
                                    for (uint i = 0; i < array.Length; i++)
                                    {
                                        result[i] = ConvertExtensions.To<StreamValue<string>>(array[i], ResultType.Stream | ResultType.String, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                                !
#endif
                                                ;
                                    }
                                    if (result is T res) return res;
                                }
                                if (valueType is ResultType.Bytes)
                                {
                                    var result = new StreamValue<byte[]>[array.Length];
                                    for (uint i = 0; i < array.Length; i++)
                                    {
                                        result[i] = ConvertExtensions.To<StreamValue<byte[]>>(array[i], ResultType.Stream | ResultType.Bytes, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                                !
#endif
                                                ;
                                    }
                                    if (result is T res) return res;
                                }
                            }
                            throw new FormatException($"The data is not a valid StreamValue[], The actual type is {data.GetType().FullName}");
                        }
                        else if (valueType == ResultType.XInfoConsumersValue)
                        {
                            if (data is object[] array)
                            {
                                if (array.Length <= 0) return ConvertExtensions.To<T>(null, type, encoding);

                                var result = new XInfoConsumersValue[array.Length];
                                for (uint i = 0; i < array.Length; i++)
                                {
                                    result[i] = ConvertExtensions.To<XInfoConsumersValue>(array[i], ResultType.XInfoConsumersValue, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                                !
#endif
                                        ;
                                }
                                if (result is T res) return res;
                            }
                            throw new FormatException($"The data is not a valid XInfoConsumersValue[], The actual type is {data.GetType().FullName}");
                        }
                        else if (valueType == ResultType.XInfoGroupsValue)
                        {
                            if (data is object[] array)
                            {
                                if (array.Length <= 0) return ConvertExtensions.To<T>(null, type, encoding);

                                XInfoGroupsValue[] result = new XInfoGroupsValue[array.Length];
                                for (uint i = 0; i < array.Length; i++)
                                {
                                    result[i] = ConvertExtensions.To<XInfoGroupsValue>(array[i], ResultType.XInfoGroupsValue, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                                !
#endif
                                        ;
                                }
                                if (result is T res) return res;
                            }
                            throw new FormatException($"The data is not a valid XInfoGroupsValue[], The actual type is {data.GetType().FullName}");
                        }
                        else if(valueType == ResultType.FunctionInfoValue)
                        {
                            if (data is object[] array)
                            {
                                var result = new FunctionInfoValue[array.Length];
                                for (uint i = 0; i < result.Length; i++)
                                {
                                    result[i] = new FunctionInfoValue(array[i], encoding);
                                }
                                if (result is T arrayResult) return arrayResult;
                            }
                            throw new FormatException($"The data is not a valid FunctionInfoValue[], The actual type is {data.GetType().FullName}");
                        }
                        else if (data is T arrayVal) return arrayVal;
                        else
                        {
                            throw new NotSupportedException($"Unsupported array of {valueType} type");
                        }
                        if (data is T _res) return _res;
                        throw new FormatException($"The data is not a valid Array, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Lcs:
                    {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        if (data is Dictionary<string, object?> dict)
#else
                        if (data is Dictionary<string, object> dict)
#endif
                        {
                            var lcsValue = LcsValue.Parse(dict);
                            if (lcsValue is T lcsVal) return lcsVal;
                        }
                        if (data is object[])
                        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            var dic = ConvertExtensions.To<Dictionary<string, object?>>(data, ResultType.Dictionary | ResultType.Object, encoding);
#else
                            var dic = ConvertExtensions.To<Dictionary<string, object>>(data, ResultType.Dictionary | ResultType.Object, encoding);
#endif
                            if (dic != null)
                            {
                                var lcsValue = LcsValue.Parse(dic);
                                if (lcsValue is T lcsVal) return lcsVal;
                            }
                        }
                        if (data is T res) return res;
                        throw new FormatException($"The data is not a valid Lcs, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.KeyValuePair:
                    {
                        var valueType = type & ~ResultType.KeyValuePair;
                        valueType &= ~ResultType.Nullable;
                        if (valueType is ResultType.String)
                        {
                            if (data is object[] array)
                            {
                                if (array.Length is 2)
                                {
                                    var field = ConvertExtensions.To<string>(array[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var value = ConvertExtensions.To<string>(array[1], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var result = new KeyValuePair<string, string>(field, value);
                                    if (result is T res) return res;
                                }
                                else if (array.Length is 1 && array[0] is object[] itemArray && itemArray.Length == 2)
                                {
                                    var field = ConvertExtensions.To<string>(itemArray[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var value = ConvertExtensions.To<string>(itemArray[1], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var result = new KeyValuePair<string, string>(field, value);
                                    if (result is T res) return res;
                                }
                                throw new FormatException($"The data is not a valid KeyValuePair<string, string>, The actual type is {data.GetType().FullName}");
                            }
                        }
                        else if (valueType is ResultType.Bytes)
                        {
                            if (data is object[] array)
                            {
                                if (array.Length is 2)
                                {
                                    var field = ConvertExtensions.To<string>(array[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var value = ConvertExtensions.To<byte[]>(array[1], ResultType.Bytes, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var result = new KeyValuePair<string, byte[]>(field, value);
                                    if (result is T res) return res;
                                }
                                else if (array.Length is 1 && array[0] is object[] itemArray && itemArray.Length == 2)
                                {
                                    var field = ConvertExtensions.To<string>(itemArray[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var value = ConvertExtensions.To<byte[]>(itemArray[1], ResultType.Bytes, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var result = new KeyValuePair<string, byte[]>(field, value);
                                    if (result is T res) return res;
                                }
                                throw new FormatException($"The data is not a valid KeyValuePair<string, byte[]>, The actual type is {data.GetType().FullName}");
                            }
                        }
                        else if (valueType == (ResultType.Array | ResultType.String))
                        {
                            if (data is object[] array && array.Length is 2)
                            {
                                var key = ConvertExtensions.To<string>(array[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                var value = ConvertExtensions.To<string[]>(array[1], ResultType.Array | ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                var result = new KeyValuePair<string, string[]>(key, value);
                                if (result is T res) return res;
                            }
                        }
                        else if (valueType == (ResultType.Array | ResultType.Bytes))
                        {
                            if (data is object[] array && array.Length is 2)
                            {
                                var key = ConvertExtensions.To<string>(array[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                var value = ConvertExtensions.To<byte[][]>(array[1], ResultType.Array | ResultType.Bytes, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                var result = new KeyValuePair<string, byte[][]>(key, value);
                                if (result is T res) return res;
                            }
                        }
                        else if ((valueType & (ResultType.Array | ResultType.MemberScoreValue)) == (ResultType.Array | ResultType.MemberScoreValue))
                        {
                            if (data is object[] array && array.Length is 2)
                            {
                                var key = ConvertExtensions.To<string>(array[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                if ((valueType & ResultType.String) == ResultType.String)
                                {
                                    var value = ConvertExtensions.To<MemberScoreValue<string>[]>(array[1], valueType, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var result = new KeyValuePair<string, MemberScoreValue<string>[]>(key, value);
                                    if (result is T res) return res;
                                }
                                if ((valueType & ResultType.Bytes) == ResultType.Bytes)
                                {
                                    var value = ConvertExtensions.To<MemberScoreValue<byte[]>[]>(array[1], valueType, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                    var result = new KeyValuePair<string, MemberScoreValue<byte[]>[]>(key, value);
                                    if (result is T res) return res;
                                }
                            }
                        }
                        else if (valueType == (ResultType.MemberScoreValue | ResultType.String))
                        {
                            if (data is object[] array)
                            {
                                var key = ConvertExtensions.To<string>(array[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                if (array.Length is 2)
                                {
                                    var value = ConvertExtensions.To<MemberScoreValue<string>>(array[1], valueType, encoding);
                                    var result = new KeyValuePair<string, MemberScoreValue<string>>(key, value);
                                    if (result is T res) return res;
                                }
                                else if (array.Length is 3)
                                {
#if NET8_0_OR_GREATER
                                    var value = ConvertExtensions.To<MemberScoreValue<string>>(array[1..], valueType, encoding);
#else
                                    var value = ConvertExtensions.To<MemberScoreValue<string>>(new object[] { array[1], array[2] }, valueType, encoding);
#endif
                                    var result = new KeyValuePair<string, MemberScoreValue<string>>(key, value);
                                    if (result is T res) return res;
                                }
                            }
                        }
                        else if (valueType == (ResultType.MemberScoreValue | ResultType.Bytes))
                        {
                            if (data is object[] array)
                            {
                                var key = ConvertExtensions.To<string>(array[0], ResultType.String, encoding) ?? throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                                if (array.Length is 2)
                                {
                                    var value = ConvertExtensions.To<MemberScoreValue<byte[]>>(array[1], valueType, encoding);
                                    var result = new KeyValuePair<string, MemberScoreValue<byte[]>>(key, value);
                                    if (result is T res) return res;
                                }
                                else if (array.Length is 3)
                                {
#if NET8_0_OR_GREATER
                                    var value = ConvertExtensions.To<MemberScoreValue<byte[]>>(array[1..], valueType, encoding);
#else
                                    var value = ConvertExtensions.To<MemberScoreValue<byte[]>>(new object[] { array[1], array[2] }, valueType, encoding);
#endif
                                    var result = new KeyValuePair<string, MemberScoreValue<byte[]>>(key, value);
                                    if (result is T res) return res;
                                }
                            }
                        }
                        else if (data is T _result) return _result;

                        if (data is T _res) return _res;
                        throw new FormatException($"The data is not a valid KeyValuePair, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.MemberScoreValue:
                    {
                        if ((type & ResultType.String) == ResultType.String)
                        {
                            var result = new MemberScoreValue<string>(data, encoding, ResultType.String);
                            if (result is T res) return res;
                            throw new FormatException($"The data is not a valid MemberScoreValue, The actual type is {data.GetType().FullName}");
                        }
                        else if ((type & ResultType.Bytes) == ResultType.Bytes)
                        {
                            var result = new MemberScoreValue<byte[]>(data, encoding, ResultType.Bytes);
                            if (result is T res) return res;
                            throw new FormatException($"The data is not a valid MemberScoreValue, The actual type is {data.GetType().FullName}");
                        }
                        
                        if (data is T _res) return _res;
                        throw new FormatException($"The data is not a valid MemberScoreValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.ScoreRankValue:
                    {
                        if (data is object[] array)
                        {
                            if (array[0] is object[] itemArray && itemArray.Length == 2)
                            {
                                array = itemArray;
                            }

                            if (array.Length == 2)
                            {
                                var rank = ConvertExtensions.To<long>(array[0], ResultType.Int64, encoding);
                                var score = ConvertExtensions.To<NumberValue>(array[1], ResultType.Number, encoding)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                                !
#endif
                                ;
                                var sr = new ScoreRankValue(rank, score);
                                if (sr is T res) return res;
                            }
                        }

                        if (data is T _result) return _result;
                        throw new FormatException($"The data is not a valid ScoreRankValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Enum:
                    {
                        int status;
                        if (data is NumberValue nv)
                        {
                            status = nv;
                        }
                        else if (data is object[] objArray && objArray.Length is 1 && objArray[0] is NumberValue nvs)
                        {
                            status = nvs;
                        }
                        else if (data is T _result) return _result;
                        else
                        {
                            throw new FormatException($"Value {data} is not defined in enum {data.GetType().FullName}");
                        }
#if NET5_0_OR_GREATER
                        if (typeof(T).FullName == "System.Object")
                        {
                            if (status is T res) return res;
                            throw new FormatException($"Value {data} is not defined in enum {data.GetType().FullName}");
                        }
                        return Unsafe.As<int, T>(ref status);
#else
                        if (status is T res) return res;
                        throw new FormatException($"Value {data} is not defined in enum {data.GetType().FullName}");
#endif
                    }
                case ResultType.Scan:
                    {
                        if (data is object[] objArray
                            && objArray.Length is 2)
                        {
                            var cursor = ConvertExtensions.To<ulong>(objArray[0], ResultType.UInt64, encoding);
                            var valueType = type & ~ResultType.Scan;
                            if (valueType == (ResultType.Dictionary | ResultType.String))
                            {
                                var value = ConvertExtensions.To<Dictionary<string, string>>(objArray[1], valueType, encoding);
                                var result = new ScanValue<Dictionary<string, string>>(cursor, value);
                                if (result is T res) return res;
                            }
                            if (valueType == (ResultType.Dictionary | ResultType.Bytes))
                            {
                                var value = ConvertExtensions.To<Dictionary<string, byte[]>>(objArray[1], valueType, encoding);
                                var result = new ScanValue<Dictionary<string, byte[]>>(cursor, value);
                                if (result is T res) return res;
                            }
                            if (valueType == (ResultType.Array | ResultType.String))
                            {
                                var value = ConvertExtensions.To<string[]>(objArray[1], valueType, encoding);
                                var result = new ScanValue<string[]>(cursor, value);
                                if (result is T res) return res;
                            }
                            if (valueType == (ResultType.Array | ResultType.Bytes))
                            {
                                var value = ConvertExtensions.To<byte[][]>(objArray[1], valueType, encoding);
                                var result = new ScanValue<byte[][]>(cursor, value);
                                if (result is T res) return res;
                            }
                            if (valueType == (ResultType.Array | ResultType.MemberScoreValue | ResultType.String))
                            {
                                var value = ConvertExtensions.To<MemberScoreValue<string>[]>(objArray[1], valueType, encoding);
                                var result = new ScanValue<MemberScoreValue<string>[]>(cursor, value);
                                if (result is T res) return res;
                            }
                            if (valueType == (ResultType.Array | ResultType.MemberScoreValue | ResultType.Bytes))
                            {
                                var value = ConvertExtensions.To<MemberScoreValue<byte[]>[]>(objArray[1], valueType, encoding);
                                var result = new ScanValue<MemberScoreValue<byte[]>[]>(cursor, value);
                                if (result is T res) return res;
                            }
                        }

                        if (data is T _result) return _result;
                        throw new FormatException($"The data is not a valid ScanValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Bytes:
                    {
                        if (data is T result) return result;
                        if (data is object[] array && array.Length == 1 && array[0] is T res) return res;
                        throw new NotSupportedException($"byte[] is not supported. The current type is {data.GetType().FullName}");
                    }
                case ResultType.XAutoClaimValue:
                    {
                        var valueType = type & ~ResultType.XAutoClaimValue;
                        if (data is object[] array && array.Length >= 2)
                        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                            var nextId = ConvertExtensions.To<string>(array[0], ResultType.String, encoding)!;
                            string[]? deleted = null;
#else
                            var nextId = ConvertExtensions.To<string>(array[0], ResultType.String, encoding);
                            string[] deleted = null;
#endif
                            if (valueType is ResultType.String)
                            {
                                var message = ConvertExtensions.To<StreamValue<string>[]>(array[1], ResultType.Array | ResultType.Stream | ResultType.String, encoding);
                                if (array.Length is 3)
                                {
                                    deleted = ConvertExtensions.To<string[]>(array[2], ResultType.Array | ResultType.String, encoding);
                                }
                                var result = new XAutoClaimValue<string>(nextId, message, deleted);
                                if (result is T res) return res;
                            }
                            if (valueType is ResultType.Bytes)
                            {
                                var message = ConvertExtensions.To<StreamValue<byte[]>[]>(array[1], ResultType.Array | ResultType.Stream | ResultType.Bytes, encoding);
                                if (array.Length is 3)
                                {
                                    deleted = ConvertExtensions.To<string[]>(array[2], ResultType.Array | ResultType.String, encoding);
                                }
                                var result = new XAutoClaimValue<byte[]>(nextId, message, deleted);
                                if (result is T res) return res;
                            }
                        }
                        throw new FormatException($"The data is not a valid XAutoClaimValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Stream:
                    {
                        var valueType = type & ~ResultType.Stream;
                        object result;
                        if (valueType is ResultType.String)
                        {
                            result = new StreamValue<string>(data, encoding, ResultType.String);
                        }
                        else if (valueType is ResultType.Bytes)
                        {
                            result = new StreamValue<byte[]>(data, encoding, ResultType.Bytes);
                        }
                        else
                        {
                            throw new FormatException($"The data is not a valid StreamValue, The actual type is {data.GetType().FullName}");
                        }
                        if (result is T res) return res;
                        throw new FormatException($"The data is not a valid StreamValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.XInfoConsumersValue:
                    {
                        var result = new XInfoConsumersValue(data, encoding);
                        if (result is T res) return res;
                        throw new FormatException($"The data is not a valid XInfoConsumersValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.XInfoGroupsValue:
                    {
                        var result = new XInfoGroupsValue(data, encoding);
                        if (result is T res) return res;
                        throw new FormatException($"The data is not a valid XInfoGroupsValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.XInfoStreamValue:
                    {
                        var valueType = type & ~ResultType.XInfoStreamValue;
                        object result;
                        if (valueType is ResultType.String)
                        {
                            result = new XInfoStreamValue<string>(data, encoding, ResultType.String);
                        }
                        else if (valueType is ResultType.Bytes)
                        {
                            result = new XInfoStreamValue<byte[]>(data, encoding, ResultType.Bytes);
                        }
                        else
                        {
                            throw new FormatException($"The data is not a valid XInfoStreamValue, The actual type is {data.GetType().FullName}");
                        }
                        if (result is T res) return res;
                        throw new FormatException($"The data is not a valid XInfoStreamValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.XInfoStreamFullValue:
                    {
                        var valueType = type & ~ResultType.XInfoStreamFullValue;
                        object result;
                        if (valueType is ResultType.String)
                        {
                            result = new XInfoStreamFullValue<string>(data, encoding, ResultType.String);
                        }
                        else if (valueType is ResultType.Bytes)
                        {
                            result = new XInfoStreamFullValue<byte[]>(data, encoding, ResultType.Bytes);
                        }
                        else
                        {
                            throw new FormatException($"The data is not a valid XInfoStreamFullValue, The actual type is {data.GetType().FullName}");
                        }
                        if (result is T res) return res;
                        throw new FormatException($"The data is not a valid XInfoStreamFullValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.FunctionStatsValue:
                    {
                        var result = new FunctionStatsValue(data, encoding);
                        if (result is T res) return res;
                        throw new FormatException($"The data is not a valid FunctionStatsValue, The actual type is {data.GetType().FullName}");
                    }
                case ResultType.Object:
                default:
                    {
                        if (data is T tdata) return tdata;
                        return (T)data;
                    }
            }
        }
    }
}
