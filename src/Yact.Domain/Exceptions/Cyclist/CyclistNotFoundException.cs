namespace Yact.Domain.Exceptions.Cyclist;

public class CyclistNotFoundException : Exception
{
    public CyclistNotFoundException() { }
    public CyclistNotFoundException(Guid guid) : base($"{guid}") { }
}
