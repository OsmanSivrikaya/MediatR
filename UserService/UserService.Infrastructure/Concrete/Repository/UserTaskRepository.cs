using Microsoft.EntityFrameworkCore;
using UserService.Applitacion.Abstractions.Repository;
using UserService.Domain.Entity;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Concrete.Repository;

internal sealed class UserTaskRepository : IUserTaskRepository
{
    private readonly UserServiceDbContext _db;

    public UserTaskRepository(UserServiceDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(UserTask entity, CancellationToken ct)
    {
        await _db.UserTasks.AddAsync(entity, ct);
    }

    public async Task<UserTask?> GetByTaskIdAsync(
        Guid taskId,
        CancellationToken ct)
    {
        return await _db.UserTasks
            .FirstOrDefaultAsync(x => x.TaskId == taskId, ct);
    }

    public void Remove(UserTask entity)
    {
        _db.UserTasks.Remove(entity);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}