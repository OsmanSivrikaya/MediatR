namespace WorkService.Domain.Works;

public sealed class WorkTask
{
    public Guid Id { get; private set; }
    public Guid CreatedBy { get; private set; }
    //public TaskStatus Status { get; private set; }

    private WorkTask() { }

    public static WorkTask Create(Guid creatorId)
    {
        return new WorkTask
        {
            Id = Guid.NewGuid(),
            CreatedBy = creatorId,
            //Status = TaskStatus.Created
        };
    }

    public void MarkFailed()
    {
        //Status = TaskStatus.Failed;
    }
}