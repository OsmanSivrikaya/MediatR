namespace AuthService.Application.Abstractions;

public interface IClientCredentialValidator
{
    bool Validate(string clientId, string clientSecret);
}