using SharpRedis.Entities;
using SharpRedis.Network.Pool;

namespace SharpRedis.Network;

/// <summary>
/// Only Cluster connections carry routing/transaction state. Ordinary connections are unchanged.
/// </summary>
internal sealed class ClusterConnection : BaseConnection
{
    internal ClusterNodePool Node { get; }

    internal bool IsReplica { get; }

    internal bool Dedicated { get; }

    internal int Slot { get; set; } = -1;

    internal bool MultiPending { get; set; }

    internal bool MultiSent { get; set; }

    internal ClusterConnection Target { get; set; } = null!;

    internal ClusterConnection(
        SharpConnectionOptions options,
        CommandDispatcher dispatcher,
        ClusterNodePool node,
        bool replica = false,
        bool dedicated = false)
        : base(options, dispatcher, options.CommandTimeout)
    {
        this.Node = node;
        this.IsReplica = replica;
        this.Dedicated = dedicated;
        base.BeginReceive(null);
    }

    internal override BaseConnection Copy(IConnectionPool pool)
        => pool.CreateMasterConnection();

    protected private override void Dispose(bool disposing)
    {
        if (base._disposedValue) return;
        base._disposedValue = true;
        this.Target?.Dispose();
        this.Node?.Forget(this);
        base.Dispose(disposing);
    }
}
