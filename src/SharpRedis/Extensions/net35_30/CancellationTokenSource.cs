#if LOW_NET
#pragma warning disable IDE0130
using System;
using System.Threading;

namespace SharpRedis
{
    public sealed class CancellationTokenSource : IDisposable
    {
        private volatile bool _isCancellationRequested;
        private readonly CancellationToken _cancellationToken;
        private volatile Timer _timer;
        private volatile ManualResetEvent _kernelEvent;
        private bool _disposedValue;

        internal static readonly CancellationTokenSource _neverCanceledSource = new CancellationTokenSource();

        public CancellationToken Token
        {
            get
            {
                if (this._disposedValue) throw new ObjectDisposedException(nameof(CancellationTokenSource));
                return this._cancellationToken;
            }
        }

        public WaitHandle WaitHandle
        {
            get
            {
                if (this._disposedValue) throw new ObjectDisposedException(nameof(CancellationTokenSource));
                if (this._kernelEvent != null) return this._kernelEvent;
                var mre = new ManualResetEvent(false);
                if (Interlocked.CompareExchange(ref this._kernelEvent, mre, null) != null)
                {
                    (mre as IDisposable).Dispose();
                }

                if (this.IsCancellationRequested)
                {
                    this._kernelEvent.Set();
                }
                return this._kernelEvent;
            }
        }

        public bool IsCancellationRequested => this._isCancellationRequested;

        public CancellationTokenSource()
        {
            this._cancellationToken = new CancellationToken(this);
        }

        public void Cancel()
        {
            if (this._disposedValue) throw new ObjectDisposedException(nameof(CancellationTokenSource));
            lock (this)
            {
                if (this._isCancellationRequested) return;
                this._isCancellationRequested = true;
                this._kernelEvent?.Set();
                this._timer?.Dispose();
                this._timer = null;
            }
        }

        public void CancelAfter(int millisecondsDelay)
        {
            if (this._disposedValue) throw new ObjectDisposedException(nameof(CancellationTokenSource));
            lock (this)
            {
                if (this._isCancellationRequested) return;
                this._timer?.Dispose();
                this._timer = null;
                this._timer = new Timer(_ => this.Cancel(), null, millisecondsDelay, Timeout.Infinite);
            }
        }

        public void CancelAfter(TimeSpan dueTime)
        {
            if (this._disposedValue) throw new ObjectDisposedException(nameof(CancellationTokenSource));
            lock (this)
            {
                if (this._isCancellationRequested) return;
                this._timer?.Dispose();
                this._timer = null;
                this._timer = new Timer(_ => this.Cancel(), null, (int)dueTime.TotalMilliseconds, Timeout.Infinite);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                    if (this._kernelEvent != null)
                    {
                        var mre = Interlocked.Exchange(ref this._kernelEvent, null);
                        if (mre != null) (mre as IDisposable).Dispose();
                    }
                    this._timer?.Dispose();
                }
                this._timer = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~CancellationTokenSource()
        {
            this.Dispose(disposing: false);
        }
    }
}
#endif