using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Common;
using UserService.Applitacion.Features.Auth.ValidateUser;

namespace UserService.Api.Controllers;

/// <summary>
/// AuthService tarafından kullanılan internal endpointler
/// </summary>
[ApiController]
[Route("internal/auth")]
public sealed class InternalAuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public InternalAuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ValidateUserRequest request, CancellationToken ct)
    {
        return (await _mediator.Send(request, ct)).ToActionResult();
    }
}
