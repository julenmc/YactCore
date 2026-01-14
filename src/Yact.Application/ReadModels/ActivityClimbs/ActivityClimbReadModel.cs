using Yact.Application.ReadModels.Activities;
using Yact.Application.ReadModels.Climbs;

namespace Yact.Application.ReadModels.ActivityClimbs;

public record ActivityClimbReadModel
{
    public Guid Id { get; set; }
    public ClimbBasicReadModel? Data { get; set; }
    public ActivityBasicReadModel? Activity { get; set; }
}
