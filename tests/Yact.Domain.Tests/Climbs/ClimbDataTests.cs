using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Tests.Climbs;

public class ClimbDataTests
{
    private ClimbData _originalClimb = new ClimbData
    {
        LatitudeInit = 0.0,
        LatitudeEnd = 0.009,
        LongitudeInit = 0.0,
        LongitudeEnd = 0.0,
        AltitudeInit = 0.0,
        AltitudeEnd = 100.0,
        Metrics = new ClimbMetrics
        {
            DistanceMeters = 1000,
            Elevation = 100,
            Slope = 10,
            MaxSlope = 10
        }
    };

    private ClimbData _copyClimb = new ClimbData
    {
        LatitudeInit = 0.0,
        LatitudeEnd = 0.009,
        LongitudeInit = 0.0,
        LongitudeEnd = 0.0,
        AltitudeInit = 0.0,
        AltitudeEnd = 100.0,
        Metrics = new ClimbMetrics
        {
            DistanceMeters = 1000,
            Elevation = 100,
            Slope = 10,
            MaxSlope = 10
        }
    };

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
    public void Match_EdgeData_ReturnTrue(
        double latitudeInit,
        double latitudeEnd,
        double longitudeInit,
        double longitudeEnd,
        double altitudeInit,
        double altitudeEnd)
    {
        // Arrange
        _copyClimb.LatitudeInit = latitudeInit;
        _copyClimb.LatitudeEnd = latitudeEnd;
        _copyClimb.LongitudeInit = longitudeInit;
        _copyClimb.LongitudeEnd = longitudeEnd;
        _copyClimb.AltitudeInit = altitudeInit;
        _copyClimb.AltitudeEnd = altitudeEnd;

        // Act & Assert
        Assert.True(_originalClimb.Match(_copyClimb));
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
    public void Match_EdgeData_ReturnFalse(
        double latitudeInit,
        double latitudeEnd,
        double longitudeInit,
        double longitudeEnd,
        double altitudeInit,
        double altitudeEnd)
    {
        // Arrange
        _copyClimb.LatitudeInit = latitudeInit;
        _copyClimb.LatitudeEnd = latitudeEnd;
        _copyClimb.LongitudeInit = longitudeInit;
        _copyClimb.LongitudeEnd = longitudeEnd;
        _copyClimb.AltitudeInit = altitudeInit;
        _copyClimb.AltitudeEnd = altitudeEnd;

        // Act & Assert
        Assert.False(_originalClimb.Match(_copyClimb));
    }
}
