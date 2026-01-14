using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Applitacion.Abstractions;
using UserService.Applitacion.Abstractions.Repository;
using UserService.Infrastructure.Concrete;
using UserService.Infrastructure.Concrete.Repository;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure;

/// <summary>
/// Infrastructure bağımlılıklarını ekler
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserServiceDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("UserServiceDb"));
        });

        services.AddScoped<IUserTaskRepository, UserTaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}