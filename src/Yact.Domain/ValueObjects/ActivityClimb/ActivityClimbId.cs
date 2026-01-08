using Yact.Domain.Primitives;

namespace Yact.Domain.ValueObjects.ActivityClimb;

public record ActivityClimbId (Guid Value) : ValueObjectId<ActivityClimbId>(Value);
