using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Common;
using UserService.Applitacion.Features.UserAssignments.AssignUserTask;

namespace UserService.Api.Controllers;

[ApiController]
[Route("internal/user-tasks")]
public sealed class UserAssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserAssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign(
        AssignUserTaskRequest req,
        CancellationToken ct)
        => (await _mediator.Send(req, ct)).ToActionResult();

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel(
        CancelUserTaskCommand command,
        CancellationToken ct)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }
}
