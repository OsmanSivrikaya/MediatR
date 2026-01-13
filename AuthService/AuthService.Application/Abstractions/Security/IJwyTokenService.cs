namespace AuthService.Application.Abstractions.Security;

/// <summary>
/// JWT (JSON Web Token) üretiminden sorumlu olan soyut servis sözleşmesidir.
/// 
/// Bu interface:
/// - Token'ın NASIL üretildiğini bilmez
/// - İmzalama algoritması, key yönetimi ve claim detaylarını içermez
/// 
/// Sadece hangi tür token'ların üretilebileceğini tanımlar.
/// Implementasyonu Infrastructure katmanında yapılır.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Kullanıcıyı temsil eden bir JWT üretir.
    /// 
    /// Bu token:
    /// - Client → API isteklerinde kullanılır
    /// - Kullanıcı kimliğini (identity) temsil eder
    /// - Authorization süreçlerinde rol bilgisi taşır
    /// </summary>
    /// <param name="userId">
    /// Sistemdeki kullanıcıya ait benzersiz kimlik (UserId).
    /// Token'ın <c>sub</c> (subject) claim'i olarak kullanılır.
    /// </param>
    /// <param name="email">
    /// Kullanıcının e-posta adresi.
    /// Token içerisinde bilgi amaçlı claim olarak yer alır.
    /// </param>
    /// <param name="roles">
    /// Kullanıcının sahip olduğu roller.
    /// Authorization kontrollerinde kullanılmak üzere token içerisine eklenir.
    /// </param>
    /// <returns>
    /// Üretilen access token ve geçerlilik süresini içeren <see cref="JwtTokenResult"/>.
    /// </returns>
    JwtTokenResult CreateUserToken(Guid userId, string email, IReadOnlyList<string> roles);
    /// <summary>
    /// Servisler arası (service-to-service) iletişimde kullanılmak üzere bir JWT üretir.
    /// 
    /// Bu token:
    /// - Kullanıcıyı değil, servisi temsil eder
    /// - Client Credentials akışında kullanılır
    /// - Background job veya internal API çağrıları için üretilir
    /// </summary>
    /// <param name="clientId">
    /// Token talep eden servisin benzersiz kimliği.
    /// Token'ın <c>sub</c> (subject) claim'i olarak kullanılır.
    /// </param>
    /// <returns>
    /// Üretilen service token ve geçerlilik süresini içeren <see cref="JwtTokenResult"/>.
    /// </returns>
    JwtTokenResult CreateServiceToken(string clientId);
}