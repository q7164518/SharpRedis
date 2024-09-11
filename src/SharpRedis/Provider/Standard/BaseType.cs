using System;

namespace SharpRedis.Provider.Standard
{
    public abstract class BaseType: IDisposable
    {
        private protected BaseCall _call;
        private bool _disposedValue;

        internal BaseType(BaseCall call)
        {
            this._call = call;
        }

        internal void SetCall(BaseCall call)
        {
            this._call = call;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                this._disposedValue = true;
                if (disposing)
                {
                }
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
#pragma warning disable CS8625
#endif
                this._call = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
