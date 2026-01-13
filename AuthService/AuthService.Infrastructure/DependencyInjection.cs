using AuthService.Application.Abstractions;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.UserGateway;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.UserGateway;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure;

/// <summary>
/// Infrastructure katmanına ait bağımlılıkların
/// Dependency Injection container'a eklendiği merkezi konfigürasyon sınıfıdır.
///
/// Bu sınıf:
/// - Infrastructure katmanının dış dünyaya açılan TEK noktasıdır
/// - Application tarafından tanımlanan interface'lerin
///   hangi somut implementasyonlarla karşılanacağını belirler
/// - Configuration ve framework bağımlılıklarını izole eder
///
/// API (Composition Root) bu metodu çağırarak
/// Infrastructure katmanını sisteme dahil eder.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure katmanına ait tüm servisleri
    /// IServiceCollection içerisine kaydeder.
    ///
    /// Bu metod:
    /// - JWT üretimi ile ilgili altyapı bileşenlerini
    /// - UserService ile REST üzerinden haberleşen gateway'i
    /// - Configuration tabanlı ayarları
    /// DI container'a ekler
    /// </summary>
    /// <param name="services">
    /// Uygulamanın Dependency Injection container'ı.
    /// </param>
    /// <param name="config">
    /// appsettings.json ve environment bazlı ayarları içeren
    /// uygulama konfigürasyonu.
    /// </param>
    /// <returns>
    /// Zincirleme kullanım için aynı <see cref="IServiceCollection"/> örneğini döner.
    /// </returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // JWT ile ilgili ayarların strongly-typed bir şekilde
        // JwtOptions nesnesine bind edilmesini sağlar.
        //
        // Configuration okuma işlemi Infrastructure katmanında yapılır,
        // Application katmanı bu detaydan tamamen izole edilir.
        services.Configure<JwtOptions>(config.GetSection("Jwt"));

        // JWT üretiminden sorumlu altyapı servisini kaydeder.
        //
        // IJwtTokenService:
        // - Application katmanında tanımlanmış bir abstraction'dır
        // JwtTokenService:
        // - Bu abstraction'ın Infrastructure katmanındaki somut implementasyonudur
        //
        // Singleton olarak kaydedilmesinin sebebi:
        // - Stateless olması
        // - Thread-safe şekilde çalışması
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // UserService ile REST üzerinden haberleşen
        // IUserAuthGateway implementasyonunu kaydeder.
        //
        // HttpClientFactory kullanımı sayesinde:
        // - Connection management
        // - DNS refresh
        // - Resilience (ileride Polly vb.)
        // desteklenir.
        services.AddHttpClient<IUserAuthGateway, RestUserAuthGateway>(client =>
        {
            // UserService'in base URL bilgisi
            // application configuration üzerinden alınır.
            //
            // Bu bilgi Infrastructure detayına aittir
            // ve Application katmanına sızdırılmaz.
            client.BaseAddress = new Uri(config["Services:UserService:BaseUrl"]!);
        });

        services.AddScoped<IClientCredentialValidator, AppSettingsClientCredentialValidator>();

        return services;
    }
}
