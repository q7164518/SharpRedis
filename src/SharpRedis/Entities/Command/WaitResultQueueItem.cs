using SharpRedis.Enums;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SharpRedis.Entities.Command;

internal struct WaitResultQueueItem
{
    private readonly bool _returnBytes;
    private readonly int _resultCount;
    private int _currentResultIndex = 0;
#if MODERN_NET
    private readonly RedisResult[]? _multipleResult;
    private readonly bool[]? _batchReturnBytes;
#else
    private readonly RedisResult[] _multipleResult;
    private readonly bool[] _batchReturnBytes;
#endif

    internal TaskCompletionSource<RedisResult> ResultHandler { get; private set; }

    internal readonly int ResultCount => this._resultCount;

    internal readonly int CurrentResultIndex => this._currentResultIndex;

    internal WaitResultQueueItem(
        TaskCompletionSource<RedisResult> tcs,
        ICommand command)
    {
        this.ResultHandler = tcs;
        this._resultCount = command.ResultCount;
        if (command.IsBatch)
        {
            this._multipleResult = new RedisResult[this._resultCount];
            for (int i = 0; i < this._resultCount; i++)
            {
                if (command.RetutnBytes(i))
                {
                    this._batchReturnBytes ??= new bool[this._resultCount];
                    this._batchReturnBytes[i] = true;
                }
            }
        }
        else
        {
            this._returnBytes = command.RetutnBytes(0);
        }
    }

    internal bool SetResult(RedisResult result, Encoding encoding)
    {
        if (this._multipleResult is null)
        {
            _ = this.ResultHandler.TrySetResult(result);
            return true;
        }

        this._multipleResult![this._currentResultIndex++] = result;
        if (this._currentResultIndex >= this.ResultCount)
        {
            _ = this.ResultHandler.TrySetResult(new RedisResult(RespType.Array, this._multipleResult, encoding));
            return true;
        }
        return false;
    }

    internal readonly void SetException(Exception ex, Encoding encoding)
    {
        _ = this.ResultHandler.TrySetResult(new(RespType.Exception, ex, encoding));
    }

    internal readonly bool RetutnBytes(int index)
    {
        return this._batchReturnBytes is { } flags ? flags[index] : this._returnBytes;
    }
}
