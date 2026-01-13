using Microsoft.Extensions.DependencyInjection;

namespace UserService.Applitacion;

/// <summary>
/// Application katmanı servis kayıtları
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly));

        return services;
    }
}