using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Events;

public record ClimbCreatedEvent (ClimbId id) : IDomainEvent;