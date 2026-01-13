using MediatR;
using SharedKernel;

namespace UserService.Applitacion.Auth.ValidateUser;

/// <summary>
/// Kullanıcı kimlik bilgilerini doğrulamak için kullanılır
/// </summary>
public sealed record ValidateUserCommand(
    string Email,
    string Password
) : IRequest<Result<ValidateUserResponse>>;