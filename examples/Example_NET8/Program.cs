using SharpRedis;
var redis = Redis.UseStandalone(f =>
{
    f.Password = "123456";
    f.MaxPoolSize = 300;
});

//redis.PubSub.Subscribe("test", static (a, b) =>
//{
//    Console.WriteLine($"Channel: {a}, Data: {b}");
//    return Task.CompletedTask;
//});

//var chang = await redis.String.GetAsync("chang");
//;
await redis.Server.FlushAllAsync();
await redis.String.SetAsync("k1", "k1");
await redis.String.SetAsync("num", "1");
await redis.Hash.HSetAsync("h", "f", "嘿嘿");
await redis.List.RPushAsync("list", ["123", "456"]);


for (int i = 0; i < 300; i++)
{
    new Thread(async () =>
    {
        int indexx = i;
        int iii = indexx % 2;
        while (true)
        {
            var k1 = await redis.String.GetAsync("k1");
            if (k1 != "k1") Console.WriteLine("k1错误");

            await redis.String.IncrAsync("num");

            var hf = await redis.Hash.HGetAsync("h", "f");
            if (hf != "嘿嘿") Console.WriteLine("hash错误");

            var list = await redis.List.LIndexAsync("list", iii);
            if (iii == 0 && list != "123") Console.WriteLine("list错误");
            if (iii == 1 && list != "456") Console.WriteLine("list错误");
        }
    }).Start();
}
Console.WriteLine("启动成功.NET8");
Console.ReadLine();

























////using Autofac;
////using Example_NET8;
////using SharpRedis;
////using SharpRedis.Autofac;

////#region Autofac
////{
////    var builder = new ContainerBuilder();
////    builder.AddSharpRedisStandalone(option =>
////    {
////        option.Password = "123456";
////    });

////    builder.AddSharpRedisStandalone<LocalCache>(option =>
////    {
////        option.Password = "123456";
////    }, "localCache");

////    var container = builder.Build();

////    {
////        var redis = container.Resolve<Redis>();
////        var ok = await redis.String.SetAsync("key1", "Hello SharpRedis.Autofac");
////        var get = await redis.String.GetAsync("key1");

////        var redisString = container.Resolve<RedisString>();
////        get = await redisString.GetAsync("key1");
////    }

////    {
////        var redis = container.ResolveNamed<Redis>("localCache");
////        var ok = await redis.String.SetAsync("localcache_test:key1", "Hello SharpRedis.Autofac, LocalCache");
////        var get = await redis.String.GetAsync("localcache_test:key1");
////        get = await redis.String.GetAsync("localcache_test:key1");

////        var redisString = container.ResolveNamed<RedisString>("localCache");
////        get = await redisString.GetAsync("localcache_test:key1");
////        get = await redisString.GetAsync("none:key");
////    }
////}
////#endregion

////#region Unity

////#endregion

////Console.ReadLine();