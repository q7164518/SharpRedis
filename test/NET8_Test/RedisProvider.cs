using SharpRedis;

namespace NET8_Test;

public class RedisProvider : IEnumerable<object[]>
{
    private static readonly RedisItem _standalone = new(Redis.UseStandalone(option =>
    {
        option.Password = "123456";
        option.RespVersion = 3;
    }));

    private static readonly RedisItem _masterSlave = new(Redis.UseMasterSlave(2, (m, s) =>
    {
        m.Password = "123456";
        m.Port = 2352;

        s[0].Password = "123456";
        s[0].Port = 2353;

        s[1].Password = "123456789";
        s[1].Port = 2354;
    }));

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [RedisProvider._standalone];
        yield return [RedisProvider._masterSlave];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public class RedisItem(Redis redis)
{
    private readonly Redis _redis = redis;

    public static implicit operator Redis(RedisItem item) => item._redis;
}
