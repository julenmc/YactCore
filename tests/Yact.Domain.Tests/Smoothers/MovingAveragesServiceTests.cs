using Yact.Domain.Services.Smoother;
using Yact.Domain.Entities.Smoother;

namespace Yact.Domain.Tests.Smoothers;

public class MovingAveragesServiceTests
{
    private readonly MovingAveragesService _service = new();

    #region Error Cases

    [Fact]
    public void Smooth_RecordsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var records = new List<float>();
        var windowSize = 3;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Smooth(records, windowSize));
        Assert.Contains("Point count", exception.Message);
    }

    [Fact]
    public void Smooth_WindowSizeGreaterThanRecords_ThrowsArgumentException()
    {
        // Arrange
        var records = new List<float> { 1f, 2f, 3f };
        var windowSize = 5;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.Smooth(records, windowSize));
        Assert.Contains("smaller than the window size", exception.Message);
    }

    [Fact]
    public void Smooth_WindowSizeEqualsRecordCount_ReturnsOneMetric()
    {
        // Arrange
        var records = new List<float> { 1f, 2f, 3f };
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
        var records = new List<float> { 1f, 2f, 3f, 4f, 5f };
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
        var records = new List<float> { 1f, 2f, 3f, 4f, 5f };
        var windowSize = 2;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        foreach (var metric in result)
        {
            Assert.True(metric.RecordIndex > 0);
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
        var records = new List<float> { constantValue, constantValue, constantValue, constantValue };
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
        var records = new List<float> { 1f, 2f, 3f, 4f };
        var windowSize = 1;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        for (int i = 0; i < result.Count; i++)
        {
            Assert.Equal(records[i], result[i].Average, 5);
            Assert.Equal(0, result[i].Deviation, 5);
        }
    }

    [Fact]
    public void Smooth_SequentialValues_CalculatesCorrectAverage()
    {
        // Arrange
        var records = new List<float> { 10f, 20f, 30f };
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
        var records = new List<float> { 2f, 4f, 6f, 8f };
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
        var records = new List<float> { 1f, 2f, 3f, 4f, 5f };
        var windowSize = 3;

        // Act
        var result = _service.Smooth(records, windowSize);

        // Assert
        Assert.NotNull(result);
        // Expected indices: 2, 3, 4 (after first window completion and sliding)
        Assert.Equal(2, result[0].RecordIndex);
        Assert.Equal(3, result[1].RecordIndex);
        Assert.Equal(4, result[2].RecordIndex);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Smooth_NegativeValues_CalculatesCorrectly()
    {
        // Arrange
        var records = new List<float> { -5f, -3f, -1f, 1f, 3f };
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
        var records = new List<float> { -2f, -1f, 0f, 1f, 2f };
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
        var records = new List<float> { 1000000f, 2000000f, 3000000f };
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
        var records = new List<float> { 0.001f, 0.002f, 0.003f };
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
        var records = new List<float> { 1f, 2f, 3f, 4f, 5f, 6f };
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
