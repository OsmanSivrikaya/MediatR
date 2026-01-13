namespace AuthService.Application.Abstractions.UserGateway;

public interface IUserAuthGateway
{
    Task<UserAuthResult?> ValidateUserCredentialsAsync(string email, string password, CancellationToken ct);
}

public sealed record UserAuthResult(Guid UserId, string Email, IReadOnlyList<string> Roles, bool IsActive);