using MediatR;
using SharedKernel;

namespace UserService.Applitacion.Features.Auth.ValidateUser;

/// <summary>
/// Kullanıcı kimlik bilgilerini doğrulamak için kullanılır
/// </summary>
public sealed record ValidateUserRequest(
    string Email,
    string Password
) : IRequest<Result<ValidateUserResponse>>;