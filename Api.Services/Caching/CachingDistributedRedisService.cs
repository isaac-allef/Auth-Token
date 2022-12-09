using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Api.Services.Caching;

public class CachingDistributedRedisService : ICachingService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan DEFAULT_EXPIRATION = TimeSpan.FromMinutes(5);

    public CachingDistributedRedisService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<TValue?> GetAsync<TValue>(string key)
    {
        var value = await _cache.GetAsync(key);

        if (value is null)
        {
            return default(TValue);
        }

        return JsonSerializer.Deserialize<TValue>(value);
    }

    public Task SetAsync<TValue>(string key, TValue value)
        => SetAsync<TValue>(key, value, DEFAULT_EXPIRATION);

    public async Task SetAsync<TValue>(string key, TValue value, TimeSpan expiration)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var valueSerialized = JsonSerializer.Serialize(value);
        await _cache.SetAsync(key, Encoding.UTF8.GetBytes(valueSerialized), options);
    }
}
