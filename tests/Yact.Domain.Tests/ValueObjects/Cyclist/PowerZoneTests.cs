using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.ValueObjects.Cyclist;

public sealed class PowerZoneTests
{
    public static IEnumerable<object[]> ZoneMissing()
    {
        yield return new object[]   // 1 missing
        { 
            
            new Dictionary<int, Zone>()
            {
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 2 missing
        { 
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 3 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 4 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 5 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 6 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 7 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
            }
        };
    }
    [Theory]
    [MemberData(nameof(ZoneMissing))]
    public void Create_ZoneMissing_ExceptionThrown(IDictionary<int, Zone> values)
    {
        Assert.Throws<ArgumentException>(() => PowerZones.Create(values));
    }

    public static IEnumerable<object[]> ZoneLowLimitLowerThanPrevious()
    {
        yield return new object[]   // 2 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(137, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 3 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(188, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 4 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(226, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 5 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(264, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 6 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(302, 379) },
                { 7, Zone.Create(380, 2000) },
            }
        };
        yield return new object[]   // 7 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 138) },
                { 2, Zone.Create(139, 189) },
                { 3, Zone.Create(190, 227) },
                { 4, Zone.Create(228, 265) },
                { 5, Zone.Create(266, 303) },
                { 6, Zone.Create(304, 379) },
                { 7, Zone.Create(378, 2000) },
            }
        };
    }
    [Theory]
    [MemberData(nameof(ZoneLowLimitLowerThanPrevious))]
    public void Create_ZoneLowLimitLowerThanPrevious_ExceptionThrown(IDictionary<int, Zone> values)
    {
        Assert.Throws<ArgumentException>(() => PowerZones.Create(values));
    }

    [Fact]
    public void Create_ZonesSatisfyConditions_ReturnsCorrectValues()
    {
        // Assert 
        var dictionary = new Dictionary<int, Zone>()
        {
            { 1, Zone.Create(0, 138) },
            { 2, Zone.Create(139, 189) },
            { 3, Zone.Create(190, 227) },
            { 4, Zone.Create(228, 265) },
            { 5, Zone.Create(266, 303) },
            { 6, Zone.Create(304, 379) },
            { 7, Zone.Create(380, 2000) },
        };

        // Act 
        var powerZones = PowerZones.Create(dictionary);

        // Assert
        for (int i = 0; i < dictionary.Count; i++)
        {
            Assert.Equal(dictionary[i + 1], powerZones.Values[i + 1]);
        }
    }
}
