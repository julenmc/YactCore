using Moq;
using Yact.Domain.Entities.Climb;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

namespace Yact.Domain.Tests.Services.Analyzer.RouteAnalyzer.Climbs;

public class ClimbMatcherServiceTests
{
    private readonly Mock<IClimbRepository> _mockClimbRepository;
    private readonly ClimbMatcherService _service;

    public ClimbMatcherServiceTests()
    {
        _mockClimbRepository = new Mock<IClimbRepository>();
        _service = new ClimbMatcherService( _mockClimbRepository.Object );
    }

    [Fact]
    public async Task MatchClimbWithRepositoryAsync_WithClimbInRepo_CallsRepositoryGet()
    {
        // Arrange
        var climb = CreateSampleActivityClimb();
        var climbList = new List<ClimbData>()
        {
            CreateRepoClimb(1)
        };
        _mockClimbRepository
            .Setup(x => x.GetByCoordinatesAsync(
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>()))
            .ReturnsAsync(climbList);

        // Act
        await _service.MatchClimbWithRepositoryAsync(climb);

        // Assert
        _mockClimbRepository.Verify(x => x.GetByCoordinatesAsync(
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<float>()),
            Times.Once());
        _mockClimbRepository.Verify(x => x.AddAsync(
            It.IsAny<ClimbData>()),
            Times.Never);
    }

    [Fact]
    public async Task MatchClimbWithRepositoryAsync_WithClimbInRepo_ReturnsCorrectClimbId()
    {
        // Arrange
        var climb = CreateSampleActivityClimb();
        var climbList = new List<ClimbData>()
        {
            CreateRepoClimb(1)
        };
        _mockClimbRepository
            .Setup(x => x.GetByCoordinatesAsync(
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>()))
            .ReturnsAsync(climbList);

        // Act
        await _service.MatchClimbWithRepositoryAsync(climb);

        // Assert
        Assert.Equal(climbList.First().Id, climb.ClimbId);
    }

    [Fact]
    public async Task MatchClimbWithRepositoryAsync_WithNoClimbInRepo_ClimbIdIsZero()
    {
        // Arrange
        var climb = CreateSampleActivityClimb();
        var newClimb = CreateRepoClimb(1);
        _mockClimbRepository
            .Setup(x => x.GetByCoordinatesAsync(
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>()))
            .ReturnsAsync(new List<ClimbData>());
        _mockClimbRepository
            .Setup(x => x.AddAsync(It.IsAny<ClimbData>()))
            .ReturnsAsync(newClimb);

        // Act
        await _service.MatchClimbWithRepositoryAsync(climb);

        // Assert
        Assert.Equal(0, climb.ClimbId);
    }

    #region Helper Methods

    private ActivityClimb CreateSampleActivityClimb()
    {
        return new ActivityClimb
        {
            Id = 0,
            ActivityId = 0,
            ClimbId = 0,
            IntervalId = 0,
            Data = new ClimbData
            {
                Id = 0,
                LatitudeInit = 40.0,
                LatitudeEnd = 40.1,
                LongitudeInit = -74.0,
                LongitudeEnd = -74.1,
                AltitudeInit = 100,
                AltitudeEnd = 200,
                Metrics = new ClimbMetrics
                {
                    DistanceMeters = 1000,
                    Elevation = 100,
                    Slope = 10,
                    MaxSlope = 12
                }
            },
            StartPointMeters = 0
        };
    }

    private ClimbData CreateRepoClimb(int id)
    {
        return new ClimbData
        {
            Id = id,
            Name = $"Climb {id}",
            LatitudeInit = 40.0,
            LatitudeEnd = 40.1,
            LongitudeInit = -74.0,
            LongitudeEnd = -74.1,
            AltitudeInit = 100,
            AltitudeEnd = 200,
            Metrics = new ClimbMetrics
            {
                DistanceMeters = 1000,
                Elevation = 100,
                Slope = 10,
                MaxSlope = 12
            }
        };
    }

    #endregion
}
