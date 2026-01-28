using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;

public class HarversineDistanceCalculatorService
{
    private const double EarthRadiusMeters = 6371000.0;

    public List<float> CalculateDistances(IReadOnlyList<Coordinates> records)
    {
        List<float> result = new List<float>();
        if (records.Count == 0) 
            return result;

        float totalDistance = 0;
        result.Add(totalDistance);

        for (int i = 1; i < records.Count; i++)
        {
            Coordinates curr = records[i];
            Coordinates prev = records[i - 1];

            var distance = CalculateDistanceFromToPoints(prev, curr);
            totalDistance += distance;

            result.Add(totalDistance);
        }

        return result;
    }

    public float CalculateDistanceFromToPoints(Coordinates from, Coordinates to)
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

        return (float)(EarthRadiusMeters * c);
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
