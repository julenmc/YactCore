namespace Yact.Domain.Exceptions.Cyclist;

public class NoCyclistException : Exception
{
    public NoCyclistException() { }
    public NoCyclistException(Guid guid) : base($"{guid}") { }
}
