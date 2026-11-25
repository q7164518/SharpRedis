using SharpRedis.Entities.Command;
using System;
using System.Text;

namespace SharpRedis.Pipelines.Middlewares.ClientCaching;

internal sealed class CacheKeyInvoker : IDisposable
{
    private bool _disposedValue;
    private Encoding _encoding;
    private string _keyPrefixString;
    private string _keySuffixString;

    internal CacheKeyInvoker(Encoding encoding,string keyPrefix, string keySuffix)
    {
        this._encoding = encoding;
        this._keyPrefixString = keyPrefix;
        this._keySuffixString = keySuffix;
    }

#if NET8_0_OR_GREATER
    unsafe
#endif
    internal string GetCacheKey(ICommand command)
    {
        // Keep return metadata outside the framed command arguments.
        return (command.RetutnBytes(0) ? "B:" : "T:")
            + command.CacheKey(this._encoding, this._keyPrefixString, this._keySuffixString);
    }

    internal string FullKey(ref readonly SharpValue key)
    {
        return $"{this._keyPrefixString}{key.ToString(this._encoding)}{this._keySuffixString}";
    }

    private void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            this._disposedValue = true;
            if (disposing)
            {
            }
            this._encoding = null!;
            this._keyPrefixString = null!;
            this._keySuffixString = null!;
        }
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
