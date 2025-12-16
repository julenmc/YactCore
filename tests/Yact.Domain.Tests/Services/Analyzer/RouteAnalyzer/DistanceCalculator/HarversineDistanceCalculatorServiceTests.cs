using Yact.Domain.Entities.Activity;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;

namespace Yact.Domain.Tests.Services.Analyzer.RouteAnalyzer.DistanceCalculator;

public class HarversineDistanceCalculatorServiceTests
{
    private readonly HarversineDistanceCalculatorService _service = new();

    #region CalculateDistance Tests

    [Fact]
    public void CalculateDistanceFromToPoints_SameCoordinates_ReturnsZero()
    {
        // Arrange
        var coordinates = new CoordinatesData { Latitude = 40.7128, Longitude = -74.0060 };

        // Act
        var distance = _service.CalculateDistanceFromToPoints(coordinates, coordinates);

        // Assert
        Assert.Equal(0, distance, 5);
    }

    [Fact]
    public void CalculateDistanceFromToPoints_ShortDistance_CalculatesCorrectly()
    {
        // Arrange - Two points 10 m apart (approximately)
        var from = new CoordinatesData { Latitude = 0.0, Longitude = 0.0 };
        var to = new CoordinatesData { Latitude = 0.00009, Longitude = 0.0 };

        // Act
        var distance = _service.CalculateDistanceFromToPoints(from, to);

        // Assert
        // 0.00009 degrees latitude is approximately 10 meters
        Assert.Equal(10.0f, distance, 1);
    }

    [Fact]
    public void CalculateDistanceFromToPoints_IsSymmetric()
    {
        // Arrange
        var point1 = new CoordinatesData { Latitude = 51.5074, Longitude = -0.1278 }; // London
        var point2 = new CoordinatesData { Latitude = 48.8566, Longitude = 2.3522 }; // Paris

        // Act
        var distance1To2 = _service.CalculateDistanceFromToPoints(point1, point2);
        var distance2To1 = _service.CalculateDistanceFromToPoints(point2, point1);

        // Assert
        Assert.Equal(distance1To2, distance2To1, 1);
    }

    [Fact]
    public void CalculateDistanceFromToPoints_NearEquator_CalculatesCorrectly()
    {
        // Arrange
        var from = new CoordinatesData { Latitude = 0.0, Longitude = 0.0 };
        var to = new CoordinatesData { Latitude = 0.0, Longitude = 0.009 };

        // Act
        var distance = _service.CalculateDistanceFromToPoints(from, to);

        // Assert
        // At equator, 0.009 degrees longitude is approximately 1000 meters
        Assert.InRange(distance, 900, 1100);
    }

    [Fact]
    public void CalculateDistanceFromToPoints_ReturnValueIsPositive()
    {
        // Arrange
        var from = new CoordinatesData { Latitude = 40.7128, Longitude = -74.0060 };
        var to = new CoordinatesData { Latitude = 34.0522, Longitude = -118.2437 };

        // Act
        var distance = _service.CalculateDistanceFromToPoints(from, to);

        // Assert
        Assert.True(distance >= 0);
    }

    #endregion

    #region CalculateDistances Tests

    [Fact]
    public void CalculateDistances_EmptyRecords_DoesNotThrow()
    {
        // Arrange
        var records = new List<RecordData>();

        // Act
        var exception = Record.Exception(() => _service.CalculateDistances(records));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CalculateDistances_SingleRecord_FirstRecordDistanceIsZero()
    {
        // Arrange
        var records = new List<RecordData>
        {
            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.7128, Longitude = -74.0060 } }
        };

        // Act
        _service.CalculateDistances(records);

        // Assert
        Assert.Equal(0, records[0].DistanceMeters);
    }

    [Fact]
    public void CalculateDistances_TwoRecords_CalculatesDistance()
    {
        // Arrange
        var records = new List<RecordData>
        {
            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.7128, Longitude = -74.0060 } },
            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.7260, Longitude = -74.0067 } }
        };

        // Act
        _service.CalculateDistances(records);

        // Assert
        Assert.Equal(0, records[0].DistanceMeters);
        Assert.True(records[1].DistanceMeters > 0);
    }

    [Fact]
    public void CalculateDistances_MultipleRecords_DistancesAreIncreasing()
    {
        // Arrange
        var records = new List<RecordData>
        {
            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.7128, Longitude = -74.0060 } },
            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.7260, Longitude = -74.0067 } },
            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.7300, Longitude = -74.0100 } },
            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.7400, Longitude = -74.0150 } }
        };

        // Act
        _service.CalculateDistances(records);

        // Assert
        Assert.Equal(0, records[0].DistanceMeters);
        Assert.True(records[1].DistanceMeters >= records[0].DistanceMeters);
        Assert.True(records[2].DistanceMeters >= records[1].DistanceMeters);
        Assert.True(records[3].DistanceMeters >= records[2].DistanceMeters);
    }

    [Fact]
    public void CalculateDistances_AllRecordsHaveSameCoordinates_DistanceRemainZero()
    {
        // Arrange
        var coord = new CoordinatesData { Latitude = 40.7128, Longitude = -74.0060 };
        var records = new List<RecordData>
        {
            new RecordData { Coordinates = coord },
            new RecordData { Coordinates = new CoordinatesData { Latitude = coord.Latitude, Longitude = coord.Longitude } },
            new RecordData { Coordinates = new CoordinatesData { Latitude = coord.Latitude, Longitude = coord.Longitude } }
        };

        // Act
        _service.CalculateDistances(records);

        // Assert
        foreach (var record in records)
        {
            Assert.True(record.DistanceMeters == 0);
        }
    }

    #endregion
}
