#if LOW_NET
#pragma warning disable IDE0130
using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent
{
    internal sealed class ConcurrentStack<T>
    {
        private Node _head = null;

        internal bool IsEmpty => this._head == null;

        internal ConcurrentStack()
        {
        }

        internal void Push(T item)
        {
            var newNode = new Node(item);
            while (true)
            {
                var oldHead = this._head;
                newNode._next = oldHead;
                if (Interlocked.CompareExchange(ref this._head, newNode, oldHead) == oldHead)
                {
                    return;
                }
            }
        }

        internal bool TryPop(out T result)
        {
            while (true)
            {
                var oldHead = this._head;
                if (oldHead == null)
                {
                    result = default;
                    return false;
                }

                Node newHead = oldHead._next;
                if (Interlocked.CompareExchange(ref this._head, newHead, oldHead) == oldHead)
                {
                    result = oldHead._value;
                    return true;
                }
            }
        }

        internal bool TryPeek(out T result)
        {
            var oldHead = this._head;
            if (oldHead == null)
            {
                result = default;
                return false;
            }
            result = oldHead._value;
            return true;
        }

        internal int Count
        {
            get
            {
                int count = 0;
                var current = this._head;
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
            this._head = null;
        }

        internal T[] ToArray()
        {
            var list = new List<T>();
            var current = this._head;
            while (current != null)
            {
                list.Add(current._value);
                current = current._next;
            }
            list.Reverse();
            return list.ToArray();
        }

        private sealed class Node
        {
            internal T _value;
            internal Node _next;

            public Node(T value)
            {
                this._value = value;
                this._next = null;
            }
        }
    }
}
#endif
