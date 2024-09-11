#if LOW_NET
#pragma warning disable IDE0130
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    internal sealed class ConcurrentDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly int _numLocks;
        private readonly object[] _locks;
        private readonly Dictionary<TKey, TValue>[] _dictionaries;

        internal int Count
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _numLocks; i++)
                {
                    lock (this._locks[i])
                    {
                        count += this._dictionaries[i].Count;
                    }
                }
                return count;
            }
        }

        internal bool IsEmpty
        {
            get
            {
                for (uint i = 0; i < this._numLocks; i++)
                {
                    lock (this._locks[i])
                    {
                        if (this._dictionaries[i].Count > 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        internal IEnumerable<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>();
                for (int i = 0; i < this._numLocks; i++)
                {
                    lock (this._locks[i])
                    {
                        keys.AddRange(this._dictionaries[i].Keys);
                    }
                }
                return keys;
            }
        }

        internal IEnumerable<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                for (int i = 0; i < this._numLocks; i++)
                {
                    lock (this._locks[i])
                    {
                        values.AddRange(this._dictionaries[i].Values);
                    }
                }
                return values;
            }
        }

        internal ConcurrentDictionary(int concurrencyLevel = 16, int initialCapacity = 101)
        {
            this._numLocks = concurrencyLevel;
            this._locks = new object[this._numLocks];
            this._dictionaries = new Dictionary<TKey, TValue>[this._numLocks];

            for (uint i = 0; i < this._numLocks; i++)
            {
                this._locks[i] = new object();
                this._dictionaries[i] = new Dictionary<TKey, TValue>(initialCapacity / _numLocks);
            }
        }

        private int GetLockIndex(TKey key)
        {
            return (key.GetHashCode() & 0x7FFFFFFF) % this._numLocks;
        }

        internal bool TryAdd(TKey key, TValue value)
        {
            int lockIndex = this.GetLockIndex(key);
            lock (this._locks[lockIndex])
            {
                if (this._dictionaries[lockIndex].ContainsKey(key))
                {
                    return false;
                }
                this._dictionaries[lockIndex][key] = value;
                return true;
            }
        }

        internal bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            int lockIndex = this.GetLockIndex(key);
            lock (this._locks[lockIndex])
            {
                if (this._dictionaries[lockIndex].TryGetValue(key, out var existingValue) && EqualityComparer<TValue>.Default.Equals(existingValue, comparisonValue))
                {
                    this._dictionaries[lockIndex][key] = newValue;
                    return true;
                }
                return false;
            }
        }

        internal bool TryRemove(TKey key, out TValue value)
        {
            int lockIndex = this.GetLockIndex(key);
            lock (this._locks[lockIndex])
            {
                if (this._dictionaries[lockIndex].TryGetValue(key, out value))
                {
                    this._dictionaries[lockIndex].Remove(key);
                    return true;
                }
                return false;
            }
        }

        internal bool TryGetValue(TKey key, out TValue value)
        {
            int lockIndex = this.GetLockIndex(key);
            lock (this._locks[lockIndex])
            {
                return this._dictionaries[lockIndex].TryGetValue(key, out value);
            }
        }

        internal TValue this[TKey key]
        {
            get
            {
                if (this.TryGetValue(key, out var value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                int lockIndex = this.GetLockIndex(key);
                lock (this._locks[lockIndex])
                {
                    this._dictionaries[lockIndex][key] = value;
                }
            }
        }

        internal bool ContainsKey(TKey key)
        {
            return this.TryGetValue(key, out _);
        }

        internal void Clear()
        {
            for (int i = 0; i < _numLocks; i++)
            {
                lock (this._locks[i])
                {
                    this._dictionaries[i].Clear();
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < this._numLocks; i++)
            {
                lock (this._locks[i])
                {
                    foreach (var kvp in this._dictionaries[i])
                    {
                        yield return kvp;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif
