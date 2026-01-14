using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entity;

namespace UserService.Infrastructure.Persistence;

public sealed class UserServiceDbContext : DbContext
{
    public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options)
        : base(options) { }

    public DbSet<UserTask> UserTasks => Set<UserTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserTask>(cfg =>
        {
            cfg.ToTable("user_tasks");

            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.UserId)
                .IsRequired();

            cfg.Property(x => x.TaskId)
                .IsRequired();

            cfg.HasIndex(x => x.TaskId)
                .IsUnique();
        });
    }
}