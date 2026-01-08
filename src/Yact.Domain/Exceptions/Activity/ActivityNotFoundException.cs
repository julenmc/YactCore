namespace Yact.Domain.Exceptions.Activity;

public class ActivityNotFoundException : Exception
{
    public ActivityNotFoundException() { }
    public ActivityNotFoundException(Guid id) : base($"ID: {id}") { }
}
