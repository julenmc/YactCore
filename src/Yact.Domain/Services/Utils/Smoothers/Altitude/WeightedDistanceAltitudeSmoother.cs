using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Domain.Services.Utils.Smoothers.Altitude;

/// <summary>
/// Smoothin with weight: nearest points have more influence.
/// Uses a Gaussian function for the weight.
/// </summary>
public class WeightedDistanceAltitudeSmoother
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

    public IReadOnlyList<SmoothedAltitude> Smooth(
        IReadOnlyList<Coordinates> coordinates, 
        IReadOnlyList<float> distances)
    {
        if (coordinates.Count != distances.Count)
            throw new ArgumentException($"Coordinates {coordinates.Count} and distances {distances.Count} count must be the same.");

        if (coordinates.Count < 2)
        {
            var res = new List<SmoothedAltitude>();
            for (int i = 0; i < coordinates.Count; i++)
            {
                res.Add(new SmoothedAltitude()
                {
                    Altitude = coordinates[i].Altitude,
                    Slope = (float)coordinates[i].Altitude / distances[i] * 100.0f
                });
            }
            return res;
        }

        var halfWindow = _windowDistanceMeters / 2;
        var smoothedAltitudeList = new List<SmoothedAltitude>();

        for (int i = 0; i < coordinates.Count; i++)
        {
            //if (distances == null)
            //    continue;

            var centerDistance = distances[i];
            var minDistance = Math.Max(centerDistance - halfWindow, 0);
            var maxDistance = Math.Min(centerDistance + halfWindow, distances.Last());

            // Find window range
            var startIdx = FindFirstIndexGreaterOrEqual(distances, minDistance, i);
            var endIdx = FindLastIndexLessOrEqual(distances, maxDistance, i);

            if (startIdx == -1 || endIdx == -1 || startIdx > endIdx)
                continue;

            var weightedSum = 0.0;
            var totalWeight = 0.0;

            for (int j = startIdx; j <= endIdx; j++)
            {
                //if (distances[j] == null)
                //    continue;

                var pointDistance = distances[j];
                var distanceFromCenter = Math.Abs(pointDistance - centerDistance);

                // Peso gaussiano
                var weight = Math.Exp(-(distanceFromCenter * distanceFromCenter) /
                                     (2 * _sigma * _sigma));

                weightedSum += coordinates[j].Altitude * weight;
                totalWeight += weight;
            }

            if (totalWeight > 0)
            {
                double smoothedAlt = weightedSum / totalWeight;
                float slope = 0;
                if (i > 0)
                {
                    slope = (float)(coordinates[i].Altitude - coordinates[i - 1].Altitude) / (distances[i] - distances[i - 1]) * 100;
                }
                smoothedAltitudeList.Add(new SmoothedAltitude() { Altitude = smoothedAlt, Slope = slope });
            }
        }

        return smoothedAltitudeList;
    }

    private int FindFirstIndexGreaterOrEqual(
        IReadOnlyList<float> distancesList,
        double distance, 
        int currentIndex)
    {
        if (currentIndex == 0) return 0;

        int left = currentIndex - 1;
        int result = -1;

        while (left >= 0)
        {
            if (distancesList[left] <= distance)
            {
                result = left;
                break;
            }
            left--;
        }

        return result;
    }

    private int FindLastIndexLessOrEqual(
        IReadOnlyList<float> distancesList,
        double distance,
        int currentIndex)
    {
        if (currentIndex == distancesList.Count - 1) return currentIndex;

        int right = currentIndex + 1;
        int result = -1;

        while (right < distancesList.Count)
        {
            if (distancesList[right] >= distance)
            {
                result = right;
                break;
            }
            right++;
        }

        return result;
    }
}
