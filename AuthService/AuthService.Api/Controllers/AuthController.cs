using AuthService.Api.Common;
using AuthService.Application.Auth.ClientCredentials;
using AuthService.Application.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

/// <summary>
/// Kimlik doğrulama (authentication) ile ilgili
/// HTTP endpoint'lerini barındıran API controller'ıdır.
///
/// Bu controller:
/// - Login (user authentication) akışını
/// - Client Credentials (service-to-service authentication) akışını
/// MediatR üzerinden Application katmanına iletir
///
/// İş kuralı veya teknik detay içermez.
/// HTTP → Application geçişinden sorumludur.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Authentication use-case'lerini tetiklemek için
    /// MediatR bağımlılığını alır.
    /// </summary>
    /// <param name="mediator">
    /// Application katmanındaki command handler'lara
    /// istekleri ileten MediatR bileşeni.
    /// </param>
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Kullanıcı adı ve şifre bilgileri ile
    /// kullanıcıyı doğrulayan login endpoint'idir.
    ///
    /// Bu endpoint:
    /// - LoginCommand'i Application katmanına gönderir
    /// - Dönen <see cref="SharedKernel.Result{T}"/> nesnesini
    ///   HTTP response'a dönüştürür
    /// </summary>
    /// <param name="cmd">
    /// Kullanıcının e-posta ve şifre bilgilerini içeren login komutu.
    /// </param>
    /// <param name="ct">
    /// Asenkron işlem için kullanılan cancellation token.
    /// </param>
    /// <returns>
    /// Kimlik doğrulama sonucuna göre üretilmiş HTTP response.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
    {
        return (await _mediator.Send(cmd, ct)).ToActionResult();
    }

    /// <summary>
    /// Client Credentials (service-to-service) akışı kapsamında
    /// servisler için JWT access token üreten endpoint'tir.
    ///
    /// Bu endpoint:
    /// - ClientCredentialsTokenCommand'i Application katmanına gönderir
    /// - Doğrulama ve token üretim sürecini Application katmanına bırakır
    /// </summary>
    /// <param name="command">
    /// ClientId ve ClientSecret bilgilerini içeren token talep komutu.
    /// </param>
    /// <param name="ct">
    /// Asenkron işlem için kullanılan cancellation token.
    /// </param>
    /// <returns>
    /// Service-to-service authentication için üretilmiş JWT token
    /// veya uygun hata response'u.
    /// </returns>
    [HttpPost("client-token")]
    public async Task<IActionResult> Token([FromBody] ClientCredentialsTokenCommand command, CancellationToken ct)
    {
        return (await _mediator.Send(command, ct)).ToActionResult();
    }
}
