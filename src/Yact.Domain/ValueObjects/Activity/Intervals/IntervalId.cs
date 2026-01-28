using Yact.Domain.Primitives;

namespace Yact.Domain.ValueObjects.Activity.Intervals;

public record IntervalId (Guid Value) : ValueObjectId<IntervalId>(Value);
