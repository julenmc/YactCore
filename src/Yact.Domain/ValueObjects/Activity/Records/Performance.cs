namespace Yact.Domain.ValueObjects.Activity.Records;

public record Performance(
    int? HeartRate,
    float? Power,
    int? Cadence,
    float? SpeedMps
);
