using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.Utils.Smoothers.Altitude;

/// <summary>
/// Smoothin with weight: nearest points have more influence.
/// Uses a Gaussian function for the weight.
/// </summary>
public class WeightedDistanceAltitudeSmoother : IAltitudeSmootherService
{
    private readonly double _windowDistanceMeters;
    private readonly double _sigma;

    /// <summary>
    /// Creates a smoother by distance
    /// </summary>
    /// <param name="windowDistanceMeters">Window distance (30-60m)</param>
    /// <param name="sigma">Standard gaussian deviation. 
    /// Lower = center has more impact. By default: windowDistance/3</param>
    public WeightedDistanceAltitudeSmoother(
        double windowDistanceMeters = 40,
        double? sigma = null)
    {
        _windowDistanceMeters = windowDistanceMeters;
        _sigma = sigma ?? windowDistanceMeters / 3.0;
    }

    public void Smooth(List<RecordData> records)
    {
        if (records.Count < 2)
            return;

        var originalAltitudes = records
            .Select(r => r.Coordinates?.Altitude)
            .ToList();

        var halfWindow = _windowDistanceMeters / 2;

        for (int i = 0; i < records.Count; i++)
        {
            if (records[i].DistanceMeters == null)
                continue;

            var centerDistance = records[i].DistanceMeters!.Value;
            var minDistance = Math.Max(centerDistance - halfWindow, 0);
            var maxDistance = Math.Min(centerDistance + halfWindow, records.Last().DistanceMeters!.Value);

            // Find window range
            var startIdx = FindFirstIndexGreaterOrEqual(records, minDistance, i);
            var endIdx = FindLastIndexLessOrEqual(records, maxDistance, i);

            if (startIdx == -1 || endIdx == -1 || startIdx > endIdx)
                continue;

            var weightedSum = 0.0;
            var totalWeight = 0.0;

            for (int j = startIdx; j <= endIdx; j++)
            {
                if (records[j].DistanceMeters == null)
                    continue;

                var pointDistance = records[j].DistanceMeters!.Value;
                var distanceFromCenter = Math.Abs(pointDistance - centerDistance);

                // Peso gaussiano
                var weight = Math.Exp(-(distanceFromCenter * distanceFromCenter) /
                                     (2 * _sigma * _sigma));

                weightedSum += originalAltitudes[j]!.Value * weight;
                totalWeight += weight;
            }

            if (totalWeight > 0)
            {
                records[i].Coordinates.Altitude = weightedSum / totalWeight;
                if (i > 0)
                {
                    records[i].Slope = (float)(records[i].Coordinates.Altitude - records[i - 1].Coordinates.Altitude) / (records[i].DistanceMeters - records[i - 1].DistanceMeters) * 100;
                }
            }
        }
    }

    private int FindFirstIndexGreaterOrEqual(List<RecordData> records, double distance, int currentIndex)
    {
        if (currentIndex == 0) return 0;

        int left = currentIndex - 1;
        int result = -1;

        while (left >= 0)
        {
            if (records[left].DistanceMeters != null && records[left].DistanceMeters <= distance)
            {
                result = left;
                break;
            }
            left--;
        }

        return result;
    }

    private int FindLastIndexLessOrEqual(List<RecordData> records, double distance, int currentIndex)
    {
        if (currentIndex == records.Count - 1) return currentIndex;

        int right = currentIndex + 1;
        int result = -1;

        while (right < records.Count)
        {
            if (records[right].DistanceMeters != null && records[right].DistanceMeters >= distance)
            {
                result = right;
                break;
            }
            right++;
        }

        return result;
    }
}
