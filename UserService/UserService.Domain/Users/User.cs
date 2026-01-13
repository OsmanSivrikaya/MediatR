namespace UserService.Domain.Users;

/// <summary>
/// Sistem kullanıcısını temsil eden domain entity
/// </summary>
public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserStatus Status { get; private set; }
    public UserRole Role { get; private set; }

    private User() { }

    public User(Guid id, string email, string passwordHash)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Status = UserStatus.Active;
        Role = UserRole.User;
    }

    public void Lock()
    {
        Status = UserStatus.Locked;
    }

    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }
}
