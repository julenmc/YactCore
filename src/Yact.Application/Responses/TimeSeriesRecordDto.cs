namespace Yact.Application.Responses;

public class TimeSeriesRecordDto<T>
{
    public DateTime Timestamp { get; set; }
    public double? DistanceMeters { get; set; }
    public T? Value { get; set; }
}

public class TimeSeriesResponseDto<T>
{
    public required Guid ActivityId { get; set; }
    public required string Metric { get; set; }
    public required string Unit { get; set; }
    public List<TimeSeriesRecordDto<T>> Records { get; set; } = new();
}
