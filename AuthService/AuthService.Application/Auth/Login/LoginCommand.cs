using MediatR;
using SharedKernel;

namespace AuthService.Application.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
