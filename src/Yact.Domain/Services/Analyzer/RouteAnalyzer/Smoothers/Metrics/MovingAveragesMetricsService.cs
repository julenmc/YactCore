using Yact.Domain.Entities.Smoother;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Smoothers.Metrics;

public class MovingAveragesMetricsService : IMetricsSmootherService<List<MovingAverageMetric>>
{
    /// <summary>
    /// Calculates a moving average and related metrics for a sequence of numeric records using a sliding window.
    /// </summary>
    /// <remarks>The method computes the moving average, standard deviation, and the difference between the
    /// maximum and minimum values within each sliding window of the specified size. The results are returned as a list
    /// of <see cref="MovingAverageMetric"/> objects, where each object corresponds to a window position in the input
    /// sequence.
    /// Attention: This service won't detect where the activity stops. </remarks>
    /// <param name="records">The list of numeric records to process. Must contain at least <paramref name="windowSize"/> elements.</param>
    /// <param name="windowSize">The size of the sliding window used to calculate the moving average and other metrics. Must be a positive
    /// integer.</param>
    /// <returns>A list of <see cref="MovingAverageMetric"/> objects, each representing the calculated metrics for a specific
    /// window position.</returns>
    /// <exception cref="ArgumentException">Thrown if the number of elements in <paramref name="records"/> is less than <paramref name="windowSize"/>.</exception>
    public List<MovingAverageMetric> Smooth (List<float> records, int windowSize)
    {
        if (records.Count < windowSize)
            throw new ArgumentException(nameof(records), $"Point count ({records.Count}) is smaller than the window size {windowSize}");

        var result = new List<MovingAverageMetric>();
        var recordValues = new Queue<float>();
        float min = float.MaxValue;
        float max = float.MinValue;
        int index = 0;

        int firstPoint = index + windowSize;
        float sum = 0;
        float sumSquares = 0;

        // Initialize first window
        while (index < firstPoint)
        {
            var value = records[index];
            recordValues.Enqueue(value);
            sum += value;
            sumSquares += value * value;
            min = Math.Min(min, value);
            max = Math.Max(max, value);
            index++;
        }

        // Calculate for the first window
        if (recordValues.Count > 0)
        {
            float avg = sum / recordValues.Count;
            float variance = sumSquares / recordValues.Count - avg * avg;
            float stdDev = (float)Math.Sqrt(Math.Max(0, variance));

            result.Add(new MovingAverageMetric
            {
                RecordIndex = index - 1,    // index was ++ earlier 
                Average = avg,
                Deviation = stdDev,
                MaxMinDelta = max - min
            });
        }

        // Sliding window calculations
        while (index < records.Count)
        {
            // Remove the oldest value
            var oldValue = recordValues.Dequeue();
            sum -= oldValue;
            sumSquares -= oldValue * oldValue;

            // Add the new value
            var newValue = records[index];
            recordValues.Enqueue(newValue);
            sum += newValue;
            sumSquares += newValue * newValue;

            // Recalculate min and max
            min = recordValues.Min();
            max = recordValues.Max();

            // Calculate metrics
            float avg = sum / windowSize;
            float variance = sumSquares / windowSize - avg * avg;
            float stdDev = (float)Math.Sqrt(Math.Max(0, variance));
            result.Add(new MovingAverageMetric
            {
                RecordIndex = index,
                Average = avg,
                Deviation = stdDev,
                MaxMinDelta = max - min
            });

            index++;
        }

        return result;
    }
}
