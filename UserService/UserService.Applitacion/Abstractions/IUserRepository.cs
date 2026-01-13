using UserService.Domain.Users;

namespace UserService.Applitacion.Abstractions;

/// <summary>
/// Kullanıcı verisine erişim kontratı
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
}