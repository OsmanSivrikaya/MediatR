using AuthService.Application.Abstractions;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Common.Errors;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Auth.ClientCredentials;

/// <summary>
/// Client Credentials (service-to-service) akışı kapsamında
/// servisler için JWT access token üreten command handler'dır.
///
/// Bu handler:
/// - Client kimlik bilgilerini doğrular
/// - Doğrulama başarılıysa service token üretir
/// - Sonucu <see cref="Result{T}"/> olarak döner
///
/// İş kuralı ihlali değil, use-case sonucu üretir.
/// Bu nedenle exception yerine Result kullanır.
/// </summary>
public sealed class ClientCredentialsTokenCommandHandler
    : IRequestHandler<ClientCredentialsTokenCommand, Result<ClientTokenResponse>>
{
    private readonly IClientCredentialValidator _validator;
    private readonly IJwtTokenService _jwt;

    /// <summary>
    /// Client credential doğrulama ve token üretimi için
    /// gerekli olan bağımlılıkları alır.
    /// </summary>
    /// <param name="validator">
    /// ClientId / ClientSecret doğrulamasından sorumlu abstraction.
    /// </param>
    /// <param name="jwt">
    /// Service token üretiminden sorumlu JWT altyapı servisi.
    /// </param>
    public ClientCredentialsTokenCommandHandler(
        IClientCredentialValidator validator,
        IJwtTokenService jwt)
    {
        _validator = validator;
        _jwt = jwt;
    }

    /// <summary>
    /// Client Credentials token talebini işler.
    ///
    /// Akış:
    /// 1. Client kimlik bilgileri doğrulanır
    /// 2. Doğrulama başarısızsa <see cref="Result{T}.Failure"/> döner
    /// 3. Doğrulama başarılıysa service JWT token üretilir
    ///
    /// Bu metod:
    /// - Beklenen başarısızlıklarda exception fırlatmaz
    /// - Use-case sonucunu Result üzerinden bildirir
    /// </summary>
    public Task<Result<ClientTokenResponse>> Handle(
        ClientCredentialsTokenCommand request,
        CancellationToken ct)
    {
        // ❌ Client doğrulanamadı → beklenen başarısızlık
        if (!_validator.Validate(request.ClientId, request.ClientSecret))
        {
            return Task.FromResult(
                Result<ClientTokenResponse>.Failure(
                    AuthErrors.InvalidClientCredentials
                ));
        }

        // ✅ Service token üretimi
        var token = _jwt.CreateServiceToken(request.ClientId);

        return Task.FromResult(
            Result<ClientTokenResponse>.Success(
                new ClientTokenResponse(
                    token.AccessToken,
                    token.ExpiresAtUtc
                )
            ));
    }
}