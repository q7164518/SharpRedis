#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using SharpRedis.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis
{
    public sealed partial class RedisList
    {
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
        public long LInsert(string key, ReadOnlySpan<char> element, string pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return this.LInsert(key, element.SpanToBytes(base._call.Encoding), pivot, ba, cancellationToken);
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
        public Task<long> LInsertAsync(string key, ReadOnlySpan<char> element, string pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return this.LInsertAsync(key, element.SpanToBytes(base._call.Encoding), pivot, ba, cancellationToken);
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
        public long LInsert(string key, ReadOnlySpan<char> element, ReadOnlySpan<char> pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return this.LInsert(key, element.SpanToBytes(base._call.Encoding), pivot.SpanToBytes(base._call.Encoding), ba, cancellationToken);
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
        public Task<long> LInsertAsync(string key, ReadOnlySpan<char> element, ReadOnlySpan<char> pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return this.LInsertAsync(key, element.SpanToBytes(base._call.Encoding), pivot.SpanToBytes(base._call.Encoding), ba, cancellationToken);
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
        public long LInsert(string key, string element, ReadOnlySpan<char> pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return this.LInsert(key, element, pivot.SpanToBytes(base._call.Encoding), ba, cancellationToken);
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
        public Task<long> LInsertAsync(string key, string element, ReadOnlySpan<char> pivot, BeforeAfter ba, CancellationToken cancellationToken = default)
        {
            return this.LInsertAsync(key, element, pivot.SpanToBytes(base._call.Encoding), ba, cancellationToken);
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
        public long LPush(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LPush(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long> LPushAsync(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LPushAsync(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public long LPushX(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LPushX(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long> LPushXAsync(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LPushXAsync(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public long RPush(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.RPush(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long> RPushAsync(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.RPushAsync(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public long RPushX(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.RPushX(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long> RPushXAsync(string key, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.RPushXAsync(key, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public long LRem(string key, long count, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LRem(key, count, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<long> LRemAsync(string key, long count, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LRemAsync(key, count, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public bool LSet(string key, long index, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LSet(key, index, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public Task<bool> LSetAsync(string key, long index, ReadOnlySpan<char> element, CancellationToken cancellationToken = default)
        {
            return this.LSetAsync(key, index, element.SpanToBytes(base._call.Encoding), cancellationToken);
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
        public long? LPos(string key, ReadOnlySpan<char> element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
        {
            return this.LPos(key, element.SpanToBytes(base._call.Encoding), rank, maxlen, cancellationToken);
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
        public Task<long?> LPosAsync(string key, ReadOnlySpan<char> element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
        {
            return this.LPosAsync(key, element.SpanToBytes(base._call.Encoding), rank, maxlen, cancellationToken);
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
        public long[]? LPos(ulong count, string key, ReadOnlySpan<char> element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
        {
            return this.LPos(count, key, element.SpanToBytes(base._call.Encoding), rank, maxlen, cancellationToken);
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
        public Task<long[]?> LPosAsync(ulong count, string key, ReadOnlySpan<char> element, long? rank = null, ulong? maxlen = null, CancellationToken cancellationToken = default)
        {
            return this.LPosAsync(count, key, element.SpanToBytes(base._call.Encoding), rank, maxlen, cancellationToken);
        }
    }
}
#endif
