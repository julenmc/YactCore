using Yact.Domain.Services.Utils.Smoothers.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Yact.Domain.Services.Utils.Smoothers.Metrics.MovingAveragesMetricsService;

namespace Yact.Domain.Tests.Services.Utils.Smoothers;

public class MovingAveragesServiceTests
{
    private readonly MovingAveragesMetricsService _service = new();

    #region Error Cases

    [Fact]
    public void Smooth_RecordsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var records = new List<SmoothInput>();
        var windowSize = 3;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Smooth(records, windowSize));
        Assert.Contains("Point count", exception.Message);
    }

    [Fact]
    public void Smooth_WindowSizeGreaterThanRecords_ThrowsArgumentException()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput> 
        { 
            new SmoothInput(date, 1f),
            new SmoothInput(date.AddSeconds(1), 2f),
            new SmoothInput(date.AddSeconds(2), 3f),
        };
        var windowSize = 5;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Smooth(records, windowSize));
        Assert.Contains("smaller than the window size", exception.Message);
    }

    [Fact]
    public void Smooth_WindowSizeEqualsRecordCount_ReturnsOneMetric()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 1f),
            new SmoothInput(date.AddSeconds(1), 2f),
            new SmoothInput(date.AddSeconds(2), 3f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    #endregion

    #region Basic Functionality

    [Fact]
    public void Smooth_SimpleSequence_ReturnsCorrectNumberOfMetrics()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 1f),
            new SmoothInput(date.AddSeconds(1), 2f),
            new SmoothInput(date.AddSeconds(2), 3f),
            new SmoothInput(date.AddSeconds(3), 4f),
            new SmoothInput(date.AddSeconds(4), 5f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        // With 5 records and window size 3: first window at index 3, then sliding for indices 3, 4
        // Total: 1 (first window) + 2 (sliding) = 3 results
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Smooth_ResultContainsValidMetrics()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 1f),
            new SmoothInput(date.AddSeconds(1), 2f),
            new SmoothInput(date.AddSeconds(2), 3f),
            new SmoothInput(date.AddSeconds(3), 4f),
            new SmoothInput(date.AddSeconds(4), 5f),
        };
        var windowSize = 2;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        foreach (var metric in result)
        {
            Assert.True(metric.LastPoint.Timestamp > date);
            Assert.True(metric.Average >= 0);
            Assert.True(metric.Deviation >= 0);
        }
    }

    #endregion

    #region Mathematical Validation

    [Fact]
    public void Smooth_ConstantValues_AverageEqualsValue()
    {
        // Arrange
        var constantValue = 5f;
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, constantValue),
            new SmoothInput(date.AddSeconds(1), constantValue),
            new SmoothInput(date.AddSeconds(2), constantValue),
            new SmoothInput(date.AddSeconds(3), constantValue),
            new SmoothInput(date.AddSeconds(4), constantValue),
        };
        var windowSize = 2;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        foreach (var metric in result)
        {
            Assert.Equal(constantValue, metric.Average, 5); // 5 decimal places precision
            Assert.Equal(0, metric.Deviation, 5);
        }
    }

    [Fact]
    public void Smooth_WindowSizeOne_AverageEqualsValue()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 1f),
            new SmoothInput(date.AddSeconds(1), 2f),
            new SmoothInput(date.AddSeconds(2), 3f),
            new SmoothInput(date.AddSeconds(3), 4f),
        };
        var windowSize = 1;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        for (int i = 0; i < result.Count; i++)
        {
            Assert.Equal(records[i].Value, result[i].Average, 5);
            Assert.Equal(0, result[i].Deviation, 5);
        }
    }

    [Fact]
    public void Smooth_SequentialValues_CalculatesCorrectAverage()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 10f),
            new SmoothInput(date.AddSeconds(1), 20f),
            new SmoothInput(date.AddSeconds(2), 30f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.Single(result);
        var expectedAverage = (10f + 20f + 30f) / 3f; // 20f
        Assert.Equal(expectedAverage, result[0].Average, 5);
    }

    [Fact]
    public void Smooth_CalculatesCorrectStandardDeviation()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 2f),
            new SmoothInput(date.AddSeconds(1), 4f),
            new SmoothInput(date.AddSeconds(2), 6f),
            new SmoothInput(date.AddSeconds(3), 8f),
        };
        var windowSize = 2;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // First result should be for window [2, 4]
        // Average = 3, variance = ((4-3)^2 + (2-3)^2) / 2 = 1
        var firstMetric = result[0];
        Assert.Equal(3f, firstMetric.Average, 5);
        Assert.Equal(1f, firstMetric.Deviation, 5);
    }

    #endregion

    #region Index Validation

    [Fact]
    public void Smooth_RecordIndexIsCorrect()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 1f),
            new SmoothInput(date.AddSeconds(1), 2f),
            new SmoothInput(date.AddSeconds(2), 3f),
            new SmoothInput(date.AddSeconds(3), 4f),
            new SmoothInput(date.AddSeconds(4), 5f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        // Expected indices: 2, 3, 4 (after first window completion and sliding)
        Assert.Equal(date.AddSeconds(2), result[0].LastPoint.Timestamp);
        Assert.Equal(date.AddSeconds(3), result[1].LastPoint.Timestamp);
        Assert.Equal(date.AddSeconds(4), result[2].LastPoint.Timestamp);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Smooth_NegativeValues_CalculatesCorrectly()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, -5f),
            new SmoothInput(date.AddSeconds(1), -3f),
            new SmoothInput(date.AddSeconds(2), -1f),
            new SmoothInput(date.AddSeconds(3), 1f),
            new SmoothInput(date.AddSeconds(4), 3f),
        };
        var windowSize = 2;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // First window [-5, -3]: average = -4
        Assert.Equal(-4f, result[0].Average, 5);
    }

    [Fact]
    public void Smooth_MixedPositiveAndNegativeValues_CalculatesCorrectly()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, -2f),
            new SmoothInput(date.AddSeconds(1), -1f),
            new SmoothInput(date.AddSeconds(2), 0f),
            new SmoothInput(date.AddSeconds(3), 1f),
            new SmoothInput(date.AddSeconds(4), 2f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        // First window [-2, -1, 0]: average = -1
        Assert.Equal(-1f, result[0].Average, 5);
    }

    [Fact]
    public void Smooth_LargeValues_CalculatesCorrectly()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 1000000f),
            new SmoothInput(date.AddSeconds(1), 2000000f),
            new SmoothInput(date.AddSeconds(2), 3000000f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.Single(result);
        var expectedAverage = (1000000f + 2000000f + 3000000f) / 3f;
        Assert.Equal(expectedAverage, result[0].Average, 0);
    }

    [Fact]
    public void Smooth_SmallValues_CalculatesCorrectly()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 0.001f),
            new SmoothInput(date.AddSeconds(1), 0.002f),
            new SmoothInput(date.AddSeconds(2), 0.003f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.Single(result);
        var expectedAverage = (0.001f + 0.002f + 0.003f) / 3f;
        Assert.Equal(expectedAverage, result[0].Average, 6);
    }

    #endregion

    #region Sliding Window Behavior

    [Fact]
    public void Smooth_SlidingWindowMovesCorrectly()
    {
        // Arrange
        var date = DateTime.Now;
        var records = new List<SmoothInput>
        {
            new SmoothInput(date, 1f),
            new SmoothInput(date.AddSeconds(1), 2f),
            new SmoothInput(date.AddSeconds(2), 3f),
            new SmoothInput(date.AddSeconds(3), 4f),
            new SmoothInput(date.AddSeconds(4), 5f),
            new SmoothInput(date.AddSeconds(5), 6f),
        };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count); // 1 initial + 3 sliding

        // Verify consecutive windows
        // Window 1: [1, 2, 3] average = 2
        Assert.Equal(2f, result[0].Average, 5);

        // Window 2: [2, 3, 4] average = 3
        Assert.Equal(3f, result[1].Average, 5);

        // Window 3: [3, 4, 5] average = 4
        Assert.Equal(4f, result[2].Average, 5);

        // Window 4: [4, 5, 6] average = 5
        Assert.Equal(5f, result[3].Average, 5);
    }

    #endregion
}
