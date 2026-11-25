using SharpRedis.Entities.Command;
using SharpRedis.Network;
using System.Threading;
using System.Threading.Tasks;

namespace SharpRedis.Pipelines;

internal delegate RedisResult RedisDelegate(ICommand command, BaseConnection connection, CancellationToken cancellationToken);

internal delegate Task<RedisResult> AsyncRedisDelegate(ICommand command, BaseConnection connection, CancellationToken cancellationToken);
