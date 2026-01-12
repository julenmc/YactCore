namespace Yact.Domain.ValueObjects.Cyclist;

public record Size
{
    public ushort HeightCm { get; }
    public float WeightKg { get; }

    public Size(ushort heightCm, float weightKg)
    {
        HeightCm = heightCm;
        WeightKg = weightKg;
    }
}
