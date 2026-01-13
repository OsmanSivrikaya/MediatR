using Microsoft.Extensions.DependencyInjection;
using UserService.Applitacion.Abstractions;
using UserService.Infrastructure.Security;
using UserService.Infrastructure.Users;

namespace UserService.Infrastructure;

/// <summary>
/// Infrastructure bağımlılıklarını ekler
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}