using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpRedis;

public struct SharpEnumerator<T> : IEnumerator<T>
{
    private readonly IList<T> _items;
    private int _index;

    public SharpEnumerator()
    {
        throw new NotImplementedException();
    }

    internal SharpEnumerator(IList<T> items)
    {
        this._items = items;
        this._index = -1;
    }

    public readonly T Current => this._items[this._index];

    readonly object IEnumerator.Current => this.Current!;

    public bool MoveNext() => ++this._index < this._items.Count;

    public void Reset() => this._index = -1;

    public readonly void Dispose() { }
}
