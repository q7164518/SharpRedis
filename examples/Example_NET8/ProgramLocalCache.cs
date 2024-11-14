//using Autofac;
//using Example_NET8;
//using SharpRedis;
//using SharpRedis.Autofac;

//#region Autofac
//{
//    var builder = new ContainerBuilder();
//    builder.AddSharpRedisStandalone(option =>
//    {
//        option.Password = "123456";
//    });

//    builder.AddSharpRedisStandalone<LocalCache>(option =>
//    {
//        option.Password = "123456";
//    }, "localCache");

//    var container = builder.Build();

//    {
//        using var fdsf = new CancellationTokenSource();
//        fdsf.CancelAfter(TimeSpan.FromSeconds(5));
//        var redis = container.Resolve<Redis>();
//        var ok = await redis.String.SetAsync("key1", "Hello SharpRedis.Autofac", fdsf.Token);
//        var get = await redis.String.GetAsync("key1");

//        var redisString = container.Resolve<RedisString>();
//        get = await redisString.GetAsync("key1");
//    }

//    {
//        var redis = container.ResolveNamed<Redis>("localCache");
//        var ok = await redis.String.SetAsync("localcache_test:key1", "Hello SharpRedis.Autofac, LocalCache");
//        var get = await redis.String.GetAsync("localcache_test:key1");
//        get = await redis.String.GetAsync("localcache_test:key1");

//        var redisString = container.ResolveNamed<RedisString>("localCache");
//        get = await redisString.GetAsync("localcache_test:key1");
//        get = await redisString.GetAsync("none:key");
//        while (true)
//        {
//            //Console.WriteLine(await redis.String.GetAsync("localcache_test:key1"));
//            Console.WriteLine(await redis.String.MGetAsync(["localcache_test:key1","ffds"]));
//            Console.ReadLine();
//        }
//    }
//}
//#endregion

//#region Unity

//#endregion

//Console.ReadLine();