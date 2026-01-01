using Yact.Domain.Events;

namespace Yact.Domain.Entities;

public interface IEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
