using SharpRedis.Provider.Standard;

namespace SharpRedis;

public static class SwitchDataBaseExtensions
{
    /// <summary>
    /// Switch temporarily to another database
    /// <para>临时切换到其它数据库</para>
    /// </summary>
    /// <param name="redis"></param>
    /// <param name="database">The subscript of the database to be switched to
    /// <para>要切换到的数据库下标</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var db = redis.Select(10))
    /// {
    ///     var r1 = db.String.Get("key");
    ///     var r2 = await db.String.GetAsync("key");
    /// }
    /// </code>
    /// </example>
    public static IRedisSwitchClient Select(this IRedisSwitch redis, int database)
        => redis.Switch(database);

    /// <summary>
    /// Switch temporarily to another database
    /// <para>临时切换到其它数据库</para>
    /// </summary>
    /// <param name="redis"></param>
    /// <param name="database">The subscript of the database to be switched to
    /// <para>要切换到的数据库下标</para>
    /// </param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// using (var db = redis.Select(10))
    /// {
    ///     var r1 = db.String.Get("key");
    ///     var r2 = await db.String.GetAsync("key");
    /// }
    /// </code>
    /// </example>
    public static IRedisSwitchClient Switch(this IRedisSwitch redis, int database)
    {
        return redis.CreateSwitchRedis(database);
    }
}
