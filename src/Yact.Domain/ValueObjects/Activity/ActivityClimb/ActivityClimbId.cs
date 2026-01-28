using Yact.Domain.Primitives;

namespace Yact.Domain.ValueObjects.Activity.ActivityClimb;

public record ActivityClimbId (Guid Value) : ValueObjectId<ActivityClimbId>(Value);
