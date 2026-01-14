using MediatR;
using SharedKernel;
using UserService.Applitacion.Abstractions.Repository;
using UserService.Domain.Entity;

namespace UserService.Applitacion.Features.UserAssignments.AssignUserTask;

public sealed class AssignUserTaskHandler
    : IRequestHandler<AssignUserTaskRequest, Result>
{
    private readonly IUserTaskRepository _repository;

    public AssignUserTaskHandler(IUserTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        AssignUserTaskRequest request,
        CancellationToken ct)
    {
        var userTask = UserTask.Create(
            request.UserId,
            request.TaskId);

        await _repository.AddAsync(userTask, ct);
        await _repository.SaveChangesAsync(ct);

        // 🔥 TEST için bilinçli hata
        // throw new Exception("UserService Assign patladı");

        return Result.Success();
    }
}