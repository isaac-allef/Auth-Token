using Api.Models;

namespace Api.Services;

public interface ITokenService
{
    public Token Generate(string username, int expiresInMinutes);
}
