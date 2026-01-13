using AuthService.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace AuthService.Api.Common;


/// <summary>
/// Application katmanından dönen <see cref="Result{T}"/> nesnelerini
/// HTTP protokolüne uygun <see cref="IActionResult"/> çıktısına
/// dönüştürmek için kullanılan yardımcı extension metodlarını içerir.
///
/// Bu sınıf:
/// - Controller'lardaki boilerplate kodu ortadan kaldırır
/// - Result → HTTP eşlemesini merkezi bir noktada toplar
/// - API katmanının Application ve Domain'den izole kalmasını sağlar
///
/// Framework bağımlılığı yalnızca API katmanında tutulur.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Bir <see cref="Result{T}"/> nesnesini,
    /// HTTP standardına uygun bir <see cref="IActionResult"/> çıktısına dönüştürür.
    ///
    /// Bu metod:
    /// - Başarılı sonuçlar için <see cref="OkObjectResult"/> döner
    /// - Başarısız sonuçlarda hata koduna göre uygun HTTP response üretir
    /// - Error.Code üzerinden merkezi bir HTTP eşlemesi yapılmasını sağlar
    ///
    /// Böylece Controller'lar yalnızca use-case tetikler,
    /// HTTP detayları tek bir yerde yönetilir.
    /// </summary>
    /// <typeparam name="T">
    /// Başarılı sonuç durumunda döndürülecek response modelinin tipi.
    /// </typeparam>
    /// <param name="result">
    /// Application katmanından dönen use-case sonucu.
    /// </param>
    /// <returns>
    /// Result durumuna göre oluşturulmuş bir <see cref="IActionResult"/>.
    /// </returns>
    public static IActionResult ToActionResult<T>(
        this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result.Error!.Code switch
        {
            var code when code == AuthErrors.InvalidCredentials.Code
                => new UnauthorizedObjectResult(result.Error),

            var code when code == AuthErrors.UserLocked.Code
                => new ForbidResult(),

            _ => new BadRequestObjectResult(result.Error)
        };
    }
}