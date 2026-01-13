namespace Yact.Application.Responses;

public class ActivityClimbDto
{
    public Guid Id { get; set; }
    public ClimbResponse? Data { get; set; }
    public ActivityResponse? Activity { get; set; }
}
