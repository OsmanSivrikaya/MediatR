namespace UserService.Domain.Enums;

/// <summary>
/// Kullanıcının sistemdeki durumunu belirtir
/// </summary>
public enum UserStatus
{
    Passive = 0,
    Active = 1,
    Locked = 2
}