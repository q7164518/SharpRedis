namespace SharpRedis.Provider.Standard;

public interface IRedisTran
{
    IRedisTranClient CreateTranRedis();
}
