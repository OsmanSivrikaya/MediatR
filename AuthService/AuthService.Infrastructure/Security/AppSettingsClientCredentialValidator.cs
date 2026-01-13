using AuthService.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Security;

/// <summary>
/// Client Credentials (service-to-service) akışında,
/// client bilgilerini application configuration (appsettings)
/// üzerinden doğrulayan altyapı bileşenidir.
///
/// Bu sınıf:
/// - ClientId ve ClientSecret bilgisini configuration'dan okur
/// - Doğrulamanın NASIL yapıldığını bilir
/// - Teknik bir doğrulama mekanizması sağlar
///
/// İş kuralı içermez, sadece altyapı doğrulamasından sorumludur.
/// </summary>
internal sealed class AppSettingsClientCredentialValidator
    : IClientCredentialValidator
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Client credential bilgilerini okumak için
    /// uygulama konfigürasyonunu alır.
    ///
    /// Configuration erişimi Infrastructure katmanına aittir.
    /// Application katmanı bu detayı bilmez.
    /// </summary>
    /// <param name="configuration">
    /// appsettings.json ve environment bazlı ayarları içeren
    /// uygulama konfigürasyonu.
    /// </param>
    public AppSettingsClientCredentialValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Verilen client kimlik bilgilerini (ClientId / ClientSecret)
    /// configuration üzerinde tanımlı değerlerle karşılaştırarak doğrular.
    ///
    /// Bu metod:
    /// - Service-to-service authentication senaryolarında kullanılır
    /// - Geçerli client bilgileri için <c>true</c>
    /// - Geçersiz bilgiler için <c>false</c> döner
    ///
    /// Validation sonucu sadece teknik bir doğrulamadır;
    /// authorization veya iş kararı içermez.
    /// </summary>
    /// <param name="clientId">
    /// Token talep eden servisin client kimliği.
    /// </param>
    /// <param name="clientSecret">
    /// Token talep eden servisin gizli anahtarı.
    /// </param>
    /// <returns>
    /// Client bilgileri geçerliyse <c>true</c>,
    /// aksi halde <c>false</c>.
    /// </returns>
    public bool Validate(string clientId, string clientSecret)
    {
        var section = _configuration.GetSection("Clients:WorkService");

        return section["ClientId"] == clientId &&
               section["ClientSecret"] == clientSecret;
    }
}