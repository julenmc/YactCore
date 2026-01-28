using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Events;

public record CyclistLoadedEvent (CyclistId CyclistId) : IDomainEvent;
