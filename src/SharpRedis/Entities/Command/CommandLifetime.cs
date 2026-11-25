using System.Threading;

namespace SharpRedis.Entities.Command;

internal struct CommandLifetime
{
    // The sign bit tracks disposal by the caller. The remaining bits count send leases.
    private int _state;

    internal bool HasLeases => (Volatile.Read(ref this._state) & int.MaxValue) != 0;

    internal bool TryRetain()
    {
        int state;
        do
        {
            state = Volatile.Read(ref this._state);
            if (state < 0) return false;
        } while (Interlocked.CompareExchange(ref this._state, state + 1, state) != state);
        return true;
    }

    internal bool Release() => Interlocked.Decrement(ref this._state) == int.MinValue;

    internal bool DisposeOwner()
    {
        int state;
        do
        {
            state = Volatile.Read(ref this._state);
            if (state < 0) return false;
        } while (Interlocked.CompareExchange(ref this._state, state | int.MinValue, state) != state);
        return state == 0;
    }
}
