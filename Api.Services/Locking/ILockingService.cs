namespace Api.Services;

public interface ILockingService
{
    public Task<T> RunAsync<T>(string name, Func<Task<T>> execution);
}
