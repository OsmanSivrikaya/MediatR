namespace UserService.Domain.Entity;

public sealed class UserTask
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid TaskId { get; private set; }

    private UserTask() { }

    public static UserTask Create(Guid userId, Guid taskId)
    {
        return new UserTask
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TaskId = taskId
        };
    }
}
