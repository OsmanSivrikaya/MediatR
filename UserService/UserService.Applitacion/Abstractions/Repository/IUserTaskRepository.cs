using UserService.Domain.Entity;

namespace UserService.Applitacion.Abstractions.Repository;

public interface IUserTaskRepository
{
    Task AddAsync(UserTask entity, CancellationToken ct);
    Task<UserTask?> GetByTaskIdAsync(Guid taskId, CancellationToken ct);
    void Remove(UserTask entity);
    Task SaveChangesAsync(CancellationToken ct);
}