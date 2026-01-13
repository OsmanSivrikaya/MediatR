namespace AuthService.Infrastructure.Security;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string PrivateKeyPath { get; set; } = default!;
    public int AccessTokenMinutes { get; set; } = 30;
}