using UserService.Applitacion.Abstractions.Repository;
using UserService.Domain.Entity;

namespace UserService.Infrastructure.Concrete.Repository;

/// <summary>
/// Örnek kullanıcı repository (ileride EF Core olacak)
/// </summary>
internal sealed class UserRepository : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        if (email != "test@demo.com")
            return Task.FromResult<User?>(null);

        var user = new User(
            Guid.NewGuid(),
            email,
            BCrypt.Net.BCrypt.HashPassword("123456")
        );

        return Task.FromResult<User?>(user);
    }
}