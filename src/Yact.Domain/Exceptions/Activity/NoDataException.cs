namespace Yact.Domain.Exceptions.Activity;

public class NoDataException : Exception
{
    public NoDataException() { }
    public NoDataException(string message) : base(message) { }
}
