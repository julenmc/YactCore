namespace Yact.Domain.ValueObjects.Activity.Records;

public record RecordData
{
    public DateTime Timestamp { get; init; }
    public Coordinates Coordinates { get; init; }
    public Performance Performance { get; init; }
    public float? DistanceMeters { get; init; }
    public SmoothedAltitude SmoothedAltitude { get; init; }
}
