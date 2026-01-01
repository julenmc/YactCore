namespace Yact.Domain.ValueObjects.Activity;

public record ActivityId(int Value)
{
    public static ActivityId NewId() => new(0);
    public static ActivityId From(int value)
    {
        if (value <= 0) throw new ArgumentException("ID must be positive");
        return new ActivityId(value);
    }
}
