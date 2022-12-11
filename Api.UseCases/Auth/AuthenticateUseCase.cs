using Api.Models;
using Api.Services;
using Api.Services.Caching;

namespace Api.UseCases;

public class AuthenticateUseCase : IAuthenticateUseCase
{
    private readonly Settings _settings;
    private readonly ICachingService _cache;
    private readonly ILockingService _lockingService;
    private readonly ITokenService _tokenService;

    public AuthenticateUseCase(
        Settings settings, ICachingService cache, ILockingService lockingService, ITokenService tokenService)
    {
        _settings = settings;
        _cache = cache;
        _lockingService = lockingService;
        _tokenService = tokenService;
    }

    public async Task<Token?> GetTokenAsync(AccessCredentials accessCredentials)
    {
        if (!HasValidCredentials(accessCredentials))
        {
            return null;
        }

        var token = await _cache.GetAsync<Token?>(accessCredentials.Username);
        if (token is not null)
        {
            return token;
        }

        return await _lockingService.RunAsync(accessCredentials.Username, async () =>
        {
            var token = await _cache.GetAsync<Token?>(accessCredentials.Username);
            if (token is not null)
            {
                return token;
            }

            var newToken = _tokenService.Generate(accessCredentials.Username, _settings.TokenExpirationInMinutes);

            var expiration = newToken.ExpiresIn.Subtract(DateTime.UtcNow);
            await _cache.SetAsync<Token>(accessCredentials.Username, newToken, expiration);

            return newToken;
        });
    }

    private bool HasValidCredentials(AccessCredentials accessCredentials)
    {
        if (!_settings.MyAccessCredentials.TryGetValue(accessCredentials.Username, out var password)
            || password != accessCredentials.Password)
        {
            return false;
        }

        return true;
    }
}
