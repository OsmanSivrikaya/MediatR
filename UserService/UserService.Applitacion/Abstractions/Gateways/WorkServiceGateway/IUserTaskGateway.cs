namespace UserService.Applitacion.Abstractions.Gateways.WorkServiceGateway;

public interface IUserTaskGateway
{
    Task AssignAsync(Guid userId, Guid taskId, CancellationToken ct);
    Task CancelAsync(Guid taskId, CancellationToken ct);
}
