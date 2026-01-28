using Yact.Domain.Primitives;

namespace Yact.Domain.ValueObjects.Cyclist;

public record CyclistId(Guid Value) : ValueObjectId<CyclistId>(Value);
