using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Domain.Tests.Services.Analyzer.RouteAnalyzer.Climbs;

public partial class ClimbFinderServiceTest
{
    #region Single Irregular Climbs

    [Fact]
    public void FindClimbs_SingularIrregularClimb_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { 
                DistanceMeters = 0, 
                SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 },
                Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 },
                Timestamp = DateTime.Now},
            new RecordData() { 
                DistanceMeters = 1000, 
                SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 },
                Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 },
                Timestamp = DateTime.Now },
            new RecordData() { 
                DistanceMeters = 2000, 
                SmoothedAltitude = new SmoothedAltitude { Altitude = 250, Slope = 5 },
                Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 },
                Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_SingleIrregularClimb_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() { 
                DistanceMeters = 0, 
                SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 },
                Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 },
                Timestamp = DateTime.Now},
            new RecordData() { 
                DistanceMeters = 1000, 
                SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 },
                Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 },
                Timestamp = DateTime.Now},
            new RecordData() {
                DistanceMeters = 2000, 
                SmoothedAltitude = new SmoothedAltitude { Altitude = 250, Slope = 5 }, 
                Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, 
                Timestamp = DateTime.Now},
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(2000, climb.Metrics.DistanceMeters);
        Assert.Equal(150, climb.Metrics.NetElevationMeters);
        Assert.Equal(7.5f, climb.Metrics.Slope, 2);
        Assert.Equal(10f, climb.Metrics.MaxSlope, 2);
    }

    #endregion

    #region Climb Basic Stops 

    [Fact]
    public void FindClimbs_FlatStop_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1000, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1100, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2100, SmoothedAltitude = new SmoothedAltitude { Altitude = 300, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_FlatStop_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1000, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1100, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2100, SmoothedAltitude = new SmoothedAltitude { Altitude = 300, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(2100, climb.Metrics.DistanceMeters);
        Assert.Equal(200, climb.Metrics.NetElevationMeters);
        Assert.Equal(9.5f, climb.Metrics.Slope, 1);
        Assert.Equal(10f, climb.Metrics.MaxSlope, 2);
    }

    [Fact]
    public void FindClimbs_DownStop_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1000, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1100, SmoothedAltitude = new SmoothedAltitude { Altitude = 190, Slope = -10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2100, SmoothedAltitude = new SmoothedAltitude { Altitude = 290, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_DownStop_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1000, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1100, SmoothedAltitude = new SmoothedAltitude { Altitude = 190, Slope = -10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2100, SmoothedAltitude = new SmoothedAltitude { Altitude = 290, Slope = 10 }, Coordinates = new Coordinates { Longitude = 10, Latitude = 10, Altitude = 10 }, Timestamp = DateTime.Now},
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(2100, climb.Metrics.DistanceMeters);
        Assert.Equal(200, climb.Metrics.TotalElevationMeters);
        Assert.Equal(9.0f, climb.Metrics.Slope, 1);
        Assert.Equal(10f, climb.Metrics.MaxSlope, 2);
    }

    [Fact]
    public void FindClimbs_DobleStop_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1000, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1100, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2100, SmoothedAltitude = new SmoothedAltitude { Altitude = 300, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2200, SmoothedAltitude = new SmoothedAltitude { Altitude = 300, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 3200, SmoothedAltitude = new SmoothedAltitude { Altitude = 400, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_DobleStop_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude { Altitude = 100, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1000, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 1100, SmoothedAltitude = new SmoothedAltitude { Altitude = 200, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2100, SmoothedAltitude = new SmoothedAltitude { Altitude = 300, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 2200, SmoothedAltitude = new SmoothedAltitude { Altitude = 300, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 3200, SmoothedAltitude = new SmoothedAltitude { Altitude = 400, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(3200, climb.Metrics.DistanceMeters);
        Assert.Equal(300, climb.Metrics.NetElevationMeters);
        Assert.Equal(9.4f, climb.Metrics.Slope, 1);
        Assert.Equal(10f, climb.Metrics.MaxSlope, 2);
    }

    #endregion

    #region Edge Single Climb Cases

    public static IEnumerable<object[]> EdgeFlatData()  // Climb distance before flat / Flat distance. 
    {
        yield return new object[] { 1900f, 500f };
        yield return new object[] { 3900f, 800f };
        yield return new object[] { 5900f, 1000f };
        yield return new object[] { 9900f, 1500f };
        yield return new object[] { 14900f, 2000f };
        yield return new object[] { 20000f, 2900f };
    }

    [Theory]
    [MemberData(nameof(EdgeFlatData))]
    public void FindClimbs_EdgeFlatStop_ReturnsCorrectNumberOfClimbs(float distanceBeforeFlat, float flatDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var firstSectorLen = distanceBeforeFlat;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var secondSectorLen = 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;
        var totalSlope = (firstSectorEle + secondSectorEle) / (firstSectorLen + flatDistance + secondSectorLen) * 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0 , Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now },
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle , Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now },
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle , Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now },
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Theory]
    [MemberData(nameof(EdgeFlatData))]
    public void FindClimbs_EdgeFlatStop_ReturnsCorrectMetrics(float distanceBeforeFlat, float flatDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var firstSectorLen = distanceBeforeFlat != 0 ? distanceBeforeFlat : 1000;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var secondSectorLen = 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;
        var totalSlope = (firstSectorEle + secondSectorEle) / (firstSectorLen + flatDistance + secondSectorLen) * 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(firstSectorLen + flatDistance + secondSectorLen, climb.Metrics.DistanceMeters, 0);
        Assert.Equal(firstSectorEle + secondSectorEle, climb.Metrics.NetElevationMeters, 0);
        Assert.Equal(totalSlope, climb.Metrics.Slope, 1);
        Assert.Equal(climbSectorsSlope, climb.Metrics.MaxSlope, 2);
    }

    public static IEnumerable<object[]> EdgeDownData()  // Climb distance before downhill / Downhill distance. 
    {
        yield return new object[] { 1900f, 200f };
        yield return new object[] { 3900f, 600f };
        yield return new object[] { 5900f, 800f };
        yield return new object[] { 9900f, 900f };
        yield return new object[] { 14900f, 1200f };
        yield return new object[] { 20000f, 2000f };
    }

    [Theory]
    [MemberData(nameof(EdgeDownData))]
    public void FindClimbs_EdgeDownStop_ReturnsCorrectNumberOfClimbs(float distanceBeforeDown, float downDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var downSectorSlope = -5f;
        var elevationLost = downDistance * downSectorSlope / 100;
        var firstSectorLen = distanceBeforeDown;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var downSectorEle = firstSectorEle + elevationLost;
        var secondSectorLen = downDistance + 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;
        var totalSlope = (firstSectorEle + secondSectorEle + elevationLost) / (firstSectorLen + downDistance + secondSectorLen) * 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle, Slope = downSectorSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = climbSectorsSlope, Latitude = climbSectorsSlope, Altitude = climbSectorsSlope }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Theory]
    [MemberData(nameof(EdgeDownData))]
    public void FindClimbs_EdgeDownStop_ReturnsCorrectMetrics(float distanceBeforeDown, float downDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var downSectorSlope = -5f;
        var elevationLost = downDistance * downSectorSlope / 100;
        var firstSectorLen = distanceBeforeDown != 0 ? distanceBeforeDown : 1000;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var downSectorEle = firstSectorEle + elevationLost;
        var secondSectorLen = downDistance + 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;
        var totalSlope = (firstSectorEle + secondSectorEle + elevationLost) / (firstSectorLen + downDistance + secondSectorLen) * 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle , Slope = downSectorSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(firstSectorLen + downDistance + secondSectorLen, climb.Metrics.DistanceMeters, 0);
        Assert.Equal(firstSectorEle + secondSectorEle + elevationLost, climb.Metrics.NetElevationMeters, 0);
        Assert.Equal(totalSlope, climb.Metrics.Slope, 1);
        Assert.Equal(climbSectorsSlope, climb.Metrics.MaxSlope, 2);
    }

    #endregion

    #region Edge Multiple Climbs Cases  

    public static IEnumerable<object[]> EdgeFlatTwoClimbData()  // Climb distance before flat / Flat distance. 
    {
        yield return new object[] { 1900f, 510f };
        yield return new object[] { 3900f, 810f };
        yield return new object[] { 5900f, 1010f };
        yield return new object[] { 9900f, 1510f };
        yield return new object[] { 14900f, 2010f };
        yield return new object[] { 25000f, 3010f };
    }

    [Theory]
    [MemberData(nameof(EdgeFlatTwoClimbData))]
    public void FindClimbs_EdgeFlatStop_ReturnsTwoClimbs(float distanceBeforeFlat, float flatDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var firstSectorLen = distanceBeforeFlat;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var secondSectorLen = 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(2, climbList?.Count);
    }

    [Theory]
    [MemberData(nameof(EdgeFlatTwoClimbData))]
    public void FindClimbs_EdgeFlatStop_ReturnsTwoClimbsCorrectMetrics(float distanceBeforeFlat, float flatDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var firstSectorLen = distanceBeforeFlat;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var secondSectorLen = 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + flatDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        // First climb
        var climb = climbList[0];
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(firstSectorLen, climb.Metrics.DistanceMeters, 0);
        Assert.Equal(firstSectorEle, climb.Metrics.NetElevationMeters, 0);
        Assert.Equal(climbSectorsSlope, climb.Metrics.Slope, 2);
        Assert.Equal(climbSectorsSlope, climb.Metrics.MaxSlope, 2);
        // Second climb
        climb = climbList[1];
        Assert.Equal(firstSectorLen + flatDistance, climb.StartPointMeters, 0);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(secondSectorLen, climb.Metrics.DistanceMeters, 0);
        Assert.Equal(secondSectorEle, climb.Metrics.NetElevationMeters, 0);
        Assert.Equal(climbSectorsSlope, climb.Metrics.Slope, 2);
        Assert.Equal(climbSectorsSlope, climb.Metrics.MaxSlope, 2);
    }

    public static IEnumerable<object[]> EdgeDownTwoClimbData()  // Climb distance before downhill / Downhill distance. 
    {
        yield return new object[] { 1900f, 210f };
        yield return new object[] { 3900f, 610f };
        yield return new object[] { 5900f, 810f };
        yield return new object[] { 9900f, 1010f };
        yield return new object[] { 14900f, 1210f };
        yield return new object[] { 25000f, 2010f };
    }

    [Theory]
    [MemberData(nameof(EdgeDownTwoClimbData))]
    public void FindClimbs_EdgeDownStop_ReturnsTwoClimbs(float distanceBeforeDown, float downDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var downSectorSlope = -5f;
        var elevationLost = downDistance * downSectorSlope / 100;
        var firstSectorLen = distanceBeforeDown;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var downSectorEle = firstSectorEle + elevationLost;
        var secondSectorLen = downDistance + 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle, Slope = downSectorSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(2, climbList?.Count);
    }

    [Theory]
    [MemberData(nameof(EdgeDownTwoClimbData))]
    public void FindClimbs_EdgeDownStop_ReturnsTwoClimbsCorrectMetrics(float distanceBeforeDown, float downDistance)
    {
        // Arrange
        var climbSectorsSlope = 8f;
        var downSectorSlope = -5f;
        var elevationLost = downDistance * downSectorSlope / 100;
        var firstSectorLen = distanceBeforeDown != 0 ? distanceBeforeDown : 1000;
        var firstSectorEle = firstSectorLen * climbSectorsSlope / 100;
        var downSectorEle = firstSectorEle + elevationLost;
        var secondSectorLen = downDistance + 1000;
        var secondSectorEle = secondSectorLen * climbSectorsSlope / 100;

        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = firstSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle, Slope = downSectorSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = firstSectorLen + downDistance + secondSectorLen, SmoothedAltitude = new SmoothedAltitude() { Altitude = downSectorEle + secondSectorEle, Slope = climbSectorsSlope }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        // First climb
        var climb = climbList[0];
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(firstSectorLen, climb.Metrics.DistanceMeters, 0);
        Assert.Equal(firstSectorEle, climb.Metrics.NetElevationMeters, 0);
        Assert.Equal(climbSectorsSlope, climb.Metrics.Slope, 2);
        Assert.Equal(climbSectorsSlope, climb.Metrics.MaxSlope, 2);
        // Second climb
        climb = climbList[1];
        Assert.Equal(firstSectorLen + downDistance, climb.StartPointMeters, 0);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(secondSectorLen, climb.Metrics.DistanceMeters, 0);
        Assert.Equal(secondSectorEle, climb.Metrics.NetElevationMeters, 0);
        Assert.Equal(climbSectorsSlope, climb.Metrics.Slope, 2);
        Assert.Equal(climbSectorsSlope, climb.Metrics.MaxSlope, 2);
    }

    #endregion

    #region Edge Mix Cases

    // Checks that a short downhill after the flat doesn't have any effect on the detection
    [Fact]
    public void FindClimbs_EdgeFlatDownStop_ReturnsCorrectNumberOfClimbs()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 10000, SmoothedAltitude = new SmoothedAltitude() { Altitude = 800, Slope = 8 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 11400, SmoothedAltitude = new SmoothedAltitude() { Altitude = 800, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 11500, SmoothedAltitude = new SmoothedAltitude() { Altitude = 792, Slope = -10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 13500, SmoothedAltitude = new SmoothedAltitude() { Altitude = 952, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climbList = _service.FindClimbs(records);

        // Assert
        Assert.NotNull(climbList);
        Assert.Equal(1, climbList?.Count);
    }

    [Fact]
    public void FindClimbs_EdgeFlatDownStop_ReturnsCorrectMetrics()
    {
        // Arrange
        List<RecordData> records = new List<RecordData>()
        {
            new RecordData() {DistanceMeters = 0, SmoothedAltitude = new SmoothedAltitude() { Altitude = 0, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 10000, SmoothedAltitude = new SmoothedAltitude() { Altitude = 800, Slope = 8 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 11400, SmoothedAltitude = new SmoothedAltitude() { Altitude = 800, Slope = 0 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 11500, SmoothedAltitude = new SmoothedAltitude() { Altitude = 792, Slope = -10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
            new RecordData() {DistanceMeters = 13500, SmoothedAltitude = new SmoothedAltitude() { Altitude = 952, Slope = 10 }, Coordinates = new Coordinates { Longitude = 0, Latitude = 0, Altitude = 0 }, Timestamp = DateTime.Now},
        };

        // Act
        var climb = _service.FindClimbs(records)[0];

        // Assert
        Assert.NotNull(climb);
        Assert.Equal(0, climb.StartPointMeters);
        Assert.NotNull(climb.Metrics);
        Assert.Equal(13500, climb.Metrics.DistanceMeters, 0);
        Assert.Equal(960, climb.Metrics.TotalElevationMeters, 0);
        Assert.Equal(7.1f, climb.Metrics.Slope, 1);
        Assert.Equal(10, climb.Metrics.MaxSlope, 2);
    }

    #endregion
}
