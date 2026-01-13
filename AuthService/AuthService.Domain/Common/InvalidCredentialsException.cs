using SharedKernel;

namespace AuthService.Domain.Common;

public sealed class InvalidCredentialsException
    : DomainException
{
    public InvalidCredentialsException()
        : base("Kullanıcı adı veya şifre hatalı.")
    {
    }
}