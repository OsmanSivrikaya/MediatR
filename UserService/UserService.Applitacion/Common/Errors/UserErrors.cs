using SharedKernel;

namespace UserService.Applitacion.Common.Errors;

/// <summary>
/// Kullanıcı doğrulama hataları
/// </summary>
public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("USER_001", "E-posta veya şifre hatalı.");

    public static readonly Error UserLocked =
        new("USER_002", "Kullanıcı hesabı kilitlenmiş.");
}