namespace SharpRedis.Provider.Standard;

public interface IRedisSwitch
{
    IRedisSwitchClient CreateSwitchRedis(int database);
}
