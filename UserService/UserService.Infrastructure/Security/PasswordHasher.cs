using UserService.Applitacion.Abstractions;

namespace UserService.Infrastructure.Security;

/// <summary>
/// BCrypt tabanlı şifre doğrulama
/// </summary>
internal sealed class PasswordHasher : IPasswordHasher
{
    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}