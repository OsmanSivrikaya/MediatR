namespace UserService.Applitacion.Abstractions;

/// <summary>
/// Şifre doğrulama soyutlaması
/// </summary>
public interface IPasswordHasher
{
    bool Verify(string password, string passwordHash);
}