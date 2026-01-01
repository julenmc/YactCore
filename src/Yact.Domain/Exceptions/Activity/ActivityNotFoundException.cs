namespace Yact.Domain.Exceptions.Activity;

public class ActivityNotFoundException : Exception
{
    public ActivityNotFoundException() { }
    public ActivityNotFoundException(int id) : base($"ID: {id}") { }
}
