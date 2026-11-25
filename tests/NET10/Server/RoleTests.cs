using SharpRedis;

namespace NET10.Server;

internal class RoleTests
{
    // Use a primary with a connected replica and a Sentinel monitoring that primary.
    [TestCase(Resp.RESP2, false)]
    [TestCase(Resp.RESP2, true)]
    [TestCase(Resp.RESP3, false)]
    [TestCase(Resp.RESP3, true)]
    public async Task RoleReportsSelectedNode(Resp resp, bool async)
    {
        string Connection(string role)
        {
            var name = $"SHARPREDIS_ROLE_TEST_{role}";
            var connection = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(connection)) Assert.Ignore($"Set {name} to run the ROLE topology checks.");
            return connection!;
        }

        using var primary = Redis.UseStandalone(Connection("MASTER"), o => o.RespVersion = resp);
        using var replica = Redis.UseStandalone(Connection("REPLICA"), o => o.RespVersion = resp);
        using var sentinel = Redis.UseStandalone(Connection("SENTINEL"), o => o.RespVersion = resp);
        var expectedName = Environment.GetEnvironmentVariable("SHARPREDIS_ROLE_TEST_MASTER_NAME");
        foreach (var flags in new[] { CommandFlags.None, CommandFlags.ReturnBytes })
        {
            var master = async ? await primary.Server.RoleAsync(flags) : primary.Server.Role(flags);
            Assert.That(master.Role, Is.EqualTo("master"));
            Assert.That(master.ReplicationOffset, Is.GreaterThanOrEqualTo(0));
            Assert.That(master.Replicas, Is.Not.Empty);
            foreach (var item in master.Replicas)
            {
                Assert.That(item.Host, Is.Not.Empty);
                Assert.That(item.Port, Is.InRange(1, 65535));
                Assert.That(item.ReplicationOffset, Is.InRange(0L, master.ReplicationOffset!.Value));
            }

            var slave = async ? await replica.Server.RoleAsync(flags) : replica.Server.Role(flags);
            Assert.That(slave.Role, Is.EqualTo("slave"));
            Assert.That(slave.MasterHost, Is.Not.Empty);
            Assert.That(slave.MasterPort, Is.InRange(1, 65535));
            Assert.That(slave.ReplicationState, Is.EqualTo("connected"));
            Assert.That(slave.ReplicationOffset, Is.GreaterThanOrEqualTo(0));
            Assert.That(slave.Replicas, Is.Empty);

            var monitor = async ? await sentinel.Server.RoleAsync(flags) : sentinel.Server.Role(flags);
            Assert.That(monitor.Role, Is.EqualTo("sentinel"));
            Assert.That(monitor.MonitoredMasters, Is.Not.Empty);
            if (expectedName != null) Assert.That(monitor.MonitoredMasters, Does.Contain(expectedName));
            Assert.That(monitor.ReplicationOffset, Is.Null);
        }
    }
}
