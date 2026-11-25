using SharpRedis;

namespace NET10.Server;

// Requires an isolated Redis 7+ with module commands enabled and the Redis
// tests/modules/moduleconfigs.c fixture compiled on the server.
[NonParallelizable]
internal class ModuleTests
{
    [TestCase(Resp.RESP2, false)]
    [TestCase(Resp.RESP2, true)]
    [TestCase(Resp.RESP3, false)]
    [TestCase(Resp.RESP3, true)]
    public async Task LoadListUnload(Resp resp, bool async)
    {
        var connection = Environment.GetEnvironmentVariable("SHARPREDIS_MODULE_TEST_CONNECTION");
        var path = Environment.GetEnvironmentVariable("SHARPREDIS_MODULE_TEST_PATH");
        if (string.IsNullOrEmpty(connection) || string.IsNullOrEmpty(path))
            Assert.Ignore("Set SHARPREDIS_MODULE_TEST_CONNECTION and SHARPREDIS_MODULE_TEST_PATH for the moduleconfigs fixture.");

        using var redis = Redis.UseStandalone(connection!, o => o.RespVersion = resp);
        const string name = "moduleconfigs";
        Assert.That(redis.Server.ModuleList().Any(module => module.Name == name), Is.False);
        foreach (bool extended in new[] { false, true })
        {
            KeyValuePair<string, SharpValue>[] configs = [new("moduleconfigs.numeric", "123"), new("moduleconfigs.mutable_bool", "no")];
            byte[] binary = [0, 255, 65];
            SharpValue[] args = ["first argument", binary];
            bool loaded = extended
                ? (async ? await redis.Server.ModuleLoadExAsync(path!, configs, args) : redis.Server.ModuleLoadEx(path!, configs, args))
                : (async ? await redis.Server.ModuleLoadAsync(path!, args) : redis.Server.ModuleLoad(path!, args));
            try
            {
                Assert.That(loaded, Is.True);
                var modules = async ? await redis.Server.ModuleListAsync(CommandFlags.ReturnBytes) : redis.Server.ModuleList(CommandFlags.ReturnBytes);
                var module = modules.Single(item => item.Name == name);
                Assert.That(module.Version, Is.EqualTo(1));
                Assert.That(module.Path, Is.EqualTo(path));
                Assert.That(module.Args, Has.Length.EqualTo(2));
                Assert.That((string?)module.Args[0], Is.EqualTo("first argument"));
                Assert.That(module.Args[1].RawValue, Is.EqualTo(binary));
                if (extended)
                {
                    var values = redis.Server.ConfigGet("moduleconfigs.*")!;
                    Assert.That(values["moduleconfigs.numeric"], Is.EqualTo("123"));
                    Assert.That(values["moduleconfigs.mutable_bool"], Is.EqualTo("no"));
                }
            }
            finally
            {
                Assert.That(async ? await redis.Server.ModuleUnloadAsync(name) : redis.Server.ModuleUnload(name), Is.True);
            }
            Assert.That(redis.Server.ModuleList().Any(item => item.Name == name), Is.False);
        }
    }
}
