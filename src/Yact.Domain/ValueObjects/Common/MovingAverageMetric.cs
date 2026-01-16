namespace Yact.Domain.ValueObjects.Common;

public record MovingAverageMetric
{
    public DateTime Timestamp { get; set; }
    public float Average { get; set; }
    public float Deviation { get; set; }
    public float CoefficientOfVariation => Deviation / Average;
    internal float MaxMinDelta { get; set; }
    internal float RangePercent => MaxMinDelta / Average;
    internal float DeviationFromReference { get; set; }
}
