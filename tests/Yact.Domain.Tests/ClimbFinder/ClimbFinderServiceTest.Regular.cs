using System.Security.Claims;
using Yact.Domain.Entities.Activity;
using Yact.Domain.Entities.Climb;
using Yact.Domain.Services.ClimbFinder;

namespace Yact.Domain.Tests.ClimbFinder;

public partial class ClimbFinderServiceTest
{
    private readonly ClimbFinderService _service = new();

    #region ErrorCases

    [Fact]
    public void FindClimbs_NoRecords_ThrowsException()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.FindClimbs(records));
        Assert.Contains("No records", exception.Message);
    }

    #endregion

    #region Basic Functionalities

    [Fact]
    public void FindClimbs_FlatRoute_NoClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(0, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_SingleClimb_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_SingleClimb_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.Equal(2000, climb.EndPointMeters);
        Assert.NotNull(climb.Data.Metrics);
        Assert.Equal(2000, climb.Data.Metrics.DistanceMeters);
        Assert.Equal(200, climb.Data.Metrics.Elevation);
        Assert.Equal(10, climb.Data.Metrics.Slope);
        Assert.Equal(10, climb.Data.Metrics.MaxSlope);
    }

    [Fact]
    public void GetDebugTrace_FlatRoute_ReturnsTrace()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
        };

        // Act
        _service.FindClimbs(records);
        var debugTrace = _service.GetDebugTrace();

        // Assert
        Assert.NotNull(debugTrace);
        Assert.NotEqual(0, debugTrace?.Count);
    }

    [Fact]
    public void GetDebugTrace_FlatRoute_TraceIsReset()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
        };

        // Act
        _service.FindClimbs(records);
        var debugTrace1 = _service.GetDebugTrace();
        _service.FindClimbs(records);
        var debugTrace2 = _service.GetDebugTrace();

        // Assert
        // Both debug traces must have the same values. Once "FindClimbs" is called,
        // the previous trace must be deleted
        Assert.NotNull(debugTrace1);
        Assert.NotNull(debugTrace1);
        Assert.Equal(debugTrace2?.Count, debugTrace1?.Count);
        for (int i = 0; i < debugTrace1?.Count; i++)
        {
            Assert.Equal(debugTrace1[i], debugTrace2![i]);
        }
    }

    #endregion

    #region Single Regular Climbs

    [Fact]
    public void FindClimbs_SingleUpDown_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = -10 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = -10 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_SingleUpDown_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = -10 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = -10 },
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.Equal(2000, climb.EndPointMeters);
        Assert.NotNull(climb.Data.Metrics);
        Assert.Equal(2000, climb.Data.Metrics.DistanceMeters);
        Assert.Equal(200, climb.Data.Metrics.Elevation);
        Assert.Equal(10, climb.Data.Metrics.Slope);
        Assert.Equal(10, climb.Data.Metrics.MaxSlope);
    }

    [Fact]
    public void FindClimbs_SingleUpFlat_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 0 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 0 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_SingleUpFlat_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 0 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 0 },
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.Equal(2000, climb.EndPointMeters);
        Assert.NotNull(climb.Data.Metrics);
        Assert.Equal(2000, climb.Data.Metrics.DistanceMeters);
        Assert.Equal(200, climb.Data.Metrics.Elevation);
        Assert.Equal(10, climb.Data.Metrics.Slope);
        Assert.Equal(10, climb.Data.Metrics.MaxSlope);
    }

    #endregion

    #region Multiple Regular Climbs

    [Fact]
    public void FindClimbs_TwoEqualClimbs_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = -10 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = -10 },
            new RecordData() { DistanceMeters = 5000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 6000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(2, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_TwoEqualClimbs_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = -10 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = -10 },
            new RecordData() { DistanceMeters = 5000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 6000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(0, climbList[0].StartPointMeters);
        Assert.Equal(2000, climbList[0].EndPointMeters);
        Assert.Equal(4000, climbList[1].StartPointMeters);
        Assert.Equal(6000, climbList[1].EndPointMeters);
        // Both climbs metrics are equal, easier to test
        foreach (ActivityClimb climb in  climbList)
        {
            Assert.NotNull(climb.Data.Metrics);
            Assert.Equal(2000, climb.Data.Metrics.DistanceMeters);
            Assert.Equal(200, climb.Data.Metrics.Elevation);
            Assert.Equal(10, climb.Data.Metrics.Slope);
            Assert.Equal(10, climb.Data.Metrics.MaxSlope);
        }
    }

    [Fact]
    public void FindClimbs_TwoDifferentClimbs_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = -10 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = -10 },
            new RecordData() { DistanceMeters = 5000, Coordinates = new CoordinatesData() { Altitude = 250 }, Slope = 15 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(2, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_TwoDifferentClimbs_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 3000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = -10 },
            new RecordData() { DistanceMeters = 4000, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = -10 },
            new RecordData() { DistanceMeters = 5000, Coordinates = new CoordinatesData() { Altitude = 250 }, Slope = 15 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        // First climb
        var climb = climbList[0];
        Assert.Equal(0, climb.StartPointMeters);
        Assert.Equal(2000, climb.EndPointMeters);
        Assert.NotNull(climb.Data.Metrics);
        Assert.Equal(2000, climb.Data.Metrics.DistanceMeters);
        Assert.Equal(200, climb.Data.Metrics.Elevation);
        Assert.Equal(10, climb.Data.Metrics.Slope);
        Assert.Equal(10, climb.Data.Metrics.MaxSlope);
        // Second climb
        climb = climbList[1];
        Assert.Equal(4000, climb.StartPointMeters);
        Assert.Equal(5000, climb.EndPointMeters);
        Assert.NotNull(climb.Data.Metrics);
        Assert.Equal(1000, climb.Data.Metrics.DistanceMeters);
        Assert.Equal(150, climb.Data.Metrics.Elevation);
        Assert.Equal(15, climb.Data.Metrics.Slope);
        Assert.Equal(15, climb.Data.Metrics.MaxSlope);
    }

    #endregion

    #region Edge Cases

    public static IEnumerable<object[]> EdgeClimbData()
    {
        yield return new object[] { 60.0f, 10.0f };
        yield return new object[] { 100.0f, 8.0f };
        yield return new object[] { 150.0f, 7.0f };
        yield return new object[] { 250.0f, 6.0f };
        yield return new object[] { 500.0f, 5.0f };
        yield return new object[] { 1000.0f, 4.0f };
        yield return new object[] { 2000.0f, 2.5f };
    }

    [Theory]
    [MemberData(nameof(EdgeClimbData))]
    public void FindClimbs_EdgeRegular_ReturnsCorrectNumberOfClimbs(float distance, float slope)
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 0 }, Slope = 0 },
            new RecordData() { DistanceMeters = distance, Coordinates = new CoordinatesData() { Altitude = distance * slope / 100 }, Slope = slope },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Theory]
    [MemberData(nameof(EdgeClimbData))]
    public void FindClimbs_EdgeRegular_ReturnsCorrectMetrics(float distance, float slope)
    {
        // Arrange
        var elevation = distance * slope / 100;
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 0 }, Slope = 0 },
            new RecordData() { DistanceMeters = distance, Coordinates = new CoordinatesData() { Altitude = elevation }, Slope = slope },
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.Equal(distance, climb.EndPointMeters);
        Assert.NotNull(climb.Data.Metrics);
        Assert.Equal(distance, climb.Data.Metrics.DistanceMeters);
        Assert.Equal(elevation, climb.Data.Metrics.Elevation, 0);   
        Assert.Equal(slope, climb.Data.Metrics.Slope);
        Assert.Equal(slope, climb.Data.Metrics.MaxSlope);
    }

    public static IEnumerable<object[]> EdgeNoClimbData()
    {
        yield return new object[] { 50.0f, 12.0f };
        yield return new object[] { 80.0f, 9.0f };
        yield return new object[] { 125.0f, 7.5f };
        yield return new object[] { 200.0f, 6.5f };
        yield return new object[] { 400.0f, 5.0f };
        yield return new object[] { 900.0f, 4.5f };
        yield return new object[] { 2000.0f, 2.0f };
    }

    [Theory]
    [MemberData(nameof(EdgeNoClimbData))]
    public void FindClimbs_EdgeRegular_ReturnsNoClimbs(float distance, float slope)
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 0 }, Slope = 0 },
            new RecordData() { DistanceMeters = distance, Coordinates = new CoordinatesData() { Altitude = distance * slope / 100 }, Slope = slope },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(0, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_SingleClimbFinishWithWarning_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2100, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 0 },
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_SingleClimbFinishWithWarning_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { DistanceMeters = 0, Coordinates = new CoordinatesData() { Altitude = 100 }, Slope = 0 },
            new RecordData() { DistanceMeters = 1000, Coordinates = new CoordinatesData() { Altitude = 200 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2000, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 10 },
            new RecordData() { DistanceMeters = 2100, Coordinates = new CoordinatesData() { Altitude = 300 }, Slope = 0 },
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.Equal(2000, climb.EndPointMeters);
        Assert.NotNull(climb.Data.Metrics);
        Assert.Equal(2000, climb.Data.Metrics.DistanceMeters);
        Assert.Equal(200, climb.Data.Metrics.Elevation);
        Assert.Equal(10, climb.Data.Metrics.Slope);
        Assert.Equal(10, climb.Data.Metrics.MaxSlope);
    }

    #endregion
}
