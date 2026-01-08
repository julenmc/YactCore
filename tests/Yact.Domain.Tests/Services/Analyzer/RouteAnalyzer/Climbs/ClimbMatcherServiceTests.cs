using Moq;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;
using Yact.Domain.ValueObjects.Climb;

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
        var climb = CreateSampleClimbDetails();
        var guid = Guid.NewGuid();
        var climbList = new List<Climb>()
        {
            CreateRepoClimb(guid)
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
            It.IsAny<Climb>()),
            Times.Never);
    }

    [Fact]
    public async Task MatchClimbWithRepositoryAsync_WithClimbInRepo_ReturnsCorrectClimbId()
    {
        // Arrange
        var climb = CreateSampleClimbDetails();
        var guid = Guid.NewGuid();
        var climbList = new List<Climb>()
        {
            CreateRepoClimb(guid)
        };
        _mockClimbRepository
            .Setup(x => x.GetByCoordinatesAsync(
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>()))
            .ReturnsAsync(climbList);

        // Act
        var found = await _service.MatchClimbWithRepositoryAsync(climb);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(climbList.First().Id, found.Id);
    }

    [Fact]
    public async Task MatchClimbWithRepositoryAsync_WithNoClimbInRepo_ClimbIdIsZero()
    {
        // Arrange
        var climb = CreateSampleClimbDetails();
        var guid = Guid.NewGuid();
        var newClimb = CreateRepoClimb(guid);
        _mockClimbRepository
            .Setup(x => x.GetByCoordinatesAsync(
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<float>()))
            .ReturnsAsync(new List<Climb>());
        _mockClimbRepository
            .Setup(x => x.AddAsync(It.IsAny<Climb>()))
            .ReturnsAsync(newClimb);

        // Act
        var found = await _service.MatchClimbWithRepositoryAsync(climb);

        // Assert
        Assert.Null(found);
    }

    #region Helper Methods

    private ClimbDetails CreateSampleClimbDetails()
    {
        return new ClimbDetails
        {
            Coordinates = new ClimbCoordinates
            {
                LatitudeInit = 40.0,
                LatitudeEnd = 40.1,
                LongitudeInit = -74.0,
                LongitudeEnd = -74.1,
                AltitudeInit = 100,
                AltitudeEnd = 200,
            },
            Metrics = new ClimbMetrics
            {
                DistanceMeters = 1000,
                NetElevationMeters = 100,
                Slope = 10,
                MaxSlope = 12
            },
            StartPointMeters = 0
        };
    }

    private Climb CreateRepoClimb(Guid id)
    {
        return Climb.Load(
            id: ClimbId.From(id),
            data: CreateSampleClimbDetails(),
            summary: new ClimbSummary($"Climb {id}"));
    }

    #endregion
}
