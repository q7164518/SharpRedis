#pragma warning disable IDE0130
using System;

namespace SharpRedis
{
    /// <summary>
    /// Transaction return value
    /// <para>执行事务的返回值</para>
    /// </summary>
    public sealed class TransactionValue
    {
        private readonly bool _discarded;
        /// <summary>
        /// Whether the transaction was discarded and not executed.
        /// <para>事务是否被丢弃且未执行</para>
        /// </summary>
        public bool Discarded => this._discarded;


#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        private readonly object?[]? _values;
        private readonly object?[]? _queued;
        private readonly Exception? _discardedException;
#else
        private readonly object[] _values;
        private readonly object[] _queued;
        private readonly Exception _discardedException;
#endif

        /// <summary>
        /// Gets the transaction execution return value
        /// <para>获取事务执行的返回值</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public object?[]? Values => this._values;
#else
        public object[] Values => this._values;
#endif

        /// <summary>
        /// Gets the return value of the command added to the queue
        /// <para>获取命令入列的返回值</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public object? Queued => this._queued;
#else
        public object Queued => this._queued;
#endif

        /// <summary>
        /// Get the exception that the transaction discarded
        /// <para>获得事务被丢弃的异常对象</para>
        /// </summary>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Exception? DiscardedException => this._discardedException;
#else
        public Exception DiscardedException => this._discardedException;
#endif


#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        internal TransactionValue(object?[]? values, object?[]? _queued, bool discarded, Exception? exception)
#else
        internal TransactionValue(object[] values, object[] _queued, bool discarded, Exception exception)
#endif
        {
            this._values = values;
            this._queued = _queued;
            this._discarded = discarded;
            this._discardedException = exception;
        }

        public static implicit operator bool(TransactionValue value)
        {
            return !value._discarded && value._values != null;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static implicit operator object?[]?(TransactionValue value)
#else
        public static implicit operator object[](TransactionValue value)
#endif
        {
            return value._values;
        }

        /// <summary>
        /// Deconstruct
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="values"></param>
        /// <param name="queued"></param>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public void Deconstruct(out bool ok, out object?[]? values, out object?[]? queued)
#else
        public void Deconstruct(out bool ok, out object[] values, out object[] queued)
#endif
        {
            ok = !this._discarded && this._values != null;
            values = this._values;
            queued = this._queued;
        }

        /// <summary>
        /// Deconstruct
        /// </summary>
        /// <param name="values"></param>
        /// <param name="queued"></param>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public void Deconstruct(out object?[]? values, out object?[]? queued)
#else
        public void Deconstruct(out object[] values, out object[] queued)
#endif
        {
            values = this._values;
            queued = this._queued;
        }
    }
}
