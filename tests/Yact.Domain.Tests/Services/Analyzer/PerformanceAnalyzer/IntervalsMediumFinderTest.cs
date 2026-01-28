using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Diagnostics;
using Xunit.Abstractions;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

public sealed class MediumFinderUnitTests
{
    private readonly ITestOutputHelper output;
    private const int IntervalMinTime = 4 * 60;    // 4 mins

    private IntervalsMediumFinder finder;
    private PowerZones powerZones;

    public MediumFinderUnitTests(ITestOutputHelper output)
    {
        this.output = output;
        powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
    }

    private void CreateFinder(List<TestRecord> testRecords)
    {
        var records = FitnessDataService.SetData(testRecords);
        finder = new IntervalsMediumFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { output.WriteLine(e); };
    }

    #region No Interval

    [Fact]
    public void Search_NoRecords_NoIntervalReturned()
    {
        // Arrange
        var finder = new IntervalsMediumFinder(powerZones, new List<RecordData>());

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Search_MinimumTimeNotReached_NoIntervalReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = IntervalMinTime - 1, Power = powerZones.Values[3].HighLimit, HearRate = 120, Cadence = 85},
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Search_MinimumPowerNotReached_NoIntervalReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 5 * 60, Power = powerZones.Values[3].LowLimit, HearRate = 120, Cadence = 85},
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Search_MaximumPowerReached_NoIntervalReturned()
    {
        // Asserts that interval won't start until power goes bellow the max allowed
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 5 * 60, Power = powerZones.Values[6].HighLimit + 1, HearRate = 120, Cadence = 85}, 
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // No Interval

    #region Simple Intervals

    [Fact]
    public void Search_PowerAndDurationReached_OneIntervalReturned()
    {
        // Arrange
        var power = (powerZones.Values[3].HighLimit + powerZones.Values[3].LowLimit) / 2;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = IntervalMinTime, Power = power, HearRate = 120, Cadence = 85},
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(IntervalMinTime, intervals.First().DurationSeconds);
        Assert.Equal(power, intervals.First().AveragePower);
        Assert.Equal(120, intervals.First().AverageHeartRate);
        Assert.Equal(85, intervals.First().AverageCadence);
    }

    [Fact]
    public void Search_IntervalBetweenBreaks_OneIntervalReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 30, Power = powerZones.Values[3].LowLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[3].HighLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 30, Power = powerZones.Values[3].LowLimit, HearRate = 120, Cadence = 85},
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(30), intervals.First().StartTime);
        Assert.Equal(IntervalMinTime, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[3].HighLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);
    }

    [Fact]
    public void Search_TwoIntervalsDividedByBreak_TwoIntervalsReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[3].HighLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 6, Power = powerZones.Values[3].HighLimit / 2 - 1, HearRate = 120, Cadence = 85},   // 6 secs above 50% deviation has to break an interval
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[3].HighLimit, HearRate = 125, Cadence = 90},
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Equal(2, intervals.Count());
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().StartTime);
        Assert.Equal(IntervalMinTime, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[3].HighLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);

        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(IntervalMinTime + 6), intervals.Last().StartTime);
        Assert.Equal(IntervalMinTime, intervals.Last().DurationSeconds);
        Assert.Equal(powerZones.Values[3].HighLimit, intervals.Last().AveragePower);
        Assert.Equal(125, intervals.Last().AverageHeartRate);
        Assert.Equal(90, intervals.Last().AverageCadence);
    }

    #endregion // Simple Intervals

    #region Irregular

    [Fact]
    public void Search_SmallIrregularPower_OneIntervalReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>();
        for (int i = 0; i < 30; i++)
        {
            testRecords.Add(new TestRecord { Time = 4, Power = powerZones.Values[3].LowLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 6, Power = powerZones.Values[4].LowLimit, HearRate = 125, Cadence = 90 });
        }
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var expectedPower = (powerZones.Values[3].LowLimit * 4 + powerZones.Values[4].LowLimit * 6) / 10;
        var expectedHr = (120 * 4 + 125 * 6) / 10;
        var expectedCadence = (90 * 4 + 85 * 6) / 10;
        Assert.Equal(296, intervals.First().DurationSeconds);   
        Assert.Equal(expectedPower, intervals.First().AveragePower, 1f);
        Assert.Equal(expectedHr, intervals.First().AverageHeartRate, 1f);
        Assert.Equal(expectedCadence, intervals.First().AverageCadence, 1f);
    }

    [Fact]
    public void Search_BigDeviation_NoIntervalReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>();
        for (int i = 0; i < 30; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[2].HighLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85 });
        }
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Search_SmoothStartThenBigDeviation_NoIntervalReturned()
    {
        // When the interval is too irregular, it should't be detected
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>()
        {
            new TestRecord { Time = 30, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85 }
        };
        for (int i = 0; i < 30; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[2].HighLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85 });
        }
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // Irregular

    #region Changes

    private (PowerZones, int, int, int) ChangeSetUp(int normalPower, float powerChangePercentage, int changeTime)
    {
        var averagePower = powerChangePercentage < 1 ? powerZones.Values[3].HighLimit : powerChangePercentage * 1.1f;
        var changePower = normalPower * powerChangePercentage;
        
        var normalPowerDuration = (int)((changeTime * (averagePower - changePower)) / (2 * (normalPower - averagePower)) + 1);  // +1 just in case it rounds down
        if (normalPowerDuration < 5 * 60)   // just in case it doesn't reach the minimum time
        { 
            normalPowerDuration = 5 * 60;
            averagePower = (int)(normalPower * normalPowerDuration * 2 + changePower * changeTime) / (changeTime + normalPowerDuration * 2);
        }

        return (powerZones, normalPowerDuration, (int)changePower, (int)averagePower);
    }

    public static IEnumerable<object[]> SmallChange()
    {
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.05f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.10f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.15f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.20f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.26f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.30f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.35f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.40f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.45f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.51f, 10 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.55f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.60f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.65f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.70f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.76f, 30 };
        // 2 mins as an example, but as long as it doesn't go bellow the min power,
        // there's no time limit
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 5f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.95f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.90f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.85f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.80f, 1 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.74f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.70f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.65f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.60f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.55f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.49f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.45f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.35f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.30f, 15 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.25f, 30 };
    }
    [Theory]
    [MemberData(nameof(SmallChange))]
    public void Search_IntervalWithMiddleSmallChange_OneIntervalReturned(int normalPower, float changePowerPercentage, int changeTime)
    {
        // With a small Change in the middle, the interval is detected as one and will contain the Change.
        // Arrange
        var setup = ChangeSetUp(normalPower, changePowerPercentage, changeTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var ChangePower = setup.Item3;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = changeTime, Power = ChangePower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var averagePower = setup.Item4;
        Assert.Equal(normalPowerDuration * 2 + changeTime, intervals.First().DurationSeconds);
        Assert.Equal(averagePower, intervals.First().AveragePower, 1f);
        var expectedHr = (125 * 2 * normalPowerDuration + 120 * changeTime) / (2 * normalPowerDuration + changeTime);
        var expectedCadence = (90 * 2 * normalPowerDuration + 85 * changeTime) / (2 * normalPowerDuration + changeTime);
        Assert.Equal(expectedHr, intervals.First().AverageHeartRate, 1f);
        Assert.Equal(expectedCadence, intervals.First().AverageCadence, 1f);
    }

    [Theory]
    [MemberData(nameof(SmallChange))]
    public void Search_IntervalWithEndSmallChange_OneIntervalReturned(int normalPower, float powerChangePercentage, int changeTime)
    {
        // With a small Change in the end, the interval won't contain the Change
        // Arrange
        var setup = ChangeSetUp(normalPower, powerChangePercentage, changeTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var ChangePower = setup.Item3;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration * 2, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = changeTime, Power = ChangePower, HearRate = 120, Cadence = 85 },
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var averagePower = setup.Item4;
        Assert.Equal(normalPowerDuration * 2, intervals.First().DurationSeconds);
        Assert.Equal(normalPower, intervals.First().AveragePower, 1f);
        Assert.Equal(125, intervals.First().AverageHeartRate, 1f);
        Assert.Equal(90, intervals.First().AverageCadence, 1f);
    }

    public static IEnumerable<object[]> BigChange()
    {
        // Both reference and minimum errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.0f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.05f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.10f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.15f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.20f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.26f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.30f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.35f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.40f, 6 };

        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 5.00f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.95f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.90f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.85f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.80f, 2 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.74f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.70f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.65f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.60f, 6 };

        // Reference errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.45f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.65f, 16 };
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.70f, 16 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.74f, 16 }; // removed because the second interval was detected with the drop

        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.55f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.49f, 16 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.45f, 16 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.40f, 16 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.35f, 16 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.30f, 16 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 1.26f, 16 };

        // Minimum errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.51f, 31 };  
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.55f, 31 }; 
        yield return new object[] { IntervalsTestConstants.PowerZones[5].HighLimit, 0.60f, 31 };   
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.45f, 11 };   
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.51f, 11 };   
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.55f, 11 };   
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.60f, 11 };   
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.65f, 11 };   
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.70f, 31 };   
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.75f, 31 };   
    }
    [Theory]
    [MemberData(nameof(BigChange))]
    public void Search_IntervalWithMiddleBigChange_TwoIntervalReturned(int normalPower, float powerChangePercentage, int changeTime)
    {
        // With a big Change in the middle, two different intervals will be detected and none will contain the Change
        // Arrange
        var setup = ChangeSetUp(normalPower, powerChangePercentage, changeTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var changePower = setup.Item3;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = changeTime, Power = changePower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Equal(2, intervals.Count());
        Assert.Equal(normalPowerDuration, intervals.First().DurationSeconds);
        Assert.Equal(normalPower, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);

        Assert.Equal(normalPowerDuration, intervals.Last().DurationSeconds);
        Assert.Equal(normalPower, intervals.Last().AveragePower);
        Assert.Equal(125, intervals.Last().AverageHeartRate);
        Assert.Equal(90, intervals.Last().AverageCadence);
    }

    #endregion // Changes
}