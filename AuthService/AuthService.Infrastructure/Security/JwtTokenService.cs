using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AuthService.Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Security;

/// <summary>
/// JWT (JSON Web Token) üretiminden sorumlu olan
/// Infrastructure katmanı implementasyonudur.
///
/// Bu sınıf:
/// - Token'ın NASIL üretileceğini bilir
/// - İmzalama algoritması (RS256)
/// - Private key okuma
/// - Claim oluşturma
/// gibi tüm teknik detayları içerir.
///
/// Application katmanı bu sınıfı TANIMAZ;
/// sadece <see cref="IJwtTokenService"/> interface'i üzerinden haberleşir.
/// </summary>
internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opt;
    private readonly RsaSecurityKey _signingKey;
    private readonly SigningCredentials _creds;

    /// <summary>
    /// JWT üretimi için gerekli olan konfigürasyon ve
    /// kriptografik imzalama altyapısını hazırlar.
    ///
    /// Bu constructor:
    /// - JwtOptions ayarlarını <see cref="IOptions{T}"/> üzerinden alır
    /// - RSA private key'i dosya sisteminden okur
    /// - RS256 imzalama için gerekli SigningCredentials'ı oluşturur
    ///
    /// Bu işlemler maliyetli olduğu için constructor'da
    /// bir kez yapılır ve servis singleton olarak kullanılır.
    /// </summary>
    /// <param name="options">
    /// JWT üretimi ile ilgili ayarları içeren strongly-typed options nesnesi.
    /// </param>
    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _opt = options.Value;

        // RSA private key dosyasını okur
        var privatePem = File.ReadAllText(_opt.PrivateKeyPath);

        // RSA algoritmasını private key ile initialize eder
        var rsa = RSA.Create();
        rsa.ImportFromPem(privatePem.ToCharArray());

        // İmzalama anahtarı ve signing credentials oluşturulur
        _signingKey = new RsaSecurityKey(rsa);
        _creds = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256);
    }

    /// <summary>
    /// Kullanıcıyı temsil eden bir JWT access token üretir.
    ///
    /// Bu token:
    /// - Client → API çağrılarında kullanılır
    /// - Kullanıcı kimliğini (identity) temsil eder
    /// - Rol bilgilerini authorization süreçleri için taşır
    /// </summary>
    /// <param name="userId">
    /// Sistemdeki kullanıcıya ait benzersiz kimlik.
    /// Token'ın <c>sub</c> (subject) claim'i olarak kullanılır.
    /// </param>
    /// <param name="email">
    /// Kullanıcının e-posta adresi.
    /// Token içerisine bilgi amaçlı claim olarak eklenir.
    /// </param>
    /// <param name="roles">
    /// Kullanıcının sahip olduğu roller.
    /// Authorization kontrolleri için <see cref="ClaimTypes.Role"/> olarak eklenir.
    /// </param>
    /// <returns>
    /// Üretilen access token ve sona erme zamanını içeren
    /// <see cref="JwtTokenResult"/>.
    /// </returns>
    public JwtTokenResult CreateUserToken(
        Guid userId,
        string email,
        IReadOnlyList<string> roles)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_opt.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Kullanıcının rollerini token'a ekler
        if (roles.Any())
        {
            foreach (var r in roles.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }
        }

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: _creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new JwtTokenResult(token, expires);
    }

    /// <summary>
    /// Servisler arası (service-to-service) iletişimde kullanılmak üzere
    /// bir JWT access token üretir.
    ///
    /// Bu token:
    /// - Kullanıcıyı değil, servisi temsil eder
    /// - Client Credentials akışında kullanılır
    /// - Internal API çağrıları ve background job'lar için üretilir
    /// </summary>
    /// <param name="clientId">
    /// Token talep eden servisin benzersiz kimliği.
    /// Token'ın <c>sub</c> (subject) claim'i olarak kullanılır.
    /// </param>
    /// <returns>
    /// Üretilen service token ve sona erme zamanını içeren
    /// <see cref="JwtTokenResult"/>.
    /// </returns>
    public JwtTokenResult CreateServiceToken(string clientId)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_opt.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, clientId),
            new("token_type", "service"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: _creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new JwtTokenResult(token, expires);
    }
}