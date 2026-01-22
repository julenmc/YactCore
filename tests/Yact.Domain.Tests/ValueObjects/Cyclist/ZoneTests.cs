using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.ValueObjects.Cyclist;

public sealed class ZoneTests
{
    [Fact]
    public void Create_LowBellowZero_ThrowsException()
    {
        // Arrange
        int lowLimit = -1;
        int highLimit = 100;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Zone.Create(lowLimit, highLimit));
    }

    [Fact]
    public void Create_LowHigherThanHigh_ThrowsException()
    {
        // Arrange
        int lowLimit = 101;
        int highLimit = 100;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Zone.Create(lowLimit, highLimit));
    }

    [Fact]
    public void Create_LowEqualsHigh_ThrowsException()
    {
        // Arrange
        int lowLimit = 100;
        int highLimit = 100;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Zone.Create(lowLimit, highLimit));
    }

    [Fact]
    public void Create_LowLowerThanHigh_ReturnsZone()
    {
        // Arrange
        int lowLimit = 0;
        int highLimit = 100;

        // Act
        var zone = Zone.Create(lowLimit, highLimit);

        // Assert
        Assert.Equal(lowLimit, zone.LowLimit);
        Assert.Equal(highLimit, zone.HighLimit);
    }
}
