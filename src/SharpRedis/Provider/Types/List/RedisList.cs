#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable IDE0063
#endif
#if !LOW_NET
using System.Threading;
#else
#pragma warning disable IDE0130
#endif
using SharpRedis.Provider.Standard;
using System;
using System.Collections.Generic;
using SharpRedis.Commands;

namespace SharpRedis
{
    /// <summary>
    /// Redis list type
    /// <para>集合</para>
    /// </summary>
    public sealed partial class RedisList : BaseType
    {
        internal RedisList(BaseCall call) : base(call)
        {
        }

        /// <summary>
        /// Returns the element at index index in the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>获得List中指定索引处的元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List Key</param>
        /// <param name="index">The index is zero-based, so 0 means the first element, 1 the second element and so on.
        /// <para>Negative indices can be used to designate elements starting at the tail of the list. Here, -1 means the last element, -2 means the penultimate and so forth.</para>
        /// <para>索引从0开始，因此0表示第一个元素, 1表示第二个元素，依此类推</para>
        /// <para>负数索引可用于指定从列表尾部开始的元素. -1表示最后一个元素, -2表示倒数第二个元素，依此类推</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? LIndex(string key, long index, CancellationToken cancellationToken = default)
#else
        public string LIndex(string key, long index, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(ListCommands.LIndex(key, index), cancellationToken);
        }

        /// <summary>
        /// Returns the element at index index in the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>获得List中指定索引处的元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List Key</param>
        /// <param name="index">The index is zero-based, so 0 means the first element, 1 the second element and so on.
        /// <para>Negative indices can be used to designate elements starting at the tail of the list. Here, -1 means the last element, -2 means the penultimate and so forth.</para>
        /// <para>索引从0开始，因此0表示第一个元素, 1表示第二个元素，依此类推</para>
        /// <para>负数索引可用于指定从列表尾部开始的元素. -1表示最后一个元素, -2表示倒数第二个元素，依此类推</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? LIndexBytes(string key, long index, CancellationToken cancellationToken = default)
#else
        public byte[] LIndexBytes(string key, long index, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(ListCommands.LIndex(key, index), cancellationToken);
        }

        /// <summary>
        /// Inserts element in the list stored at key either before or after the reference value pivot.
        /// <para>Available since: 2.2.0</para>
        /// <para>根据指定的元素(pivot), 将一个新元素插入到其后面或前面</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的新元素</para>
        /// </param>
        /// <param name="ba">Before or after</param>
        /// <param name="pivot">pivot
        /// <para>指定的条件元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The list length after a successful insert operation
        /// <para>0 when the key doesn't exist.</para>
        /// <para>-1 when the pivot wasn't found.</para>
        /// <para>如果插入成功, 返回List的长度</para>
        /// <para>如果Key不存在, 返回0</para>
        /// <para>如果指定的pivot不存在, 返回-1</para>
        /// </returns>
        public long LInsert(string key, string element, string pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LInsert(key, element, pivot, ba), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Inserts element in the list stored at key either before or after the reference value pivot.
        /// <para>Available since: 2.2.0</para>
        /// <para>根据指定的元素(pivot), 将一个新元素插入到其后面或前面</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的新元素</para>
        /// </param>
        /// <param name="ba">Before or after</param>
        /// <param name="pivot">pivot
        /// <para>指定的条件元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The list length after a successful insert operation
        /// <para>0 when the key doesn't exist.</para>
        /// <para>-1 when the pivot wasn't found.</para>
        /// <para>如果插入成功, 返回List的长度</para>
        /// <para>如果Key不存在, 返回0</para>
        /// <para>如果指定的pivot不存在, 返回-1</para>
        /// </returns>
        public long LInsert(string key, byte[] element, string pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LInsert(key, element, pivot, ba), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Inserts element in the list stored at key either before or after the reference value pivot.
        /// <para>Available since: 2.2.0</para>
        /// <para>根据指定的元素(pivot), 将一个新元素插入到其后面或前面</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的新元素</para>
        /// </param>
        /// <param name="ba">Before or after</param>
        /// <param name="pivot">pivot
        /// <para>指定的条件元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The list length after a successful insert operation
        /// <para>0 when the key doesn't exist.</para>
        /// <para>-1 when the pivot wasn't found.</para>
        /// <para>如果插入成功, 返回List的长度</para>
        /// <para>如果Key不存在, 返回0</para>
        /// <para>如果指定的pivot不存在, 返回-1</para>
        /// </returns>
        public long LInsert(string key, byte[] element, byte[] pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LInsert(key, element, pivot, ba), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Inserts element in the list stored at key either before or after the reference value pivot.
        /// <para>Available since: 2.2.0</para>
        /// <para>根据指定的元素(pivot), 将一个新元素插入到其后面或前面</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的新元素</para>
        /// </param>
        /// <param name="ba">Before or after</param>
        /// <param name="pivot">pivot
        /// <para>指定的条件元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The list length after a successful insert operation
        /// <para>0 when the key doesn't exist.</para>
        /// <para>-1 when the pivot wasn't found.</para>
        /// <para>如果插入成功, 返回List的长度</para>
        /// <para>如果Key不存在, 返回0</para>
        /// <para>如果指定的pivot不存在, 返回-1</para>
        /// </returns>
        public long LInsert(string key, string element, byte[] pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LInsert(key, element, pivot, ba), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the length of the list stored at key.
        /// <para>Available since: 1.0.0</para>
        /// <para>获得一个List的长度(元素个数)</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list. If key does not exist, it is interpreted as an empty list and 0 is returned
        /// <para>List的长度(List元素个数). 如果Key不存在, 返回0</para>
        /// </returns>
        public long LLen(string key, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LLen(key), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert the value at the head of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.
        /// <para>Available since: 1.0.0</para>
        /// <para>将一个元素值插入List头部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入之后List的长度(元素个数)</para>
        /// </returns>
        public long LPush(string key, string element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPush(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert the value at the head of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.
        /// <para>Available since: 1.0.0</para>
        /// <para>将一个元素值插入List头部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入之后List的长度(元素个数)</para>
        /// </returns>
        public long LPush(string key, byte[] element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPush<byte[]>(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert all the specified values at the head of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.
        /// <para>Available since: 2.4.0</para>
        /// <para>将多个元素值插入List头部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入之后List的长度(元素个数)</para>
        /// </returns>
        public long LPush(string key, string[] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPush(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert all the specified values at the head of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.
        /// <para>Available since: 2.4.0</para>
        /// <para>将多个元素值插入List头部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入之后List的长度(元素个数)</para>
        /// </returns>
        public long LPush(string key, byte[][] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPush(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert value at the head of the list stored at key, only if key already exists and holds a list. In contrary to LPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 2.2.0</para>
        /// <para>仅在List key存在时, 再将一个元素值插入List头部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long LPushX(string key, string element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPushX(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert value at the head of the list stored at key, only if key already exists and holds a list. In contrary to LPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 2.2.0</para>
        /// <para>仅在List key存在时, 再将一个元素值插入List头部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long LPushX(string key, byte[] element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPushX<byte[]>(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Inserts specified values at the head of the list stored at key, only if key already exists and holds a list. In contrary to LPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 4.0.0</para>
        /// <para>仅在List key存在时, 再将多个元素值插入List头部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long LPushX(string key, string[] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPushX(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Inserts specified values at the head of the list stored at key, only if key already exists and holds a list. In contrary to LPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 4.0.0</para>
        /// <para>仅在List key存在时, 再将多个元素值插入List头部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long LPushX(string key, byte[][] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LPushX(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert the value at the tail of the list stored at key. If key does not exist, it is created as empty list before performing the push operation
        /// <para>Available since: 1.0.0</para>
        /// <para>将一个元素值插入List尾部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPush(string key, string element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPush(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert the value at the tail of the list stored at key. If key does not exist, it is created as empty list before performing the push operation
        /// <para>Available since: 1.0.0</para>
        /// <para>将一个元素值插入List尾部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPush(string key, byte[] element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPush<byte[]>(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key. If key does not exist, it is created as empty list before performing the push operation
        /// <para>Available since: 2.4.0</para>
        /// <para>将多个元素值插入List尾部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPush(string key, string[] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPush(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key. If key does not exist, it is created as empty list before performing the push operation
        /// <para>Available since: 2.4.0</para>
        /// <para>将多个元素值插入List尾部, 如果Key不存在则会新建一个空List再执行插入</para>
        /// <para>支持此命令的Redis版本, 2.4.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPush(string key, byte[][] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPush(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert specified value at the tail of the list stored at key, only if key already exists and holds a list. In contrary to RPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 2.2.0</para>
        /// <para>仅在List key存在时, 再将一个元素值插入List尾部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPushX(string key, string element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPushX(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Insert specified value at the tail of the list stored at key, only if key already exists and holds a list. In contrary to RPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 2.2.0</para>
        /// <para>仅在List key存在时, 再将一个元素值插入List尾部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要插入的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPushX(string key, byte[] element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPushX<byte[]>(key, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Inserts specified values at the tail of the list stored at key, only if key already exists and holds a list. In contrary to RPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 4.0.0</para>
        /// <para>仅在List key存在时, 再将多个元素值插入List尾部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPushX(string key, string[] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPushX(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Inserts specified values at the tail of the list stored at key, only if key already exists and holds a list. In contrary to RPUSH, no operation will be performed when key does not yet exist.
        /// <para>Available since: 4.0.0</para>
        /// <para>仅在List key存在时, 再将多个元素值插入List尾部, 如果Key不存在则不执行任何操作</para>
        /// <para>支持此命令的Redis版本, 4.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="elements">elements
        /// <para>要插入的元素数组</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The length of the list after the push operation
        /// <para>插入成功之后的List长度(元素个数)</para>
        /// </returns>
        public long RPushX(string key, byte[][] elements, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.RPushX(key, elements), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Atomically returns and removes the first/last element (head/tail depending on the wherefrom argument) of the list stored at source, and pushes the element at the first/last element (head/tail depending on the whereto argument) of the list stored at destination.
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定Key的头部或尾部移除并获得一个元素, 并从头部或尾部插入到目标Key中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="whereFrom">where from
        /// <para>指定从源Key的头部还是尾部移除</para>
        /// </param>
        /// <param name="whereTo">where to
        /// <para>指定从头部还是尾部插入到目标Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>移除并插入的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? LMove(string source, string destination, LeftRight whereFrom, LeftRight whereTo, CancellationToken cancellationToken = default)
#else
        public string LMove(string source, string destination, LeftRight whereFrom, LeftRight whereTo, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(ListCommands.LMove(source, destination, whereFrom, whereTo), cancellationToken);
        }

        /// <summary>
        /// Atomically returns and removes the first/last element (head/tail depending on the wherefrom argument) of the list stored at source, and pushes the element at the first/last element (head/tail depending on the whereto argument) of the list stored at destination.
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定Key的头部或尾部移除并获得一个元素, 并从头部或尾部插入到目标Key中</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="whereFrom">where from
        /// <para>指定从源Key的头部还是尾部移除</para>
        /// </param>
        /// <param name="whereTo">where to
        /// <para>指定从头部还是尾部插入到目标Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>移除并插入的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? LMoveBytes(string source, string destination, LeftRight whereFrom, LeftRight whereTo, CancellationToken cancellationToken = default)
#else
        public byte[] LMoveBytes(string source, string destination, LeftRight whereFrom, LeftRight whereTo, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(ListCommands.LMove(source, destination, whereFrom, whereTo), cancellationToken);
        }

        /// <summary>
        /// Removes and returns the first elements of the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>删除并返回指定List的第一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? LPop(string key, CancellationToken cancellationToken = default)
#else
        public string LPop(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(ListCommands.LPop(key, 1), cancellationToken);
        }

        /// <summary>
        /// Removes and returns the first elements of the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>删除并返回指定List的第一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? LPopBytes(string key, CancellationToken cancellationToken = default)
#else
        public byte[] LPopBytes(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(ListCommands.LPop(key, 1), cancellationToken);
        }

        /// <summary>
        /// Removes and returns the specified number of elements from the start
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定List的开始位置删除并返回指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="count">Removes count
        /// <para>要删除的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A list of popped elements
        /// <para>删除的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? LPop(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public string[] LPop(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(ListCommands.LPop(key, count), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Removes and returns the specified number of elements from the start
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定List的开始位置删除并返回指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="count">Removes count
        /// <para>要删除的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A list of popped elements
        /// <para>删除的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? LPopBytes(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public byte[][] LPopBytes(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(ListCommands.LPop(key, count), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            !
#endif
            ;
        }

        /// <summary>
        /// Returns the index of the first matching element
        /// <para>Available since: 6.0.6</para>
        /// <para>返回指定元素所在指定List中第一次出现的下标</para>
        /// <para>支持此命令的Redis版本, 6.0.6+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要查找的元素</para>
        /// </param>
        /// <param name="rank">The RANK option specifies the "rank" of the first element to return, in case there are multiple matches. A rank of 1 means to return the first match, 2 to return the second match, and so forth
        /// <para>RANK参数可设置返回第几个匹配的元素下标, 1标识返回第一个匹配的下标, 2表示返回第二个匹配的下标, 以此类推. 支持负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="maxlen">The MAXLEN option tells the command to compare the provided element only with a given maximum number of list items
        /// <para>最大查找次数, 0无限制</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>An integer representing the matching element
        /// <para>元素所在下标</para>
        /// </returns>
        public long? LPos(string key, string element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullNumber<long>(ListCommands.LPos(key, element, rank, null, maxlen), ResultType.Nullable | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the index of the first matching element
        /// <para>Available since: 6.0.6</para>
        /// <para>返回指定元素所在指定List中第一次出现的下标</para>
        /// <para>支持此命令的Redis版本, 6.0.6+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要查找的元素</para>
        /// </param>
        /// <param name="rank">The RANK option specifies the "rank" of the first element to return, in case there are multiple matches. A rank of 1 means to return the first match, 2 to return the second match, and so forth
        /// <para>RANK参数可设置返回第几个匹配的元素下标, 1标识返回第一个匹配的下标, 2表示返回第二个匹配的下标, 以此类推. 支持负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="maxlen">The MAXLEN option tells the command to compare the provided element only with a given maximum number of list items
        /// <para>最大查找次数, 0无限制</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>An integer representing the matching element
        /// <para>元素所在下标</para>
        /// </returns>
        public long? LPos(string key, byte[] element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullNumber<long>(ListCommands.LPos(key, element, rank, null, maxlen), ResultType.Nullable | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the index of the matching element
        /// <para>Available since: 6.0.6</para>
        /// <para>返回指定元素所在指定List中出现的下标</para>
        /// <para>支持此命令的Redis版本, 6.0.6+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要查找的元素</para>
        /// </param>
        /// <param name="count">Sometimes we want to return not just the Nth matching element, but the position of all the first N matching elements. This can be achieved using the COUNT option.
        /// <para>When COUNT is used, it is possible to specify 0 as the number of matches, as a way to tell the command we want all the matches found returned as an array of indexes. This is better than giving a very large COUNT option because it is more general.</para>
        /// <para>要返回匹配的个数, 0表示返回所有匹配的下标</para>
        /// </param>
        /// <param name="rank">The RANK option specifies the "rank" of the first element to return, in case there are multiple matches. A rank of 1 means to return the first match, 2 to return the second match, and so forth
        /// <para>RANK参数可设置返回第几个匹配的元素下标, 1标识返回第一个匹配的下标, 2表示返回第二个匹配的下标, 以此类推. 支持负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="maxlen">The MAXLEN option tells the command to compare the provided element only with a given maximum number of list items
        /// <para>最大查找次数, 0无限制</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>An array of integers representing the matching elements (or an empty array if there are no matches)
        /// <para>匹配的下标数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[]? LPos(ulong count, string key, string element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
#else
        public long[] LPos(ulong count, string key, string element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<long>(ListCommands.LPos(key, element, rank, count, maxlen), ResultType.Array | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the index of the matching element
        /// <para>Available since: 6.0.6</para>
        /// <para>返回指定元素所在指定List中出现的下标</para>
        /// <para>支持此命令的Redis版本, 6.0.6+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="element">element
        /// <para>要查找的元素</para>
        /// </param>
        /// <param name="count">Sometimes we want to return not just the Nth matching element, but the position of all the first N matching elements. This can be achieved using the COUNT option.
        /// <para>When COUNT is used, it is possible to specify 0 as the number of matches, as a way to tell the command we want all the matches found returned as an array of indexes. This is better than giving a very large COUNT option because it is more general.</para>
        /// <para>要返回匹配的个数, 0表示返回所有匹配的下标</para>
        /// </param>
        /// <param name="rank">The RANK option specifies the "rank" of the first element to return, in case there are multiple matches. A rank of 1 means to return the first match, 2 to return the second match, and so forth
        /// <para>RANK参数可设置返回第几个匹配的元素下标, 1标识返回第一个匹配的下标, 2表示返回第二个匹配的下标, 以此类推. 支持负数, -1表示倒数第一个, 以此类推</para>
        /// </param>
        /// <param name="maxlen">The MAXLEN option tells the command to compare the provided element only with a given maximum number of list items
        /// <para>最大查找次数, 0无限制</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>An array of integers representing the matching elements (or an empty array if there are no matches)
        /// <para>匹配的下标数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public long[]? LPos(ulong count, string key, byte[] element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
#else
        public long[] LPos(ulong count, string key, byte[] element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallStructArray<long>(ListCommands.LPos(key, element, rank, count, maxlen), ResultType.Array | ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Returns the specified elements of the list stored at key. The offsets start and stop are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on
        /// <para>These offsets can also be negative numbers indicating offsets starting at the end of the list. For example, -1 is the last element of the list, -2 the penultimate, and so on.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>根据开始和结束下标获得指定List中的多个元素. 下标从0开始, 0表示第一个元素</para>
        /// <para>下标可以是负数, -1表示最后一个元素, -2表示倒数第二个元素, 以此类推</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="start">start index
        /// <para>开始下标</para>
        /// </param>
        /// <param name="stop">stop index
        /// <para>结束下标</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A list of elements in the specified range
        /// <para>指定范围内的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? LRange(string key, long start, long stop, CancellationToken cancellationToken = default)
#else
        public string[] LRange(string key, long start, long stop, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(ListCommands.LRange(key, start, stop), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Returns the specified elements of the list stored at key. The offsets start and stop are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on
        /// <para>These offsets can also be negative numbers indicating offsets starting at the end of the list. For example, -1 is the last element of the list, -2 the penultimate, and so on.</para>
        /// <para>Available since: 1.0.0</para>
        /// <para>根据开始和结束下标获得指定List中的多个元素. 下标从0开始, 0表示第一个元素</para>
        /// <para>下标可以是负数, -1表示最后一个元素, -2表示倒数第二个元素, 以此类推</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="start">start index
        /// <para>开始下标</para>
        /// </param>
        /// <param name="stop">stop index
        /// <para>结束下标</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A list of elements in the specified range
        /// <para>指定范围内的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? LRangeBytes(string key, long start, long stop, CancellationToken cancellationToken = default)
#else
        public byte[][] LRangeBytes(string key, long start, long stop, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(ListCommands.LRange(key, start, stop), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Removes the first count occurrences of elements equal to element from the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>从List中删除指定个数的匹配元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="count">Greater than 0, Remove elements equal to element moving from head to tail
        /// <para>Less than 0, Remove elements equal to element moving from tail to head</para>
        /// <para>Equal to 0, Remove all elements equal to element</para>
        /// <para>大于0, 删除从头部开始count个匹配的元素</para>
        /// <para>小于0, 删除从尾部开始count个匹配的元素</para>
        /// <para>等于0, 删除所有匹配的元素</para>
        /// </param>
        /// <param name="element">element
        /// <para>要删除的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of removed elements
        /// <para>删除的元素数量</para>
        /// </returns>
        public long LRem(string key, long count, string element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LRem(key, count, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Removes the first count occurrences of elements equal to element from the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>从List中删除指定个数的匹配元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="count">Greater than 0, Remove elements equal to element moving from head to tail
        /// <para>Less than 0, Remove elements equal to element moving from tail to head</para>
        /// <para>Equal to 0, Remove all elements equal to element</para>
        /// <para>大于0, 删除从头部开始count个匹配的元素</para>
        /// <para>小于0, 删除从尾部开始count个匹配的元素</para>
        /// <para>等于0, 删除所有匹配的元素</para>
        /// </param>
        /// <param name="element">element
        /// <para>要删除的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The number of removed elements
        /// <para>删除的元素数量</para>
        /// </returns>
        public long LRem(string key, long count, byte[] element, CancellationToken cancellationToken = default)
        {
            return base._call.CallNumber<long>(ListCommands.LRem(key, count, element), ResultType.Int64, cancellationToken);
        }

        /// <summary>
        /// Sets the list element at index to element
        /// <para>Available since: 1.0.0</para>
        /// <para>设置List中指定下标处的元素值, 会将指定下标处的元素进行覆盖</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="index">index
        /// <para>要设置的下标</para>
        /// </param>
        /// <param name="element">element
        /// <para>要设置的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool LSet(string key, long index, string element, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(ListCommands.LSet(key, index, element), "OK", cancellationToken);
        }

        /// <summary>
        /// Sets the list element at index to element
        /// <para>Available since: 1.0.0</para>
        /// <para>设置List中指定下标处的元素值, 会将指定下标处的元素进行覆盖</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="index">index
        /// <para>要设置的下标</para>
        /// </param>
        /// <param name="element">element
        /// <para>要设置的元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool LSet(string key, long index, byte[] element, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(ListCommands.LSet(key, index, element), "OK", cancellationToken);
        }

        /// <summary>
        /// Trim an existing list so that it will contain only the specified range of elements specified. Both start and stop are zero-based indexes, where 0 is the first element of the list (the head), 1 the next element and so on
        /// <para>Available since: 1.0.0</para>
        /// <para>根据开始和结束下标修剪一个List. 只会保留在下标范围内的元素, 其它元素都会删除</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="start">start index
        /// <para>开始下标, 0表示第一个元素, -1表示倒数第一个元素</para>
        /// </param>
        /// <param name="stop">stop index
        /// <para>结束下标, 0表示第一个元素, -1表示倒数第一个元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
        public bool LTrim(string key, long start, long stop, CancellationToken cancellationToken = default)
        {
            return base._call.CallCondition(ListCommands.LTrim(key, start, stop), "OK", cancellationToken);
        }

        /// <summary>
        /// Removes and returns the last elements of the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>删除并返回指定List Key的最后一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of the last element
        /// <para>最后一个元素值</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? RPop(string key, CancellationToken cancellationToken = default)
#else
        public string RPop(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(ListCommands.RPop(key, 1), cancellationToken);
        }

        /// <summary>
        /// Removes and returns the last elements of the list stored at key
        /// <para>Available since: 1.0.0</para>
        /// <para>删除并返回指定List Key的最后一个元素</para>
        /// <para>支持此命令的Redis版本, 1.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The value of the last element
        /// <para>最后一个元素值</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? RPopBytes(string key, CancellationToken cancellationToken = default)
#else
        public byte[] RPopBytes(string key, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(ListCommands.RPop(key, 1), cancellationToken);
        }

        /// <summary>
        /// Removes and returns the specified number of elements from the tail
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定List的尾部删除并返回指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="count">Removes count
        /// <para>要删除的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A list of popped elements
        /// <para>删除的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string[]? RPop(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public string[] RPop(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<string>(ListCommands.RPop(key, count), ResultType.Array | ResultType.String, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Removes and returns the specified number of elements from the tail
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定List的尾部删除并返回指定数量的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="count">Removes count
        /// <para>要删除的个数</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>A list of popped elements
        /// <para>删除的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[][]? RPopBytes(string key, ulong count, CancellationToken cancellationToken = default)
#else
        public byte[][] RPopBytes(string key, ulong count, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallClassArray<byte[]>(ListCommands.RPop(key, count), ResultType.Array | ResultType.Bytes, cancellationToken)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                !
#endif
                ;
        }

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at source, and pushes the element at the first element (head) of the list stored at destination
        /// <para>Available since: 1.2.0</para>
        /// <para>从源List的尾部删除一个元素, 并从目标List的头部插入</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>移除并插入的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? RPopLPush(string source, string destination, CancellationToken cancellationToken = default)
#else
        public string RPopLPush(string source, string destination, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallString(ListCommands.RPopLPush(source, destination), cancellationToken);
        }

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at source, and pushes the element at the first element (head) of the list stored at destination
        /// <para>Available since: 1.2.0</para>
        /// <para>从源List的尾部删除一个元素, 并从目标List的头部插入</para>
        /// <para>支持此命令的Redis版本, 1.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>移除并插入的元素</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? RPopLPushBytes(string source, string destination, CancellationToken cancellationToken = default)
#else
        public byte[] RPopLPushBytes(string source, string destination, CancellationToken cancellationToken = default)
#endif
        {
            return base._call.CallBytes(ListCommands.RPopLPush(source, destination), cancellationToken);
        }

        /// <summary>
        /// Pops one element from the first non-empty list key from the list of provided key names
        /// <para>Available since: 7.0.0</para>
        /// <para>指定多个List, 从第一个非空的List头或尾部弹出1个元素</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">>List keys</param>
        /// <param name="lr">Specifies whether the element is ejected from the beginning or the tail. By default, the element is ejected from the head
        /// <para>指定从头还是尾部弹出元素, 默认从头部弹出元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: Value
        /// <para>Key: 从哪个Key中弹出了元素, Value: 弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? LMPop(string[] keys, LeftRight lr = LeftRight.Left, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.LMPop(keys, lr, 1), ResultType.KeyValuePair | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Pops one or more elements from the first non-empty list key from the list of provided key names
        /// <para>Available since: 7.0.0</para>
        /// <para>指定多个List, 从第一个非空的List头或尾部弹出N个元素</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">>List keys</param>
        /// <param name="count">Number of pop-up elements
        /// <para>弹出元素数量</para>
        /// </param>
        /// <param name="lr">Specifies whether the element is ejected from the beginning or the tail. By default, the element is ejected from the head
        /// <para>指定从头还是尾部弹出元素, 默认从头部弹出元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: Values
        /// <para>Key: 从哪个Key中弹出了元素, Value: 弹出的元素数组</para>
        /// </returns>
        public KeyValuePair<string, string[]>? LMPop(string[] keys, ulong count, LeftRight lr = LeftRight.Left, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, string[]>>(ListCommands.LMPop(keys, lr, count), ResultType.KeyValuePair | ResultType.Array | ResultType.String, cancellationToken);
        }

        /// <summary>
        /// Pops one element from the first non-empty list key from the list of provided key names
        /// <para>Available since: 7.0.0</para>
        /// <para>指定多个List, 从第一个非空的List头或尾部弹出1个元素</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">>List keys</param>
        /// <param name="lr">Specifies whether the element is ejected from the beginning or the tail. By default, the element is ejected from the head
        /// <para>指定从头还是尾部弹出元素, 默认从头部弹出元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: Value
        /// <para>Key: 从哪个Key中弹出了元素, Value: 弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? LMPopBytes(string[] keys, LeftRight lr = LeftRight.Left, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.LMPop(keys, lr, 1), ResultType.KeyValuePair | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// Pops one or more elements from the first non-empty list key from the list of provided key names
        /// <para>Available since: 7.0.0</para>
        /// <para>指定多个List, 从第一个非空的List头或尾部弹出N个元素</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">>List keys</param>
        /// <param name="count">Number of pop-up elements
        /// <para>弹出元素数量</para>
        /// </param>
        /// <param name="lr">Specifies whether the element is ejected from the beginning or the tail. By default, the element is ejected from the head
        /// <para>指定从头还是尾部弹出元素, 默认从头部弹出元素</para>
        /// </param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns>Key: Values
        /// <para>Key: 从哪个Key中弹出了元素, Value: 弹出的元素数组</para>
        /// </returns>
        public KeyValuePair<string, byte[][]>? LMPopBytes(string[] keys, ulong count, LeftRight lr = LeftRight.Left, CancellationToken cancellationToken = default)
        {
            return base._call.CallNullableStruct<KeyValuePair<string, byte[][]>>(ListCommands.LMPop(keys, lr, count), ResultType.KeyValuePair | ResultType.Array | ResultType.Bytes, cancellationToken);
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BLPop(string key, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BLPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BLPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BLPop(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BLPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BLPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BLPopBytes(string key, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BLPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BLPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BLPopBytes(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BLPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BLPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BLPop(string[] keys, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BLPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BLPop(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BLPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BLPopBytes(string[] keys, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BLPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BLPOP is a blocking list pop primitive. It is the blocking version of LPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the head of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的头部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BLPopBytes(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BLPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BRPop(string key, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BRPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BRPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BRPop(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BRPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BRPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BRPopBytes(string key, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BRPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BRPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="key">List key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BRPopBytes(string key, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
#if NET8_0_OR_GREATER
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BRPop([key], timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#else
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BRPop(new string[] { key }, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
#endif
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BRPop(string[] keys, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BRPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, string>? BRPop(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, string>>(ListCommands.BRPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.String, tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BRPopBytes(string[] keys, long timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BRPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOP is a blocking list pop primitive. It is the blocking version of RPOP because it blocks the connection when there are no elements to pop from any of the given lists. An element is popped from the tail of the first list that is non-empty, with the given keys being checked in the order that they are given
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从指定List的尾部弹出一个元素, 如果List是空的或不存在, 会阻塞等待</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>Element
        /// <para>弹出的元素</para>
        /// </returns>
        public KeyValuePair<string, byte[]>? BRPopBytes(string[] keys, double timeout)
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallNullableStruct<KeyValuePair<string, byte[]>>(ListCommands.BRPop(keys, timeout.ToString()), ResultType.KeyValuePair | ResultType.Bytes, tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOPLPUSH is the blocking variant of RPOPLPUSH.
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.2.0</para>
        /// <para>从源List的尾部删除一个元素, 并从目标List的头部插入, 如果源List不存在, 则阻塞</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>Returns null if times out</para>
        /// <para>移除并插入的元素</para>
        /// <para>如果超时, 则返回null</para>
        /// </returns>
        /// <exception cref="RedisException"></exception>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? BRPopLPush(string source, string destination, long timeout)
#else
        public string BRPopLPush(string source, string destination, long timeout)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallString(ListCommands.BRPopLPush(source, destination, timeout.ToString()), tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOPLPUSH is the blocking variant of RPOPLPUSH.
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 2.2.0</para>
        /// <para>从源List的尾部删除一个元素, 并从目标List的头部插入, 如果源List不存在, 则阻塞</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 2.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>Returns null if times out</para>
        /// <para>移除并插入的元素</para>
        /// <para>如果超时, 则返回null</para>
        /// </returns>
        /// <exception cref="RedisException"></exception>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? BRPopLPushBytes(string source, string destination, long timeout)
#else
        public byte[] BRPopLPushBytes(string source, string destination, long timeout)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallBytes(ListCommands.BRPopLPush(source, destination, timeout.ToString()), tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOPLPUSH is the blocking variant of RPOPLPUSH.
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从源List的尾部删除一个元素, 并从目标List的头部插入, 如果源List不存在, 则阻塞</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>Returns null if times out</para>
        /// <para>移除并插入的元素</para>
        /// <para>如果超时, 则返回null</para>
        /// </returns>
        /// <exception cref="RedisException"></exception>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? BRPopLPush(string source, string destination, double timeout)
#else
        public string BRPopLPush(string source, string destination, double timeout)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallString(ListCommands.BRPopLPush(source, destination, timeout.ToString()), tokenSource.Token);
            }
        }

        /// <summary>
        /// BRPOPLPUSH is the blocking variant of RPOPLPUSH.
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.0.0</para>
        /// <para>从源List的尾部删除一个元素, 并从目标List的头部插入, 如果源List不存在, 则阻塞</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.0.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>Returns null if times out</para>
        /// <para>移除并插入的元素</para>
        /// <para>如果超时, 则返回null</para>
        /// </returns>
        /// <exception cref="RedisException"></exception>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? BRPopLPushBytes(string source, string destination, double timeout)
#else
        public byte[] BRPopLPushBytes(string source, string destination, double timeout)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                return base._call.CallBytes(ListCommands.BRPopLPush(source, destination, timeout.ToString()), tokenSource.Token);
            }
        }

        /// <summary>
        /// BLMOVE is the blocking variant of LMOVE. When source contains elements, this command behaves exactly like LMOVE
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定Key的头部或尾部移除并获得一个元素, 并从头部或尾部插入到目标Key中. 如果源List不存在, 则阻塞</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="whereFrom">where from
        /// <para>指定从源Key的头部还是尾部移除</para>
        /// </param>
        /// <param name="whereTo">where to
        /// <para>指定从头部还是尾部插入到目标Key</para>
        /// </param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>Returns null if times out</para>
        /// <para>移除并插入的元素</para>
        /// <para>如果超时, 则返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public string? BLMove(string source, string destination, LeftRight whereFrom, LeftRight whereTo, double timeout)
#else
        public string BLMove(string source, string destination, LeftRight whereFrom, LeftRight whereTo, double timeout)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                try
                {
                    if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                    return base._call.CallString(ListCommands.BLMove(source, destination, whereFrom, whereTo, timeout.ToString()), tokenSource.Token);
                }
                catch (TimeoutException)
                {
                    return null;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// BLMOVE is the blocking variant of LMOVE. When source contains elements, this command behaves exactly like LMOVE
        /// <para>Not recommended because the connection will not respond if the blocking time is too long</para>
        /// <para>Available since: 6.2.0</para>
        /// <para>从指定Key的头部或尾部移除并获得一个元素, 并从头部或尾部插入到目标Key中. 如果源List不存在, 则阻塞</para>
        /// <para>不建议使用, 因为阻塞时间过长, 会导致连接无反应, 获取不到弹出的元素</para>
        /// <para>支持此命令的Redis版本, 6.2.0+</para>
        /// </summary>
        /// <param name="source">source key
        /// <para>要移除元素的Key</para>
        /// </param>
        /// <param name="destination">destination key
        /// <para>插入的目标Key</para>
        /// </param>
        /// <param name="whereFrom">where from
        /// <para>指定从源Key的头部还是尾部移除</para>
        /// </param>
        /// <param name="whereTo">where to
        /// <para>指定从头部还是尾部插入到目标Key</para>
        /// </param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <returns>The element being popped and pushed
        /// <para>Returns null if times out</para>
        /// <para>移除并插入的元素</para>
        /// <para>如果超时, 则返回null</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public byte[]? BLMoveBytes(string source, string destination, LeftRight whereFrom, LeftRight whereTo, double timeout)
#else
        public byte[] BLMoveBytes(string source, string destination, LeftRight whereFrom, LeftRight whereTo, double timeout)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            using (var tokenSource = new CancellationTokenSource())
            {
                try
                {
                    if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                    return base._call.CallBytes(ListCommands.BLMove(source, destination, whereFrom, whereTo, timeout.ToString()), tokenSource.Token);
                }
                catch (TimeoutException)
                {
                    return null;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// When any of the lists contains elements, this command behaves exactly like LMPOP
        /// <para>Available since: 7.0.0</para>
        /// <para>指定多个List, 从第一个非空List中从头或尾部弹出指定个数的元素, 如果没有非空的List, 则阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys
        /// <para>指定的多个List key数组</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <param name="lr">Specifies whether the element is ejected from the beginning or the tail. By default, the element is ejected from the head
        /// <para>指定从头还是尾部弹出元素, 默认从头部弹出元素</para>
        /// </param>
        /// <param name="count">Number of pop-up elements, default is 1
        /// <para>弹出元素数量, 默认为1个</para>
        /// </param>
        /// <returns>Key: Values
        /// <para>Key: 从哪个Key中弹出了元素, Value: 弹出的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public KeyValuePair<string, string[]>? BLMPop(string key, double timeout, string[]? keys = null, LeftRight lr = LeftRight.Left, ulong count = 1)
#else
        public KeyValuePair<string, string[]>? BLMPop(string key, double timeout, string[] keys = null, LeftRight lr = LeftRight.Left, ulong count = 1)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            var command = ListCommands.BLMPop(timeout.ToString(), key, keys, lr, count);
            using (var tokenSource = new CancellationTokenSource())
            {
                try
                {
                    if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                    return base._call.CallNullableStruct<KeyValuePair<string, string[]>>(command, ResultType.KeyValuePair | ResultType.Array | ResultType.String, tokenSource.Token);
                }
                catch (TimeoutException)
                {
                    return null;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// When any of the lists contains elements, this command behaves exactly like LMPOP
        /// <para>Available since: 7.0.0</para>
        /// <para>指定多个List, 从第一个非空List中从头或尾部弹出指定个数的元素, 如果没有非空的List, 则阻塞</para>
        /// <para>支持此命令的Redis版本, 7.0.0+</para>
        /// </summary>
        /// <param name="keys">List keys
        /// <para>指定的多个List key数组</para>
        /// </param>
        /// <param name="key">key</param>
        /// <param name="timeout">timeout seconds. 0 means no limit
        /// <para>超时阻塞时间, 单位: 秒. 0表示无限制</para>
        /// </param>
        /// <param name="lr">Specifies whether the element is ejected from the beginning or the tail. By default, the element is ejected from the head
        /// <para>指定从头还是尾部弹出元素, 默认从头部弹出元素</para>
        /// </param>
        /// <param name="count">Number of pop-up elements, default is 1
        /// <para>弹出元素数量, 默认为1个</para>
        /// </param>
        /// <returns>Key: Values
        /// <para>Key: 从哪个Key中弹出了元素, Value: 弹出的元素数组</para>
        /// </returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public KeyValuePair<string, byte[][]>? BLMPopBytes(string key, double timeout, string[]? keys = null, LeftRight lr = LeftRight.Left, ulong count = 1)
#else
        public KeyValuePair<string, byte[][]>? BLMPopBytes(string key, double timeout, string[] keys = null, LeftRight lr = LeftRight.Left, ulong count = 1)
#endif
        {
            if (timeout < 0) throw new RedisException("The timeout period cannot be negative");
            var command = ListCommands.BLMPop(timeout.ToString(), key, keys, lr, count);
            using (var tokenSource = new CancellationTokenSource())
            {
                try
                {
                    if (timeout > 0) tokenSource.CancelAfter(TimeSpan.FromSeconds(timeout + 2));
                    return base._call.CallNullableStruct<KeyValuePair<string, byte[][]>>(command, ResultType.KeyValuePair | ResultType.Array | ResultType.Bytes, tokenSource.Token);
                }
                catch (TimeoutException)
                {
                    return null;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
