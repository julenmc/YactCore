using Yact.Domain.ValueObjects.Activity;

namespace Yact.Domain.ValueObjects.Climb;

public record ClimbRecord (
    ActivityId ActivityId,
    TimeSpan Time,
    DateTime Date);
