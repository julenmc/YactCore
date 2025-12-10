using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.DistanceCalculator;

public class HarversineDistanceCalculatorService : IDistanceCalculator
{
    private const double EarthRadiusMeters = 6371000.0;

    public void CalculateDistances(List<RecordData> records)
    {
        if (records.Count == 0) return;

        List<RecordData> toRemove = new List<RecordData>();

        double totalDistance = 0;
        records[0].DistanceMeters = 0;

        for (int i = 1; i < records.Count; i++)
        {
            RecordData curr = records[i];
            RecordData prev = records[i - 1];

            var distance = CalculateDistanceFromToPoints(prev.Coordinates, curr.Coordinates);
            totalDistance += distance;

            curr.DistanceMeters = (float)totalDistance;
        }

        foreach (var record in toRemove)
        {
            records.Remove(record);
        }
    }

    public double CalculateDistanceFromToPoints(CoordinatesData from, CoordinatesData to)
    {
        if (from?.Latitude == null || from?.Longitude == null ||
            to?.Latitude == null || to?.Longitude == null)
            return 0;

        var dLat = ToRadians(to.Latitude - from.Latitude);
        var dLon = ToRadians(to.Longitude - from.Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(from.Latitude)) *
                Math.Cos(ToRadians(to.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

        return EarthRadiusMeters * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
