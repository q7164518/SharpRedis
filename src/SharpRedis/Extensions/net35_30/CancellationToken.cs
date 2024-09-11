#if LOW_NET
#pragma warning disable IDE0130
using System;
using System.Threading;

namespace SharpRedis
{
    public readonly struct CancellationToken
    {
        private readonly CancellationTokenSource _source;

        public bool IsCancellationRequested
        {
            get
            {
                if (this._source is null) return false;
                return this._source.IsCancellationRequested;
            }
        }

        public WaitHandle WaitHandle
        {
            get
            {
                if (this._source == null) return CancellationTokenSource._neverCanceledSource.WaitHandle;
                return this._source.WaitHandle;
            }
        }

        public static CancellationToken None => default;

        internal CancellationToken(CancellationTokenSource source)
        {
            this._source = source;
        }

        public void ThrowIfCancellationRequested()
        {
            if (this.IsCancellationRequested)
            {
                throw new OperationCanceledException("The operation has timed out.");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CancellationToken token)
            {
                return this.Equals(token);
            }
            return false;
        }

        public bool Equals(CancellationToken other)
        {
            if (this._source == null && other._source == null) return true;
            return this._source.Equals(other._source);
        }

        public override int GetHashCode()
        {
            return this._source.GetHashCode();
        }

        public static bool operator ==(CancellationToken left, CancellationToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CancellationToken left, CancellationToken right)
        {
            return !(left == right);
        }
    }
}
#endif
