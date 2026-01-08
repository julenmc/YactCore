using Yact.Domain.ValueObjects.ActivityClimb;

namespace Yact.Domain.ValueObjects.Climb;

public record ClimbRecord (
    ActivityClimbId ActivityClimbId,
    TimeSpan Time,
    DateTime Date);
