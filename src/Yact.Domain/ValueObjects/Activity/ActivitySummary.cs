using System.Xml.Linq;
using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Domain.ValueObjects.Activity;

public record ActivitySummary
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public double DistanceMeters { get; init; }
    public double ElevationMeters { get; init; }
    public string? Type { get; init; }

    public static ActivitySummary Create(
        string name,
        string type,
        DateTime start,
        DateTime end)
    {
        return new ActivitySummary
        {
            Name = name,
            Type = type,
            StartDate = start,
            EndDate = end,
        };
    }

    public static ActivitySummary Create(
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        double distance,
        double elevation,
        string type)
    {
        return new ActivitySummary
        {
            Name = name,
            Description = description,
            Type = type,
            StartDate = startDate,
            EndDate = endDate,
            DistanceMeters = distance,
            ElevationMeters = elevation,
        };
    }

    public static ActivitySummary CopyWithRecords(ActivitySummary summary, ActivityRecords records)
    {
        return new ActivitySummary
        {
            Name = summary.Name,
            Type = summary.Type,
            Description = summary.Description,
            StartDate = records.Values.First().Timestamp,
            EndDate = records.Values.Last().Timestamp,
            DistanceMeters = (double)records.Values.Last().DistanceMeters,
            ElevationMeters = CalculateElevation(records.Values),
        };
    }

    private static double CalculateElevation(IReadOnlyList<RecordData> records)
    {
        double elevation = 0.0;
        float lastAltitude = 0.0f;
        for(int i = 1; i < records.Count; i++)
        {
            var currentAltitude = records[i].Coordinates.Altitude;
            float altDiff = (float)(currentAltitude - lastAltitude);
            elevation += altDiff > 0 ? altDiff : 0;
        }
        return elevation;
    }
}
