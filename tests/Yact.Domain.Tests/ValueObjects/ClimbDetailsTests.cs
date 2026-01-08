using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Tests.Entities;

public class ClimbDetailsTests
{
    private static ClimbDetails _originalDetails = new ClimbDetails
    {
        Coordinates = new ClimbCoordinates
        {
            LatitudeInit = 0.0,
            LatitudeEnd = 0.009,
            LongitudeInit = 0.0,
            LongitudeEnd = 0.0,
            AltitudeInit = 0.0,
            AltitudeEnd = 100.0,
        },
        Metrics = new ClimbMetrics
        {
            DistanceMeters = 1000,
            NetElevationMeters = 100,
            Slope = 10,
            MaxSlope = 10
        },
        StartPointMeters = 0
    };

    private ClimbCoordinates CreateCoordinates(
        double latitudeInit,
        double latitudeEnd,
        double longitudeInit,
        double longitudeEnd,
        double altitudeInit,
        double altitudeEnd)
    {
        return new ClimbCoordinates
        {
            LatitudeInit = latitudeInit,
            LatitudeEnd = latitudeEnd,
            LongitudeInit = longitudeInit,
            LongitudeEnd = longitudeEnd,
            AltitudeInit = altitudeInit,
            AltitudeEnd = altitudeEnd,
        };
    }

    private ClimbMetrics CreateMetrics(
        double distanceMeters,
        double elevationMeters,
        double slope,
        double maxSlope)
    {
        return new ClimbMetrics
        {
            DistanceMeters = distanceMeters,
            NetElevationMeters = elevationMeters,
            Slope = slope,
            MaxSlope = maxSlope
        };
    }

    private ClimbDetails CreateDetails(
        ClimbCoordinates coordinates,
        ClimbMetrics metrics)
    {
        return new ClimbDetails
        {
            Coordinates = coordinates,
            Metrics = metrics,
            StartPointMeters = 0
        };
    }

    public static IEnumerable<object[]> EdgeInCoordinates()
    {
        yield return new object[] { 0.0, 0.009, 0.0, 0.0, 0.0, 100.0 };     // Same climb
        yield return new object[] { 0.00009, 0.009, 0.0, 0.0, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.00909, 0.0, 0.0, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.00009, 0.0, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.0, 0.00009, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.0, 0.0, 10.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.0, 0.0, 0.0, 110.0 };
        yield return new object[] { -0.00009, 0.009, 0.0, 0.0, 0.0, 100.0 };     // Try negative in latitude
    }

    [Theory]
    [MemberData(nameof(EdgeInCoordinates))]
    public void Match_EdgeCoordinates_ReturnTrue(
        double latitudeInit,
        double latitudeEnd,
        double longitudeInit,
        double longitudeEnd,
        double altitudeInit,
        double altitudeEnd)
    {
        // Arrange
        var test = CreateDetails(
            CreateCoordinates(
                latitudeInit,
                latitudeEnd,
                longitudeInit,
                longitudeEnd,
                altitudeInit,
                altitudeEnd),
            _originalDetails.Metrics);

        // Act & Assert
        Assert.True(_originalDetails.Match(test));
    }

    public static IEnumerable<object[]> EdgeOutCoordinates()
    {
        yield return new object[] { 0.00011, 0.009, 0.0, 0.0, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.00101, 0.0, 0.0, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.00011, 0.0, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.0, 0.00011, 0.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.0, 0.0, 11.0, 100.0 };
        yield return new object[] { 0.0, 0.009, 0.0, 0.0, 0.0, 111.0 };
    }

    [Theory]
    [MemberData(nameof(EdgeOutCoordinates))]
    public void Match_EdgeCoordinates_ReturnFalse(
        double latitudeInit,
        double latitudeEnd,
        double longitudeInit,
        double longitudeEnd,
        double altitudeInit,
        double altitudeEnd)
    {
        // Arrange
        var test = CreateDetails(
            CreateCoordinates(
                latitudeInit,
                latitudeEnd,
                longitudeInit,
                longitudeEnd,
                altitudeInit,
                altitudeEnd),
            _originalDetails.Metrics);

        // Act & Assert
        Assert.False(_originalDetails.Match(test));
    }

    public static IEnumerable<object[]> EdgeInMetrics()
    {
        yield return new object[] { 
            _originalDetails.Metrics.DistanceMeters, 
            _originalDetails.Metrics.NetElevationMeters};     // Same climb
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters + 10,
            _originalDetails.Metrics.NetElevationMeters};     
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters - 10,
            _originalDetails.Metrics.NetElevationMeters};
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters,
            _originalDetails.Metrics.NetElevationMeters + 10};     
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters,
            _originalDetails.Metrics.NetElevationMeters - 10};    
    }

    [Theory]
    [MemberData(nameof(EdgeInMetrics))]
    public void Match_EdgeMetrics_ReturnTrue(
        double distance,
        double elevation)
    {
        // Arrange
        var test = CreateDetails(
            _originalDetails.Coordinates,
            CreateMetrics(
                distance,
                elevation,
                _originalDetails.Metrics.Slope,
                _originalDetails.Metrics.MaxSlope));

        // Act & Assert
        Assert.True(_originalDetails.Match(test));
    }

    public static IEnumerable<object[]> EdgeOutMetrics()
    {
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters + 11,
            _originalDetails.Metrics.NetElevationMeters};
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters - 11,
            _originalDetails.Metrics.NetElevationMeters};
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters,
            _originalDetails.Metrics.NetElevationMeters + 11};
        yield return new object[] {
            _originalDetails.Metrics.DistanceMeters,
            _originalDetails.Metrics.NetElevationMeters - 11 };
    }

    [Theory]
    [MemberData(nameof(EdgeOutMetrics))]
    public void Match_EdgeMetrics_ReturnFalse(
        double distance,
        double elevation)
    {
        // Arrange
        var test = CreateDetails(
            _originalDetails.Coordinates,
            CreateMetrics(
                distance,
                elevation,
                _originalDetails.Metrics.Slope,
                _originalDetails.Metrics.MaxSlope));

        // Act & Assert
        Assert.False(_originalDetails.Match(test));
    }
}
