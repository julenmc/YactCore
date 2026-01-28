using Yact.Domain.Primitives;

namespace Yact.Domain.ValueObjects.Activity;

public record ActivityId(Guid Value) : ValueObjectId<ActivityId>(Value);
