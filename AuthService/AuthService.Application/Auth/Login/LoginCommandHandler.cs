using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.UserGateway;
using AuthService.Application.Common.Errors;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Auth.Login;

internal sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserAuthGateway _userAuthGateway;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUserAuthGateway userAuthGateway,
        IJwtTokenService jwtTokenService)
    {
        _userAuthGateway = userAuthGateway;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken ct)
    {
        var user = await _userAuthGateway.ValidateUserCredentialsAsync(
            request.Email,
            request.Password,
            ct);

        if (user is null)
        {
            return Result<LoginResponse>.Failure(
                AuthErrors.InvalidCredentials
            );
        }

        var token = _jwtTokenService.CreateUserToken(
            user.UserId,
            user.Email,
            user.Roles);

        return Result<LoginResponse>.Success(
            new LoginResponse(
                token.AccessToken,
                token.ExpiresAtUtc
            ));
    }
}