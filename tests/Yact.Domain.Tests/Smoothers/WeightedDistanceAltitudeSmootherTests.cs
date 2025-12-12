using Yact.Domain.Services.Analyzer.RouteAnalyzer.Smoothers.Altitude;
using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Tests.Smoothers;

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

    #region Empty and Null Cases

    [Fact]
    public void Smooth_EmptyRecords_DoesNotThrow()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother();
        var records = new List<RecordData>();

        // Act
        var exception = Record.Exception(() => smoother.Smooth(records));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Smooth_SingleRecord_DoesNotModify()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother();
        var originalAltitude = 100.0;
        var records = new List<RecordData>
        {
            new RecordData 
            { 
                DistanceMeters = 0,
                Coordinates = new CoordinatesData { Altitude = originalAltitude } 
            }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        Assert.Equal(originalAltitude, records[0].Coordinates.Altitude);
    }

    [Fact]
    public void Smooth_RecordsWithNullDistanceMeters_SkipsRecord()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother();
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = null, Coordinates = new CoordinatesData { Altitude = 110 } },
            new RecordData { DistanceMeters = 40, Coordinates = new CoordinatesData { Altitude = 120 } }
        };
        var originalAltitude = records[1].Coordinates.Altitude;

        // Act
        smoother.Smooth(records);

        // Assert
        Assert.Equal(originalAltitude, records[1].Coordinates.Altitude);
    }

    [Fact]
    public void Smooth_AllRecordsWithNullDistance_DoesNotModify()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother();
        var originalAltitudes = new[] { 100.0, 110, 120 };
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = null, Coordinates = new CoordinatesData { Altitude = originalAltitudes[0] } },
            new RecordData { DistanceMeters = null, Coordinates = new CoordinatesData { Altitude = originalAltitudes[1] } },
            new RecordData { DistanceMeters = null, Coordinates = new CoordinatesData { Altitude = originalAltitudes[2] } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        for (int i = 0; i < records.Count; i++)
        {
            Assert.Equal(originalAltitudes[i], records[i].Coordinates.Altitude);
        }
    }

    #endregion

    #region Basic Smoothing Tests

    [Fact]
    public void Smooth_TwoRecordsWithinWindow_SmoothesBoth()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 120 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        // Both records should be smoothed with each other's influence
        Assert.NotEqual(100.0, records[0].Coordinates.Altitude);
        Assert.NotEqual(120.0, records[1].Coordinates.Altitude);
    }

    [Fact]
    public void Smooth_RecordsOutsideWindow_RemainsUnchanged()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 30);
        var originalAltitude1 = 100.0;
        var originalAltitude2 = 200.0;
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = originalAltitude1 } },
            new RecordData { DistanceMeters = 100, Coordinates = new CoordinatesData { Altitude = originalAltitude2 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        // Records far apart remain unchanged (outside window)
        Assert.Equal(originalAltitude1, records[0].Coordinates.Altitude);
        Assert.Equal(originalAltitude2, records[1].Coordinates.Altitude);
        Assert.NotNull(records[1].Slope);
        var expectedSlope = originalAltitude2 - originalAltitude1; // * 100 / 100
        Assert.Equal(expectedSlope, records[1].Slope ?? 0);
    }

    [Fact]
    public void Smooth_ConstantAltitudes_PreservesValues()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var constantAltitude = 150.0;
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = constantAltitude } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = constantAltitude } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = constantAltitude } },
            new RecordData { DistanceMeters = 30, Coordinates = new CoordinatesData { Altitude = constantAltitude } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        foreach (var record in records)
        {
            Assert.Equal(constantAltitude, record.Coordinates.Altitude, 5);
        }
        for (int i = 1; i < records.Count; i++) 
        {
            Assert.NotNull(records[i].Slope);
            Assert.Equal(0, records[i].Slope ?? 0, 3);
        }
    }

    #endregion

    #region Window Distance Tests

    [Fact]
    public void Smooth_SmallWindowDistance_LimitsInfluenceRadius()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 20);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 30, Coordinates = new CoordinatesData { Altitude = 200 } }
        };
        var originalAltitudes = records.Select(r => r.Coordinates.Altitude).ToArray();

        // Act
        smoother.Smooth(records);

        // Assert
        // With small window, records should be less influenced by distant neighbors
        Assert.Equal(originalAltitudes[0], records[0].Coordinates.Altitude, 1);
        Assert.Equal(originalAltitudes[1], records[1].Coordinates.Altitude, 1);
    }

    [Fact]
    public void Smooth_LargeWindowDistance_IncreaseInfluenceRadius()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 100);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 50, Coordinates = new CoordinatesData { Altitude = 200 } },
            new RecordData { DistanceMeters = 100, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        // Middle record should be influenced by both neighbors
        Assert.NotEqual(200.0, records[1].Coordinates.Altitude);
        Assert.True(records[1].Coordinates.Altitude < 200);
        Assert.True(records[1].Coordinates.Altitude > 100);
    }

    #endregion

    #region Sigma and Gaussian Weight Tests

    [Fact]
    public void Smooth_SmallSigma_CenterPointHasMoreWeight()
    {
        // Arrange
        var smootherSmallSigma = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40, sigma: 5);
        var smootherLargeSigma = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40, sigma: 20);
        
        var records1 = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 150 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        var records2 = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 150 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        // Act
        smootherSmallSigma.Smooth(records1);
        smootherLargeSigma.Smooth(records2);

        // Assert
        // With smaller sigma, center point should retain more of original value
        var altitudeSmallSigma = records1[1].Coordinates.Altitude;
        var altitudeLargeSigma = records2[1].Coordinates.Altitude;

        Assert.True(altitudeSmallSigma > altitudeLargeSigma);
    }

    #endregion

    #region Weight Distribution Tests

    [Fact]
    public void Smooth_CenterPointHasMaximumWeight()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 5, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 200 } }, // Center
            new RecordData { DistanceMeters = 15, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        // Center point should maintain more of its original altitude despite neighbors
        Assert.True(records[2].Coordinates.Altitude > 120);
    }

    [Fact]
    public void Smooth_DistantPointsHaveMinimalWeight()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40, sigma: 10);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 19, Coordinates = new CoordinatesData { Altitude = 200 } }, // Within window but far
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        // Points near window edge should have lower influence
        Assert.True(records[0].Coordinates.Altitude < 120);
        Assert.True(records[0].Coordinates.Altitude < records[2].Coordinates.Altitude);
    }

    #endregion

    #region Smoothing Quality Tests

    [Fact]
    public void Smooth_RemovesOutliers()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 50);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 105 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 110 } },
            new RecordData { DistanceMeters = 30, Coordinates = new CoordinatesData { Altitude = 999 } }, // Outlier
            new RecordData { DistanceMeters = 40, Coordinates = new CoordinatesData { Altitude = 110 } },
            new RecordData { DistanceMeters = 50, Coordinates = new CoordinatesData { Altitude = 105 } },
            new RecordData { DistanceMeters = 60, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        Assert.True(records[3].Coordinates.Altitude < 999);
        Assert.True(records[3].Coordinates.Altitude > 100);
    }

    [Fact]
    public void Smooth_PreservesUpwardTrend()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 30);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 120 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 140 } },
            new RecordData { DistanceMeters = 30, Coordinates = new CoordinatesData { Altitude = 160 } },
            new RecordData { DistanceMeters = 40, Coordinates = new CoordinatesData { Altitude = 180 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        for (int i = 1; i < records.Count; i++)
        {
            Assert.True(records[i].Coordinates.Altitude >= records[i - 1].Coordinates.Altitude - 1);
            Assert.NotNull(records[i].Slope);
            Assert.True(records[i].Slope > 0);
        }
    }

    [Fact]
    public void Smooth_PreservesDownwardTrend()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 30);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 500 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 400 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 300 } },
            new RecordData { DistanceMeters = 30, Coordinates = new CoordinatesData { Altitude = 200 } },
            new RecordData { DistanceMeters = 40, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        for (int i = 1; i < records.Count; i++)
        {
            Assert.True(records[i].Coordinates.Altitude <= records[i - 1].Coordinates.Altitude + 1);
            Assert.NotNull(records[i].Slope);
            Assert.True(records[i].Slope < 0);
        }
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public void Smooth_RealWorldClimbData()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 50);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 105 } },
            new RecordData { DistanceMeters = 40, Coordinates = new CoordinatesData { Altitude = 110 } },
            new RecordData { DistanceMeters = 60, Coordinates = new CoordinatesData { Altitude = 115 } },
            new RecordData { DistanceMeters = 80, Coordinates = new CoordinatesData { Altitude = 120 } },
            new RecordData { DistanceMeters = 100, Coordinates = new CoordinatesData { Altitude = 125 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        // Should preserve gradual climb
        for (int i = 1; i < records.Count; i++)
        {
            Assert.True(records[i].Coordinates.Altitude >= records[i - 1].Coordinates.Altitude);
            Assert.NotNull(records[i].Slope);
            Assert.True(records[i].Slope > 0);
        }
    }

    [Fact]
    public void Smooth_HighVarianceDataSmoothed()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 50);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 15, Coordinates = new CoordinatesData { Altitude = 500 } },
            new RecordData { DistanceMeters = 30, Coordinates = new CoordinatesData { Altitude = 110 } },
            new RecordData { DistanceMeters = 45, Coordinates = new CoordinatesData { Altitude = 600 } },
            new RecordData { DistanceMeters = 60, Coordinates = new CoordinatesData { Altitude = 120 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        var variance = CalculateVariance(records.Select(r => r.Coordinates.Altitude));
        var originalVariance = CalculateVariance(new[] { 100.0, 500, 110, 600, 120 });

        Assert.True(variance < originalVariance);
    }

    [Fact]
    public void Smooth_PreservesCoordinatesExceptAltitude()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother();
        var originalLat = 40.7128;
        var originalLon = -74.0060;
        var records = new List<RecordData>
        {
            new RecordData 
            { 
                DistanceMeters = 0,
                Coordinates = new CoordinatesData 
                { 
                    Latitude = originalLat, 
                    Longitude = originalLon, 
                    Altitude = 100 
                } 
            },
            new RecordData 
            { 
                DistanceMeters = 20,
                Coordinates = new CoordinatesData 
                { 
                    Latitude = originalLat, 
                    Longitude = originalLon, 
                    Altitude = 120 
                } 
            }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        for (int i = 0; i < records.Count; i++)
        {
            Assert.Equal(originalLat, records[i].Coordinates.Latitude);
            Assert.Equal(originalLon, records[i].Coordinates.Longitude);
        }
    }

    #endregion

    #region Boundary Tests

    [Fact]
    public void Smooth_FirstRecordAtBoundary_Smoothed()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 500 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 100 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        Assert.True(records[0].Coordinates.Altitude < 500);
    }

    [Fact]
    public void Smooth_LastRecordAtBoundary_Smoothed()
    {
        // Arrange
        var smoother = new WeightedDistanceAltitudeSmoother(windowDistanceMeters: 40);
        var records = new List<RecordData>
        {
            new RecordData { DistanceMeters = 0, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 10, Coordinates = new CoordinatesData { Altitude = 100 } },
            new RecordData { DistanceMeters = 20, Coordinates = new CoordinatesData { Altitude = 500 } }
        };

        // Act
        smoother.Smooth(records);

        // Assert
        Assert.True(records[2].Coordinates.Altitude < 500);
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
