using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Events;

public record ActivityCreatedEvent(
    ActivityId ActivityId,
    CyclistId CyclistId) : IDomainEvent;
