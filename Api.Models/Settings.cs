namespace Api.Models;

public class Settings
{
    public string RedisConnectionString { get; set; } = string.Empty;
    public int TokenExpirationInMinutes { get; set; }
    public string Secret { get; set; } = string.Empty;
    public Dictionary<string, string> MyAccessCredentials { get; set; } = new Dictionary<string, string>();
}
