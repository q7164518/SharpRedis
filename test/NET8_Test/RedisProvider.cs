using SharpRedis;

namespace NET8_Test;

public class RedisProvider : IEnumerable<object[]>
{
    private static readonly RedisItem _standalone = new(Redis.UseStandalone(option =>
    {
        option.Password = "123456";
        option.RespVersion = 3;
    }));

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [RedisProvider._standalone];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public class RedisItem(Redis redis)
{
    private readonly Redis _redis = redis;

    public static implicit operator Redis(RedisItem item) => item._redis;
}
