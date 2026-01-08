using Yact.Domain.Primitives;

namespace Yact.Domain.ValueObjects.Climb;

public record ClimbId (Guid Value) : ValueObjectId<ClimbId>(Value);
