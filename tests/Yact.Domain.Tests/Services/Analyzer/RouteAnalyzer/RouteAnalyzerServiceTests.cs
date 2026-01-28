//using Yact.Domain.Entities.Climb;
//using Yact.Domain.Services.Analyzer.RouteAnalyzer;
//using Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;
//using Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;
//using Yact.Domain.Services.Utils.Smoothers.Altitude;
//using Yact.Domain.ValueObjects.Activity.Records;

//namespace Yact.Domain.Tests.Services.Analyzer.RouteAnalyzer;

//public class RouteAnalyzerServiceTests
//{
//    private readonly Mock<IClimbFinderService> _mockClimbFinderService;
//    private readonly Mock<IClimbMatcherService> _mockClimbMatcherService;
//    private readonly Mock<IDistanceCalculator> _mockDistanceCalculator;
//    private readonly Mock<IAltitudeSmootherService> _mockAltitudeSmootherService;
//    private readonly RouteAnalyzerService _service;

//    public RouteAnalyzerServiceTests()
//    {
//        _mockClimbFinderService = new Mock<IClimbFinderService>();
//        _mockClimbMatcherService = new Mock<IClimbMatcherService>();
//        _mockDistanceCalculator = new Mock<IDistanceCalculator>();
//        _mockAltitudeSmootherService = new Mock<IAltitudeSmootherService>();

//        _service = new RouteAnalyzerService(
//            _mockClimbFinderService.Object,
//            _mockClimbMatcherService.Object,
//            _mockDistanceCalculator.Object,
//            _mockAltitudeSmootherService.Object);
//    }

//    #region GetRouteDistances Tests

//    [Fact]
//    public void GetRouteDistances_WithValidRecords_CallsDistanceCalculator()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } },
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.1, Longitude = -74.1, Altitude = 150 } }
//        };

//        // Act
//        _service.GetRouteDistances(records);

//        // Assert
//        _mockDistanceCalculator.Verify(x => x.CalculateDistances(records), Times.Once);
//    }

//    [Fact]
//    public void GetRouteDistances_WithValidRecords_CallsAltitudeSmoother()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } },
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.1, Longitude = -74.1, Altitude = 150 } }
//        };

//        // Act
//        _service.GetRouteDistances(records);

//        // Assert
//        _mockAltitudeSmootherService.Verify(x => x.Smooth(records), Times.Once);
//    }

//    [Fact]
//    public void GetRouteDistances_CallsDistanceCalculatorBeforeSmoother()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } }
//        };
//        var callOrder = new List<string>();

//        _mockDistanceCalculator
//            .Setup(x => x.CalculateDistances(It.IsAny<List<RecordData>>()))
//            .Callback(() => callOrder.Add("DistanceCalculator"));

//        _mockAltitudeSmootherService
//            .Setup(x => x.Smooth(It.IsAny<List<RecordData>>()))
//            .Callback(() => callOrder.Add("AltitudeSmoother"));

//        // Act
//        _service.GetRouteDistances(records);

//        // Assert
//        Assert.Equal(new[] { "DistanceCalculator", "AltitudeSmoother" }, callOrder);
//    }

//    [Fact]
//    public void GetRouteDistances_WithEmptyRecords_CallsBothServices()
//    {
//        // Arrange
//        var records = new List<RecordData>();

//        // Act
//        _service.GetRouteDistances(records);

//        // Assert
//        _mockDistanceCalculator.Verify(x => x.CalculateDistances(records), Times.Once);
//        _mockAltitudeSmootherService.Verify(x => x.Smooth(records), Times.Once);
//    }

//    [Fact]
//    public void GetRouteDistances_WithSingleRecord_CallsBothServices()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } }
//        };

//        // Act
//        _service.GetRouteDistances(records);

//        // Assert
//        _mockDistanceCalculator.Verify(x => x.CalculateDistances(records), Times.Once);
//        _mockAltitudeSmootherService.Verify(x => x.Smooth(records), Times.Once);
//    }

//    #endregion

//    #region AnalyzeRouteAsync Tests

//    [Fact]
//    public async Task AnalyzeRouteAsync_WithValidRecords_CallsGetRouteDistances()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } }
//        };
//        _mockClimbFinderService.Setup(x => x.FindClimbs(It.IsAny<List<RecordData>>()))
//            .Returns(new List<ActivityClimb>());

//        // Act
//        await _service.AnalyzeRouteAsync(records);

//        // Assert
//        _mockDistanceCalculator.Verify(x => x.CalculateDistances(records), Times.Once);
//        _mockAltitudeSmootherService.Verify(x => x.Smooth(records), Times.Once);
//    }

//    [Fact]
//    public async Task AnalyzeRouteAsync_WithValidRecords_CallsClimbFinder()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } }
//        };
//        _mockClimbFinderService.Setup(x => x.FindClimbs(It.IsAny<List<RecordData>>()))
//            .Returns(new List<ActivityClimb>());

//        // Act
//        await _service.AnalyzeRouteAsync(records);

//        // Assert
//        _mockClimbFinderService.Verify(x => x.FindClimbs(records), Times.Once);
//    }

//    [Fact]
//    public async Task AnalyzeRouteAsync_NoClimbsFound_ReturnsEmptyList()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } }
//        };
//        var emptyClimbList = new List<ActivityClimb>();
//        _mockClimbFinderService.Setup(x => x.FindClimbs(It.IsAny<List<RecordData>>()))
//            .Returns(emptyClimbList);

//        // Act
//        var result = await _service.AnalyzeRouteAsync(records);

//        // Assert
//        Assert.Empty(result);
//        _mockClimbMatcherService.Verify(x => x.MatchClimbWithRepositoryAsync(It.IsAny<ActivityClimb>()), Times.Never);
//    }

//    [Fact]
//    public async Task AnalyzeRouteAsync_WithSingleClimb_ReturnsClimbFromFinder()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } }
//        };
//        var climb = CreateSampleActivityClimb(1);
//        var climbs = new List<ActivityClimb> { climb };
        
//        _mockClimbFinderService.Setup(x => x.FindClimbs(It.IsAny<List<RecordData>>()))
//            .Returns(climbs);
//        _mockClimbMatcherService.Setup(x => x.MatchClimbWithRepositoryAsync(It.IsAny<ActivityClimb>()));

//        // Act
//        var result = await _service.AnalyzeRouteAsync(records);

//        // Assert
//        Assert.Single(result);
//        _mockClimbMatcherService.Verify(x => x.MatchClimbWithRepositoryAsync(climb), Times.Once);
//        Assert.Equal(climb.ClimbId, climbs.First().ClimbId);
//        Assert.Equal(climb.Data.Id, climbs.First().Data.Id);
//    }

//    [Fact]
//    public async Task AnalyzeRouteAsync_WithMultipleClimbs_ReturnsAllClimbsFromFinder()
//    {
//        // Arrange
//        var records = new List<RecordData>
//        {
//            new RecordData { Coordinates = new CoordinatesData { Latitude = 40.0, Longitude = -74.0, Altitude = 100 } }
//        };
//        var climbs = new List<ActivityClimb>
//        {
//            CreateSampleActivityClimb(1),
//            CreateSampleActivityClimb(2),
//            CreateSampleActivityClimb(3)
//        };
        
//        _mockClimbFinderService.Setup(x => x.FindClimbs(It.IsAny<List<RecordData>>()))
//            .Returns(climbs);
//        _mockClimbMatcherService.Setup(x => x.MatchClimbWithRepositoryAsync(It.IsAny<ActivityClimb>()));

//        // Act
//        var result = await _service.AnalyzeRouteAsync(records);

//        // Assert
//        Assert.Equal(3, result.Count);
//        _mockClimbMatcherService.Verify(x => x.MatchClimbWithRepositoryAsync(It.IsAny<ActivityClimb>()), Times.Exactly(3));
//        for (int i = 0; i < result.Count; i++) 
//        {
//            Assert.Equal(climbs[i].ClimbId, result[i].ClimbId);
//            Assert.Equal(climbs[i].Data.Id, result[i].Data.Id);
//        }
//    }

//    [Fact]
//    public async Task AnalyzeRouteAsync_WithEmptyRecords_ReturnsEmptyList()
//    {
//        // Arrange
//        var records = new List<RecordData>();
        
//        _mockClimbFinderService.Setup(x => x.FindClimbs(It.IsAny<List<RecordData>>()))
//            .Returns(new List<ActivityClimb>());

//        // Act
//        var result = await _service.AnalyzeRouteAsync(records);

//        // Assert
//        Assert.Empty(result);
//        _mockDistanceCalculator.Verify(x => x.CalculateDistances(records), Times.Once);
//        _mockAltitudeSmootherService.Verify(x => x.Smooth(records), Times.Once);
//    }

//    #endregion

//    #region GetDebugTrace Tests

//    [Fact]
//    public void GetDebugTrace_ReturnsDebugTraceFromClimbFinder()
//    {
//        // Arrange
//        var expectedTrace = new List<string>
//        {
//            "Debug line 1",
//            "Debug line 2",
//            "Debug line 3"
//        };
        
//        _mockClimbFinderService.Setup(x => x.GetDebugTrace())
//            .Returns(expectedTrace);

//        // Act
//        var result = _service.GetDebugTrace();

//        // Assert
//        Assert.Equal(expectedTrace, result);
//    }

//    [Fact]
//    public void GetDebugTrace_CallsClimbFinderGetDebugTrace()
//    {
//        // Arrange
//        _mockClimbFinderService.Setup(x => x.GetDebugTrace())
//            .Returns(new List<string>());

//        // Act
//        _service.GetDebugTrace();

//        // Assert
//        _mockClimbFinderService.Verify(x => x.GetDebugTrace(), Times.Once);
//    }

//    [Fact]
//    public void GetDebugTrace_ReturnsEmptyList()
//    {
//        // Arrange
//        var emptyTrace = new List<string>();
        
//        _mockClimbFinderService.Setup(x => x.GetDebugTrace())
//            .Returns(emptyTrace);

//        // Act
//        var result = _service.GetDebugTrace();

//        // Assert
//        Assert.Empty(result);
//    }

//    #endregion

//    #region Helper Methods

//    private ActivityClimb CreateSampleActivityClimb(int id)
//    {
//        return new ActivityClimb
//        {
//            Id = 0,
//            ActivityId = 1,
//            ClimbId = id,
//            IntervalId = 0,
//            Data = new ClimbData
//            {
//                Id = id,
//                Name = $"Climb {id}",
//                LatitudeInit = 40.0,
//                LatitudeEnd = 40.1,
//                LongitudeInit = -74.0,
//                LongitudeEnd = -74.1,
//                AltitudeInit = 100,
//                AltitudeEnd = 200,
//                Metrics = new ClimbMetrics
//                {
//                    DistanceMeters = 1000,
//                    Elevation = 100,
//                    Slope = 10,
//                    MaxSlope = 12
//                }
//            },
//            StartPointMeters = 0
//        };
//    }

//    #endregion
//}
