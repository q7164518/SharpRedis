using System.Threading.Tasks;

namespace SharpRedis.Network;

internal sealed class HeartbeatExecution : TaskCompletionSource<RedisResult>
{
    internal HeartbeatExecution() : base(TaskCreationOptions.RunContinuationsAsynchronously)
    {
    }

    internal TaskCompletionSource<bool> Sent { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
}
