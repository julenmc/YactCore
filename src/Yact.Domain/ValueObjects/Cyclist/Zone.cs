namespace Yact.Domain.ValueObjects.Cyclist;

public record Zone
{
    public int LowLimit { get; private set; }
    public int HighLimit { get; private set; }

    public static Zone Create(int lowLimit, int highLimit)
    {
        if (lowLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(lowLimit));

        if (lowLimit >= highLimit)
            throw new ArgumentException($"Low limit ({lowLimit}) cannot be higher than high limit ({highLimit})");

        return new Zone
        {
            LowLimit = lowLimit,
            HighLimit = highLimit
        };
    }
};
