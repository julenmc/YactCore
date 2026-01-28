namespace Yact.Domain.Primitives;

public interface IDomainEvent
{
    DateTime OccurredOn => DateTime.UtcNow;
}
