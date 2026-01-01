namespace Yact.Domain.ValueObjects.Cyclist;

public record CyclistId(int Value)
{
    public static CyclistId NewId() => new(0);
    public static CyclistId From(int value)
    {
        if (value <= 0) throw new ArgumentException("ID must be positive");
        return new CyclistId(value);
    }
}
