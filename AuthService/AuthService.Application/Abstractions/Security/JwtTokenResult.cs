namespace AuthService.Application.Abstractions.Security;

/// <summary>
/// JWT üretimi sonucunda dönen erişim bilgilerini temsil eder.
/// 
/// Bu record:
/// - Token üretim sürecinin teknik detaylarını içermez
/// - Sadece tüketilmesi gereken çıktıyı taşır
/// - Application katmanı ile dış dünya (API) arasında veri transferi amacıyla kullanılır
/// </summary>
/// <param name="AccessToken">
/// Üretilen JWT access token.
/// Client veya servis tarafından Authorization header içerisinde kullanılır.
/// </param>
/// <param name="ExpiresAtUtc">
/// Token'ın UTC cinsinden sona erme tarihi.
/// Token geçerlilik kontrolü ve yenileme süreçlerinde kullanılır.
/// </param>
public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);
