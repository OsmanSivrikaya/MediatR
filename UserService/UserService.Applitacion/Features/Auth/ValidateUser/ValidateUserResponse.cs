namespace UserService.Applitacion.Features.Auth.ValidateUser;

/// <summary>
/// Doğrulanan kullanıcı bilgileri
/// </summary>
public sealed record ValidateUserResponse(
    Guid UserId,
    string Email,
    IReadOnlyList<string> Roles
);