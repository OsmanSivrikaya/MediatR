using MediatR;
using SharedKernel;

namespace UserService.Applitacion.Features.UserAssignments.AssignUserTask;

public sealed record AssignUserTaskRequest(
    Guid UserId,
    Guid TaskId
) : IRequest<Result>;