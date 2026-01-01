using Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;
using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Domain.Tests.Services.Analyzer.RouteAnalyzer.DistanceCalculator;

public class HarversineDistanceCalculatorServiceTests
{
    private readonly HarversineDistanceCalculatorService _service = new();

    #region CalculateDistance Tests

    [Fact]
    public void CalculateDistanceFromToPoints_SameCoordinates_ReturnsZero()
    {
        // Arrange
        var coordinates = new Coordinates { Latitude = 40.7128, Longitude = -74.0060, Altitude = 0 };

        // Act
        var distance = _service.CalculateDistanceFromToPoints(coordinates, coordinates);

        // Assert
        Assert.Equal(0, distance, 5);
    }

    [Fact]
    public void CalculateDistanceFromToPoints_ShortDistance_CalculatesCorrectly()
    {
        // Arrange - Two points 10 m apart (approximately)
        var from = new Coordinates{Latitude = 0.0, Longitude = 0.0, Altitude = 0 };
        var to = new Coordinates{Latitude = 0.00009, Longitude = 0.0, Altitude = 0 };

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
        var point1 = new Coordinates{Latitude = 51.5074, Longitude = -0.1278, Altitude = 0 }; // London
        var point2 = new Coordinates{Latitude = 48.8566, Longitude = 2.3522, Altitude = 0 }; // Paris

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
        var from = new Coordinates{Latitude = 0.0, Longitude = 0.0, Altitude = 0 };
        var to = new Coordinates{Latitude = 0.0, Longitude = 0.009, Altitude = 0 };

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
        var from = new Coordinates{Latitude = 40.7128, Longitude = -74.0060, Altitude = 0 };
        var to = new Coordinates{Latitude = 34.0522, Longitude = -118.2437, Altitude = 0 };

        // Act
        var distance = _service.CalculateDistanceFromToPoints(from, to);

        // Assert
        Assert.True(distance >= 0);
    }

    #endregion

    #region CalculateDistances Tests

    [Fact]
    public void CalculateDistances_EmptyCoordinates_DoesNotThrow()
    {
        // Arrange
        var coordinates = new List<Coordinates>();

        // Act
        var exception = Record.Exception(() => _service.CalculateDistances(coordinates));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CalculateDistances_SingleRecord_FirstRecordDistanceIsZero()
    {
        // Arrange
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Latitude = 40.7128, Longitude = -74.0060, Altitude = 0}
        };

        // Act
        var distances = _service.CalculateDistances(coordinates);

        // Assert
        Assert.Equal(0, distances[0]);
    }

    [Fact]
    public void CalculateDistances_Twocoordinates_CalculatesDistance()
    {
        // Arrange
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Latitude = 40.7128, Longitude = -74.0060, Altitude = 0},
            new Coordinates{Latitude = 40.7260, Longitude = -74.0067, Altitude = 0}
        };

        // Act
        var distances = _service.CalculateDistances(coordinates);

        // Assert
        Assert.Equal(0, distances[0]);
        Assert.True(distances[1] > 0);
    }

    [Fact]
    public void CalculateDistances_Multiplecoordinates_DistancesAreIncreasing()
    {
        // Arrange
        var coordinates = new List<Coordinates>
        {
            new Coordinates{Latitude = 40.7128, Longitude = -74.0060, Altitude = 0},
            new Coordinates{Latitude = 40.7260, Longitude = -74.0067, Altitude = 0},
            new Coordinates{Latitude = 40.7300, Longitude = -74.0100, Altitude = 0},
            new Coordinates{Latitude = 40.7400, Longitude = -74.0150, Altitude = 0}
        };

        // Act
        var distances = _service.CalculateDistances(coordinates);

        // Assert
        Assert.Equal(0, distances[0]);
        Assert.True(distances[1] >= distances[0]);
        Assert.True(distances[2] >= distances[1]);
        Assert.True(distances[3] >= distances[2]);
    }

    [Fact]
    public void CalculateDistances_AllcoordinatesHaveSameCoordinates_DistanceRemainZero()
    {
        // Arrange
        var coord = new Coordinates{Latitude = 40.7128, Longitude = -74.0060, Altitude = 0 };
        var coordinates = new List<Coordinates>
        {
            coord,
            new Coordinates{Latitude = coord.Latitude, Longitude = coord.Longitude, Altitude = 0},
            new Coordinates{Latitude = coord.Latitude, Longitude = coord.Longitude, Altitude = 0}
        };

        // Act
        var distances = _service.CalculateDistances(coordinates);

        // Assert
        foreach (var record in distances)
        {
            Assert.True(record == 0);
        }
    }

    #endregion
}
