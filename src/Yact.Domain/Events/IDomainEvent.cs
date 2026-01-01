namespace Yact.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn => DateTime.UtcNow;
}
