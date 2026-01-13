using SharedKernel;

namespace AuthService.Application.Common.Errors;

public static class AuthErrors
{
    public static Error InvalidCredentials =>
        new(
            Code: "AUTH_INVALID_CREDENTIALS",
            Message: "E-posta veya şifre hatalı."
        );

    public static Error UserLocked =>
        new(
            Code: "AUTH_USER_LOCKED",
            Message: "Kullanıcı hesabı kilitli."
        );
    
    /// <summary>
    /// Client Credentials (service-to-service) akışında
    /// gönderilen ClientId veya ClientSecret bilgisinin
    /// geçersiz olduğunu ifade eden hata.
    /// </summary>
    public static Error InvalidClientCredentials =>
        new(
            Code: "AUTH_INVALID_CLIENT_CREDENTIALS",
            Message: "Client kimlik bilgileri doğrulanamadı."
        );
}