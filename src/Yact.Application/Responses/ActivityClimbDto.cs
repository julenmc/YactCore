namespace Yact.Application.Responses;

public class ActivityClimbDto
{
    public int Id { get; set; }
    public ClimbDto? Data { get; set; }
    public ActivityInfoDto? Activity { get; set; }
}
