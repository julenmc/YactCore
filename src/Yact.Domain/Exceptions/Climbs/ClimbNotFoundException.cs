using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Exceptions.Climbs;

public class ClimbNotFoundException : Exception
{
    public ClimbNotFoundException(Guid id) : base($"{id}") { }
}
