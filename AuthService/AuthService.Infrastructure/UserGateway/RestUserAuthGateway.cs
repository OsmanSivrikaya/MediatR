using System.Net;
using System.Net.Http.Json;
using AuthService.Application.Abstractions.UserGateway;

namespace AuthService.Infrastructure.UserGateway;

/// <summary>
/// UserService ile REST üzerinden haberleşerek
/// kullanıcı kimlik bilgilerini doğrulayan altyapı bileşenidir.
///
/// Bu sınıf:
/// - Kullanıcı doğrulama işleminin NASIL yapıldığını bilir
/// - HTTP, status code ve serialization detaylarını içerir
/// - Application katmanına sadece doğrulama sonucunu döner
///
/// İş kuralı içermez, sadece teknik iletişimden sorumludur.
/// </summary>
internal sealed class RestUserAuthGateway : IUserAuthGateway
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// UserService ile iletişim kurmak için kullanılacak HttpClient örneğini alır.
    ///
    /// Bu HttpClient:
    /// - Infrastructure katmanında yapılandırılır
    /// - BaseAddress ve timeout gibi ayarları DI üzerinden alır
    /// </summary>
    /// <param name="httpClient">
    /// UserService'e istek atmak için yapılandırılmış HttpClient.
    /// </param>
    public RestUserAuthGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Kullanıcıya ait kimlik bilgilerini UserService üzerinden doğrular.
    ///
    /// Bu metod:
    /// - E-posta ve şifre bilgisini UserService'e gönderir
    /// - Geçersiz kullanıcı veya hatalı şifre durumunda <c>null</c> döner
    /// - Beklenmeyen durumlarda exception fırlatır
    ///
    /// HTTP ve status code detayları bu katmanda tutulur,
    /// Application katmanı bu detaylardan tamamen izole edilir.
    /// </summary>
    /// <param name="email">
    /// Doğrulanacak kullanıcının e-posta adresi.
    /// </param>
    /// <param name="password">
    /// Kullanıcının girdiği düz şifre.
    /// </param>
    /// <param name="ct">
    /// Asenkron işlem için kullanılan cancellation token.
    /// </param>
    /// <returns>
    /// Kimlik doğrulaması başarılıysa <see cref="UserAuthResult"/> döner.
    /// Kullanıcı bulunamaz veya şifre hatalıysa <c>null</c> döner.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// UserService tarafından beklenmeyen bir HTTP durum kodu dönüldüğünde fırlatılır.
    /// </exception>
    public async Task<UserAuthResult?> ValidateUserCredentialsAsync(
        string email,
        string password,
        CancellationToken ct)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "internal/auth/validate",
            new ValidateUserRequest(email, password),
            ct);

        // Kullanıcı yok / şifre hatalı
        if (response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.BadRequest)
        {
            return null;
        }

        // Beklenmeyen hata
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"UserService auth doğrulama hatası. StatusCode: {response.StatusCode}");
        }

        return await response.Content.ReadFromJsonAsync<UserAuthResult>(
            cancellationToken: ct);
    }

    /// <summary>
    /// UserService'e gönderilen kullanıcı doğrulama isteğini temsil eden
    /// internal request modelidir.
    ///
    /// Bu record:
    /// - Sadece HTTP isteği için kullanılır
    /// - Application veya Domain tarafından bilinmez
    /// - Infrastructure detaylarının dışarı sızmasını engeller
    /// </summary>
    private sealed record ValidateUserRequest(string Email, string Password);
}