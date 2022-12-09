namespace Api.Services.Caching;

public interface ICachingService
{
    public Task<TValue?> GetAsync<TValue>(string key);
    public Task SetAsync<TValue>(string key, TValue value);
    public Task SetAsync<TValue>(string key, TValue value, TimeSpan expiration);
}
