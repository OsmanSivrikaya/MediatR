using MediatR;
using SharedKernel;

namespace AuthService.Application.Auth.ClientCredentials;

public sealed record ClientCredentialsTokenCommand(string ClientId, string ClientSecret) : IRequest<Result<ClientTokenResponse>>;

public sealed record ClientTokenResponse(string AccessToken, DateTime ExpiresAtUtc);