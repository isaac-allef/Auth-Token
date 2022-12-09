namespace Api.Services;

public class LockingSemaphoreService : ILockingService
{
    private readonly SemaphoreSlim _semaphore;

    public LockingSemaphoreService(int concurrentTasksNumberInSemaphore)
    {
        _semaphore = new SemaphoreSlim(concurrentTasksNumberInSemaphore);
    }

    public async Task<T> RunAsync<T>(string name, Func<Task<T>> execution)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await execution();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
