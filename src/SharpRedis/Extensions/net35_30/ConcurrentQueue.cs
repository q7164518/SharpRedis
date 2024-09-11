#if LOW_NET
#pragma warning disable IDE0130
using System.Threading;

namespace System.Collections.Concurrent
{
    internal sealed class ConcurrentQueue<T>
    {
        private Node _head;
        private Node _tail;

        internal bool IsEmpty
        {
            get
            {
                return this._head._next == null;
            }
        }

        internal ConcurrentQueue()
        {
            this._head = new Node(default);
            this._tail = _head;
        }

        internal void Enqueue(T item)
        {
            var newNode = new Node(item);
            while (true)
            {
                Node oldTail = this._tail;
                Node oldTailNext = oldTail._next;

                if (this._tail == oldTail)
                {
                    if (oldTailNext == null)
                    {
                        if (Interlocked.CompareExchange(ref oldTail._next, newNode, null) == null)
                        {
                            _ = Interlocked.CompareExchange(ref this._tail, newNode, oldTail);
                            return;
                        }
                    }
                    else
                    {
                        _ = Interlocked.CompareExchange(ref this._tail, oldTailNext, oldTail);
                    }
                }
            }
        }

        internal bool TryDequeue(out T result)
        {
            result = default;
            while (true)
            {
                var oldHead = this._head;
                var oldTail = this._tail;
                var oldHeadNext = oldHead._next;

                if (oldHead == this._head)
                {
                    if (oldHead == oldTail)
                    {
                        if (oldHeadNext == null) return false;
                        _ = Interlocked.CompareExchange(ref this._tail, oldHeadNext, oldTail);
                    }
                    else
                    {
                        result = oldHeadNext._value;
                        if (Interlocked.CompareExchange(ref this._head, oldHeadNext, oldHead) == oldHead)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        internal bool TryPeek(out T result)
        {
            var oldHeadNext = this._head._next;
            if (oldHeadNext == null)
            {
                result = default;
                return false;
            }
            result = oldHeadNext._value;
            return true;
        }

        internal int Count
        {
            get
            {
                int count = 0;
                var current = this._head._next;
                while (current != null)
                {
                    count++;
                    current = current._next;
                }
                return count;
            }
        }

        internal void Clear()
        {
            this._head = new Node(default);
            this._tail = this._head;
        }

        private sealed class Node
        {
            internal T _value;
            internal Node _next;

            internal Node(T value)
            {
                this._value = value;
                this._next = null;
            }
        }
    }
}
#endif
