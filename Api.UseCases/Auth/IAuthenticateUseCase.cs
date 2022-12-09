using Api.Models;

namespace Api.UseCases;

public interface IAuthenticateUseCase
{
    public Task<Token?> GetTokenAsync(AccessCredentials accessCredentials);
}
