using Yact.Domain.Primitives;

namespace Yact.Domain.ValueObjects.Cyclist;

public record CyclistFitnessId (Guid Value) : ValueObjectId<CyclistFitnessId>(Value);
