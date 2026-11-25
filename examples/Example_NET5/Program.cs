using System;
using System.Threading.Tasks;
using SharpRedis;

namespace Example_NET5
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //ReadOnlySpan<string> fds = ["", ""];
            //SharpValueSpan fdsf = new(["a", "v", new byte[2], "fsdfsd".ToCharArray()]);
            var redis = Redis.UseStandalone("password=123456", o => o.RespVersion = Resp.RESP2);
            var inini = await redis.Server.LatencyDoctorAsync().ConfigureAwait(false);
            var result =  await redis.Server.HotKeysGetAsync().ConfigureAwait(false);
            var ddfdf = redis.Server.ConfigGet(new string[] { "*max-*-entries*", "maxmemory" });
            var fdfdsfds = await redis.Server.ConfigResetStatAsync().ConfigureAwait(false);
            var pass = redis.Server.ConfigSet("requirepass", "123456");
            var commm = await redis.Server.CommandAsync();
            var info = redis.Server.Info();
            var lol = await redis.Server.LolwutAsync(6);
            var time = redis.Server.Time();
            var roel = await redis.Server.RoleAsync().ConfigureAwait(false);
            var rangear = await redis.Array.ArDelRangeAsync("a1aa", 100..1024);
            var getrange = await redis.Array.ArGetRangeAsync("a1", ..20).ConfigureAwait(false);
            var argrep = redis.Array.ArGrep("a1", "-", '+', RedisArray.GrepConditions.Create((RedisArray.GrepPredicate.Match, "A")).OrMatch("f"), withValues: true, limit: 200, noCase: true);
            var aritems = await redis.Array.ArLastItemsAsync("a1", 100,true);
            var arlen = redis.Array.ArLen("a111");
            var armget = redis.Array.ArMGet("a111", new long[] { 1,2,767,88});
            var nextinde = redis.Array.ArNext("a1");
            var aroppp = await redis.Array.ArOpAsync("a2", 0..100, RedisArray.AggregateOp.Min,"name");
            var arring = redis.Array.ArRing("a3", 10, "fd");
            var typere = redis.Key.Type("a3");
            var sarascan = redis.Array.ArScan("a333", (1,5));
            var arinfo = await redis.Array.ArInfoAsync("a3",true);

            var saddex = redis.Set.SAddEx("set1", "hhhh", TimeSpan.FromSeconds(10), when: RedisSet.AddExWhen.MNX);

            _ = redis.Set.SAdd("set_hh", new string[] { "1", "2", "3" });
            var sexxx = await redis.Set.SExpireAsync("set_hh", new string[] {"1","3","npno" }, 10, RedisSet.ExpirationWhen.NX);
            var sexxxat = redis.Set.SExpireAt("set_hh", new string[] {"1","3","npno" }, DateTimeOffset.UtcNow.AddSeconds(10), RedisSet.ExpirationWhen.XX);
            var settime = await redis.Set.SExpireTimeAsync("set_hh", new string[] { "1", "3", "npno","2" });
            var spersistasync = redis.Set.SPersist("set_hh", "1");
            var spersistarray = redis.Set.SPersist("set_hh", new string[] { "1", "3", "npno", "2" });
            var spexxx = await redis.Set.SPExpireAsync("set_hh", new string[] { "1", "3", "npno" }, TimeSpan.FromSeconds(4), RedisSet.ExpirationWhen.NX);
            //var spexxx1 = redis.Set.SPExpire("set_hh", "2", TimeSpan.FromSeconds(4), RedisSet.ExpirationWhen.NX);
            var spexpireat = redis.Set.SPExpireAt("set_hh", new string[] { "1", "3", "npno" }, DateTimeOffset.UtcNow.AddSeconds(5));
            var SPExpireTime1 = redis.Set.SPExpireTime("set_hh", "3");
            var SPExpireTime2 = redis.Set.SPExpireTime("set_hh", new string[] { "1", "3", "npno" });
            var SPExpireTime3 = await redis.Set.SPExpireTimeAsync("set_hh", new string[] { "1", "3", "npno" ,"2"});
            var sttl = redis.Set.STtl("set_hh", "1");
            var sttl1 = redis.Set.STtl("set_hh", new string[] { "1", "3", "npno" });

            var spttl = await redis.Set.SPTtlAsync("set_hh", "1");
            var spttl1 = await redis.Set.SPTtlAsync("set_hh", new string[] { "1", "3", "npno" });

            redis.SortedSet.ZAdd("zset", "hah", 1);
            redis.SortedSet.ZAdd("zset", "ooOK", 2);
            redis.SortedSet.ZAdd("zset", "ooOK111", 4);
            redis.SortedSet.ZAdd("zset", "你哈哦", 40);
            redis.SortedSet.ZAdd("zset", "嘤嘤嘤", 100);

            var fdsf = redis.SortedSet.ZMove("zset11jj", "d_zset", RedisSortedSet.MoveHow.ByScore, double.NegativeInfinity, double.PositiveInfinity,1);

            var movemember = redis.SortedSet.ZMoveMember("zset", "d_ssss", "ooOK", RedisSortedSet.MoveWhen.XX);

            var baohu = redis.Key.BlessSet("zset");
            var baohuget = await redis.Key.BlessGetAsync("set_hh").ConfigureAwait(false);
            var fdsfsd = redis.Key.BlessScan(0);
            var baohuclear = redis.Key.BlessClear("zset");
            baohuclear = redis.Key.BlessClear("zset");

            using (var pipe = redis.StartPipelining())
            {
                _ = pipe.Server.CommandList();
                _ = pipe.Server.CommandInfo();
                _ = pipe.String.IncrEx("1");
                await pipe.Server.DBSizeAsync();
                var result1 = await pipe.InvokePipeliningAsync().ConfigureAwait(false);
            }

            ;
        }
    }
}
