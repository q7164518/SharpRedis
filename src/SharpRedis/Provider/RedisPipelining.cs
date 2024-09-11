#pragma warning disable IDE0130
#if !LOW_NET
using System.Threading;
using System.Threading.Tasks;
#endif
using SharpRedis.Provider.Calls;
using System;
using SharpRedis.Provider.Standard;

namespace SharpRedis
{
    /// <summary>
    /// Redis pipelining
    /// <para>Redis管道功能</para>
    /// </summary>
    public sealed class RedisPipelining : FeatureRedis<RedisPipelining>
    {
        private PipeliningCall _pipeCall;

        private protected sealed override string DisposedException => "The pipelining has been released and cannot continue";

        internal RedisPipelining(PipeliningCall call) : base(call)
        {
            this._pipeCall = call;
        }

        /// <summary>
        /// Execute the command pipeline and get the return value.
        /// <para>Return an array of values in the same order as the commands in the pipe</para>
        /// <para>执行命令管道并获取返回值</para>
        /// <para>返回值数组顺序和管道执行的命令顺序一致</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public object?[]? ExecutePipelining(CancellationToken cancellationToken = default)
#else
        public object[] ExecutePipelining(CancellationToken cancellationToken = default)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisPipelining), this.DisposedException);
             return this._pipeCall.ExecPipelining(cancellationToken);
        }

        /// <summary>
        /// Execute the command pipeline and get the return value.
        /// <para>Return an array of values in the same order as the commands in the pipe</para>
        /// <para>执行命令管道并获取返回值</para>
        /// <para>返回值数组顺序和管道执行的命令顺序一致</para>
        /// </summary>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled. The valid value overrides the system Token
        /// <para>取消Token, 有效值会覆盖系统默认的</para>
        /// </param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public object?[]? EndPipelining(CancellationToken cancellationToken = default)
#else
        public object[] EndPipelining(CancellationToken cancellationToken = default)
#endif
        {
            return this.ExecutePipelining(cancellationToken);
        }

#if !LOW_NET
        /// <summary>
        /// Execute the command pipeline and get the return value.
        /// <para>Return an array of values in the same order as the commands in the pipe</para>
        /// <para>执行命令管道并获取返回值</para>
        /// <para>返回值数组顺序和管道执行的命令顺序一致</para>
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?[]?> ExecutePipeliningAsync(CancellationToken cancellationToken = default)
#else
        public Task<object[]> ExecutePipeliningAsync(CancellationToken cancellationToken = default)
#endif
        {
            if (base._disposedValue) throw new ObjectDisposedException(nameof(RedisPipelining), this.DisposedException);
            return this._pipeCall.ExecPipeliningAsync(cancellationToken);
        }

        /// <summary>
        /// Execute the command pipeline and get the return value.
        /// <para>Return an array of values in the same order as the commands in the pipe</para>
        /// <para>执行命令管道并获取返回值</para>
        /// <para>返回值数组顺序和管道执行的命令顺序一致</para>
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public Task<object?[]?> EndPipeliningAsync(CancellationToken cancellationToken = default)
#else
        public Task<object[]> EndPipeliningAsync(CancellationToken cancellationToken = default)
#endif
        {
            return this.ExecutePipeliningAsync(cancellationToken);
        }
#endif

        protected sealed override void Dispose(bool disposing)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8625
#endif
            base.Dispose(disposing);
            this._pipeCall = null;
        }

        ~RedisPipelining()
        {
            this.Dispose(true);
        }
    }
}
