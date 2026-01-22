using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.ValueObjects.Cyclist;

public sealed class HeartRateZoneTests
{
    public static IEnumerable<object[]> ZoneMissing()
    {
        yield return new object[]   // 1 missing
        {

            new Dictionary<int, Zone>()
            {
                { 2, Zone.Create(130, 156) },
                { 3, Zone.Create(157, 174) },
                { 4, Zone.Create(175, 193) },
                { 5, Zone.Create(194, 250) },
            }
        };
        yield return new object[]   // 2 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 3, Zone.Create(157, 174) },
                { 4, Zone.Create(175, 193) },
                { 5, Zone.Create(194, 250) },
            }
        };
        yield return new object[]   // 3 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 2, Zone.Create(130, 156) },
                { 4, Zone.Create(175, 193) },
                { 5, Zone.Create(194, 250) },
            }
        };
        yield return new object[]   // 4 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 2, Zone.Create(130, 156) },
                { 3, Zone.Create(157, 174) },
                { 5, Zone.Create(194, 250) },
            }
        };
        yield return new object[]   // 5 missing
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 2, Zone.Create(130, 156) },
                { 3, Zone.Create(157, 174) },
                { 4, Zone.Create(175, 193) },
            }
        };
    }
    [Theory]
    [MemberData(nameof(ZoneMissing))]
    public void Create_ZoneMissing_ExceptionThrown(IDictionary<int, Zone> values)
    {
        Assert.Throws<ArgumentException>(() => HeartRateZones.Create(values));
    }

    public static IEnumerable<object[]> ZoneLowLimitLowerThanPrevious()
    {
        yield return new object[]   // 2 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 2, Zone.Create(128, 156) },
                { 3, Zone.Create(157, 174) },
                { 4, Zone.Create(175, 193) },
                { 5, Zone.Create(194, 250) },
            }
        };
        yield return new object[]   // 3 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 2, Zone.Create(130, 156) },
                { 3, Zone.Create(155, 174) },
                { 4, Zone.Create(175, 193) },
                { 5, Zone.Create(194, 250) },
            }
        };
        yield return new object[]   // 4 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 2, Zone.Create(130, 156) },
                { 3, Zone.Create(157, 174) },
                { 4, Zone.Create(173, 193) },
                { 5, Zone.Create(194, 250) },
            }
        };
        yield return new object[]   // 5 error
        {
            new Dictionary<int, Zone>()
            {
                { 1, Zone.Create(0, 129) },
                { 2, Zone.Create(130, 156) },
                { 3, Zone.Create(157, 174) },
                { 4, Zone.Create(175, 193) },
                { 5, Zone.Create(192, 250) },
            }
        };
    }
    [Theory]
    [MemberData(nameof(ZoneLowLimitLowerThanPrevious))]
    public void Create_ZoneLowLimitLowerThanPrevious_ExceptionThrown(IDictionary<int, Zone> values)
    {
        Assert.Throws<ArgumentException>(() => HeartRateZones.Create(values));
    }

    [Fact]
    public void Create_ZonesSatisfyConditions_ReturnsCorrectValues()
    {
        // Assert 
        var dictionary = new Dictionary<int, Zone>()
        {
            { 1, Zone.Create(0, 129) },
            { 2, Zone.Create(130, 156) },
            { 3, Zone.Create(157, 174) },
            { 4, Zone.Create(175, 193) },
            { 5, Zone.Create(194, 250) },
        };

        // Act 
        var hrZones = HeartRateZones.Create(dictionary);

        // Assert
        for (int i = 0; i < dictionary.Count; i++)
        {
            Assert.Equal(dictionary[i + 1], hrZones.Values[i + 1]);
        }
    }
}
