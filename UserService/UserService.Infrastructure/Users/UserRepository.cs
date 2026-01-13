using UserService.Applitacion.Abstractions;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Users;

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