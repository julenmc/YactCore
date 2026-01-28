namespace Yact.Domain.Exceptions.Cyclist;

public class FitnessNotFoundException : Exception
{
    public FitnessNotFoundException(Guid guid) : base($"{guid}") { }
}
