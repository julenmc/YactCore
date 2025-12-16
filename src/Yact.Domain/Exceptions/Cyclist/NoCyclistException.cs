namespace Yact.Domain.Exceptions.Cyclist;

public class NoCyclistException : Exception
{
    public NoCyclistException() { }
    public NoCyclistException(string message) : base(message) { }
}
