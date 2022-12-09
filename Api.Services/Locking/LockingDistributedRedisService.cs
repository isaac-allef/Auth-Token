using Medallion.Threading;
using Medallion.Threading.Redis;
using StackExchange.Redis;

namespace Api.Services;

public class LockingDistributedRedisService : ILockingService
{
    private readonly IDistributedLockProvider _lock;

    public LockingDistributedRedisService(ConnectionMultiplexer connection)
    {
        _lock = new RedisDistributedSynchronizationProvider(connection.GetDatabase());
    }

    public async Task<T> RunAsync<T>(string name, Func<Task<T>> execution)
    {
        await using (await _lock.AcquireLockAsync(name))
        {
            return await execution();
        }
    }
}
