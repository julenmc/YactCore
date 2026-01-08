namespace Yact.Application.Responses;

public class ActivityClimbDto
{
    public Guid Id { get; set; }
    public ClimbDto? Data { get; set; }
    public ActivityDto? Activity { get; set; }
}
