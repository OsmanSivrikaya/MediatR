using MediatR;
using SharedKernel;
using UserService.Applitacion.Abstractions;
using UserService.Applitacion.Common.Errors;

namespace UserService.Applitacion.Auth.ValidateUser;

/// <summary>
/// Kullanıcı doğrulama iş kuralı
/// </summary>
public sealed class ValidateUserCommandHandler
    : IRequestHandler<ValidateUserCommand, Result<ValidateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ValidateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<ValidateUserResponse>> Handle(
        ValidateUserCommand request,
        CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);

        if (user is null)
            return Result<ValidateUserResponse>.Failure(
                UserErrors.InvalidCredentials);

        if (!user.IsActive())
            return Result<ValidateUserResponse>.Failure(
                UserErrors.UserLocked);

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<ValidateUserResponse>.Failure(
                UserErrors.InvalidCredentials);

        return Result<ValidateUserResponse>.Success(
            new ValidateUserResponse(
                user.Id,
                user.Email,
                [user.Role.ToString()]
            ));
    }
}