namespace Yact.Domain.ValueObjects.Common;

public record MovingAverageMetric<T>
{
    public int Index { get; set; }
    public float Average { get; set; }
    public float Deviation { get; set; }
    public float CoefficientOfVariation => Deviation / Average;
    internal float MaxMinDelta { get; set; }
    internal float RangePercent => MaxMinDelta / Average;
    internal float DeviationFromReference { get; set; }

    public required T LastPoint { get; set; }
}
