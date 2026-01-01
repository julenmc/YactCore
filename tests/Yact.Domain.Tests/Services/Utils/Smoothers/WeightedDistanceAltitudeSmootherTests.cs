using Yact.Domain.Services.Utils.Smoothers.Altitude;
using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Domain.Tests.Services.Utils.Smoothers;

public class WeightedDistanceAltitudeSmootherTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithDefaultParameters_CreatesInstance()
    {
        // Act
        var smoother = new WeightedDistanceAltitudeSmoother();

        // Assert
        Assert.NotNull(smoother);
    }

    [Fact]
    public void Constructor_WithCustomWindowDistance_CreatesInstance()
    {
        // Act
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 50);

        // Assert
        Assert.NotNull(smoother);
    }

    [Fact]
    public void Constructor_WithCustomSigma_CreatesInstance()
    {
        // Act
        var smoother = new WeightedDistanceAltitudeSmoother(
            windowDistanceMeters: 40,
            sigma: 15);

        // Assert
        Assert.NotNull(smoother);
    }

    [Fact]
    public void Constructor_WithBothCustomParameters_CreatesInstance()
    {
        // Act
        var smoother = new WeightedDistanceAltitudeSmoother(
            windowDistanceMeters: 60,
            sigma: 20);

        // Assert
        Assert.NotNull(smoother);
    }

    #endregion

    #region Empty Cases

    [Fact]
    public void Smooth_EmptyRecords_DoesNotThrow()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother();
        var records = new List<Coordinates>();
        var distances = new List<float>();

        // Act
        var exception = Record.Exception(() => smoother.Smooth(records, distances));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Smooth_SingleRecord_DoesNotModify()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother();
        var originalAltitude = 100.0;
        var coordinates = new List<Coordinates>
        {
            new Coordinates
            {
                Altitude = originalAltitude,
                Latitude = 0,
                Longitude = 0
            }
        };
        var distances = new List<float>
        {
            0
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        Assert.Equal(originalAltitude, smoothed.Last().Altitude);
    }


    #endregion

    #region Basic Smoothing Tests

    [Fact]
    public void Smooth_TwoRecordsWithinWindow_SmoothesBoth()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 120, Latitude = 0, Longitude = 0}
        };
        var distances = new List<float>
        {
            0, 20
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        // Both records should be smoothed with each other's influence
        Assert.NotEqual(100.0, smoothed[0].Altitude);
        Assert.NotEqual(120.0, smoothed[1].Altitude);
    }

    [Fact]
    public void Smooth_RecordsOutsideWindow_RemainsUnchanged()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 30);
        var originalAltitude1 = 100.0;
        var originalAltitude2 = 200.0;
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = originalAltitude1, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = originalAltitude2, Latitude = 0, Longitude = 0}
        };
        var distances = new List<float>
        {
            0, 100
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        // Records far apart remain unchanged (outside window)
        Assert.Equal(originalAltitude1, smoothed[0].Altitude);
        Assert.Equal(originalAltitude2, smoothed[1].Altitude);
        var expectedSlope = originalAltitude2 - originalAltitude1; // * 100 / 100
        Assert.Equal(expectedSlope, smoothed[1].Slope);
    }

    [Fact]
    public void Smooth_ConstantAltitudes_PreservesValues()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var constantAltitude = 150.0;
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = constantAltitude, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = constantAltitude, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = constantAltitude, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = constantAltitude, Latitude = 0, Longitude = 0}
        };
        var distances = new List<float>
        {
            0, 10, 20, 30
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        foreach (var smooth in smoothed)
        {
            Assert.Equal(constantAltitude, smooth.Altitude, 5);
        }
        for (int i = 1; i < smoothed.Count; i++) 
        {
            Assert.Equal(0, smoothed[i].Slope, 3);
        }
    }

    #endregion

    #region Window Distance Tests

    [Fact]
    public void Smooth_SmallWindowDistance_LimitsInfluenceRadius()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 20);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 200, Latitude = 0, Longitude = 0}
        };
        var distances = new List<float>
        {
            0, 30
        };
        var originalAltitudes = coordinates.Select(r => r.Altitude).ToArray();

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        // With small window, records should be less influenced by distant neighbors
        Assert.Equal(originalAltitudes[0], smoothed[0].Altitude, 1);
        Assert.Equal(originalAltitudes[1], smoothed[1].Altitude, 1);
    }

    [Fact]
    public void Smooth_LargeWindowDistance_IncreaseInfluenceRadius()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 100);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 200, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0}
        };
        var distances = new List<float>
        {
            0, 50, 100
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        // Middle record should be influenced by both neighbors
        Assert.NotEqual(200.0, smoothed[1].Altitude);
        Assert.True(smoothed[1].Altitude < 200);
        Assert.True(smoothed[1].Altitude > 100);
    }

    #endregion

    #region Sigma and Gaussian Weight Tests

    [Fact]
    public void Smooth_SmallSigma_CenterPointHasMoreWeight()
    {
        // Arrange
        var smootherSmallSigma = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40, sigma: 5);
        var smootherLargeSigma = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40, sigma: 20);

        var coordinates1 = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 150, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0}
        };
        var distances1 = new List<float>
        {
            0, 10, 20
        };

        var coordinates2 = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 150, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0}
        };
        var distances2 = new List<float>
        {
            0, 10, 20
        };

        // Act
        var smoothed1 = smootherSmallSigma.Smooth(coordinates1, distances1);
        var smoothed2 = smootherLargeSigma.Smooth(coordinates2, distances2);

        // Assert
        // With smaller sigma, center point should retain more of original value
        var altitudeSmallSigma = smoothed1[1].Altitude;
        var altitudeLargeSigma = smoothed2[1].Altitude;

        Assert.True(altitudeSmallSigma > altitudeLargeSigma);
    }

    #endregion

    #region Weight Distribution Tests

    [Fact]
    public void Smooth_CenterPointHasMaximumWeight()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 200, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0}
        };
        var distances = new List<float>
        {
            0, 5, 10, 15, 20
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        // Center point should maintain more of its original altitude despite neighbors
        Assert.True(smoothed[2].Altitude > 120);
    }

    [Fact]
    public void Smooth_DistantPointsHaveMinimalWeight()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40, sigma: 10);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 200, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
        };
        var distances = new List<float>
        {
            0, 19, 20
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        // Points near window edge should have lower influence
        Assert.True(smoothed[0].Altitude < 120);
        Assert.True(smoothed[0].Altitude < smoothed[2].Altitude);
    }

    #endregion

    #region Smoothing Quality Tests

    [Fact]
    public void Smooth_RemovesOutliers()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 50);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 105, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 110, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 999, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 110, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 105, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
        };
        var distances = new List<float>
        {
            0, 10, 20, 30, 40, 50, 60
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        Assert.True(smoothed[3].Altitude < 999);
        Assert.True(smoothed[3].Altitude > 100);
    }

    [Fact]
    public void Smooth_PreservesUpwardTrend()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 30);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 120, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 140, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 160, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 180, Latitude = 0, Longitude = 0},
        };
        var distances = new List<float>
        {
            0, 10, 20, 30, 40
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        for (int i = 1; i < smoothed.Count; i++)
        {
            Assert.True(smoothed[i].Altitude >= smoothed[i - 1].Altitude - 1);
            Assert.True(smoothed[i].Slope > 0);
        }
    }

    [Fact]
    public void Smooth_PreservesDownwardTrend()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 30);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 500, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 400, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 300, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 200, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
        };
        var distances = new List<float>
        {
            0, 10, 20, 30, 40
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        for (int i = 1; i < smoothed.Count; i++)
        {
            Assert.True(smoothed[i].Altitude <= smoothed[i - 1].Altitude + 1);
            Assert.True(smoothed[i].Slope < 0);
        }
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public void Smooth_RealWorldClimbData()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 50);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 105, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 110, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 115, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 120, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 125, Latitude = 0, Longitude = 0}
        };
        var distances = new List<float>
        {
            0, 20, 40, 60, 80, 100
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        // Should preserve gradual climb
        for (int i = 1; i < smoothed.Count; i++)
        {
            Assert.True(smoothed[i].Altitude >= smoothed[i - 1].Altitude);
            Assert.True(smoothed[i].Slope > 0);
        }
    }

    [Fact]
    public void Smooth_HighVarianceDataSmoothed()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 50);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 500, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 110, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 600, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 120, Latitude = 0, Longitude = 0},
        };
        var distances = new List<float>
        {
            0, 15, 30, 45, 60
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        var variance = CalculateVariance(smoothed.Select(r => r.Altitude));
        var originalVariance = CalculateVariance(new[] { 100.0, 500, 110, 600, 120 });

        Assert.True(variance < originalVariance);
    }

    #endregion

    #region Boundary Tests

    [Fact]
    public void Smooth_FirstRecordAtBoundary_Smoothed()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 500, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
        };
        var distances = new List<float>
        {
            0, 10, 20
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        Assert.True(smoothed[0].Altitude < 500);
    }

    [Fact]
    public void Smooth_LastRecordAtBoundary_Smoothed()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 100, Latitude = 0, Longitude = 0},
            new Coordinates{Altitude = 500, Latitude = 0, Longitude = 0},
        };
        var distances = new List<float>
        {
            0, 10, 20
        };

        // Act
        var smoothed = smoother.Smooth(coordinates, distances);

        // Assert
        Assert.True(smoothed[2].Altitude < 500);
    }

    #endregion

    #region Helper Methods

    private double CalculateVariance(IEnumerable<double> values)
    {
        var valueList = values.ToList();
        if (valueList.Count == 0) return 0;

        var mean = valueList.Average();
        var squaredDifferences = valueList.Select(x => Math.Pow(x - mean, 2));
        return squaredDifferences.Average();
    }

    #endregion
}
