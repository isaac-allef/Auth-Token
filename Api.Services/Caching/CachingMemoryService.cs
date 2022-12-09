using Microsoft.Extensions.Caching.Memory;

namespace Api.Services.Caching;

public class CachingMemoryService : ICachingService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan DEFAULT_EXPIRATION = TimeSpan.FromMinutes(5);

    public CachingMemoryService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<TValue?> GetAsync<TValue>(string key)
    {
        var value = _cache.Get<TValue>(key);
        return Task.FromResult<TValue?>(value);
    }

    public Task SetAsync<TValue>(string key, TValue value)
        => SetAsync<TValue>(key, value, DEFAULT_EXPIRATION);

    public Task SetAsync<TValue>(string key, TValue value, TimeSpan expiration)
    {
        var options = new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = expiration
        };
        
        _cache.Set<TValue>(key, value, options);
        return Task.CompletedTask;
    }
}
