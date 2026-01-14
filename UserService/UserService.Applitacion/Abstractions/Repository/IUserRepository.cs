using UserService.Domain.Entity;

namespace UserService.Applitacion.Abstractions.Repository;

/// <summary>
/// Kullanıcı verisine erişim kontratı
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
}