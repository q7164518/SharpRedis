using Autofac;
using SharpRedis;

namespace NET10;

internal class Factory
{
    private static readonly Dictionary<Redis, string> _prefixes = new();

    internal static IContainer Container { get; private set; }

    static Factory()
    {
        var builder = new ContainerBuilder();
        var standaloneRedis3 = Redis.UseStandalone(static f =>
        {
            f.Password = "123456";
            f.RespVersion = Resp.RESP3;
            f.Prefix = "resp3:";
        });
        var standaloneRedis2 = Redis.UseStandalone(static f =>
        {
            f.Password = "123456";
            f.Prefix = "resp2:";
        });
        Factory._prefixes.Add(standaloneRedis3, "resp3:");
        Factory._prefixes.Add(standaloneRedis2, "resp2:");

        builder.RegisterInstance(standaloneRedis3)
            .Named<Redis>("StandaloneRedis3")
            .OnActivated(static redis =>
            {
                _ = redis.Instance.Server.FlushAll();
            });

        builder.RegisterInstance(standaloneRedis2)
            .Named<Redis>("StandaloneRedis2")
            .OnActivated(static redis =>
            {
                _ = redis.Instance.Server.FlushAll();
            });
        Factory.Container = builder.Build();
    }

    public static IEnumerable<TestFixtureData> CreateInstance()
    {
        yield return new TestFixtureData(Factory.Container.ResolveNamed<Redis>("StandaloneRedis3")).SetArgDisplayNames("Standalone_RESP3");

        yield return new TestFixtureData(Factory.Container.ResolveNamed<Redis>("StandaloneRedis2")).SetArgDisplayNames("Standalone_RESP2");
    }

    internal static string GetPrefix(Redis redis)
    {
        return Factory._prefixes[redis];
    }
}
