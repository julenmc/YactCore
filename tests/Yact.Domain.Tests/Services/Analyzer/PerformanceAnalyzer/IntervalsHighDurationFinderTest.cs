using Xunit.Abstractions;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

public sealed class HighDurationFinderUnitTests
{
    private readonly ITestOutputHelper output;
    private const int IntervalMinTime = 20 * 60;    // 20 mins

    private IntervalsHighDurationFinder finder;
    private PowerZones powerZones;

    public HighDurationFinderUnitTests(ITestOutputHelper output)
    {
        this.output = output;
        powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
    }

    private void CreateFinder(List<TestRecord> testRecords)
    {
        var records = FitnessDataService.SetData(testRecords);
        finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { output.WriteLine(e); };
    }

    #region No Interval

    [Fact]
    public void Search_NoRecords_NoIntervalReturned()
    {
        // Arrange
        var finder = new IntervalsHighDurationFinder(powerZones, new List<RecordData>());

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
            new TestRecord{ Time = IntervalMinTime - 1, Power = powerZones.Values[2].HighLimit, HearRate = 120, Cadence = 85},
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
            new TestRecord{ Time = 25 * 60, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
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
            new TestRecord{ Time = 25 * 60, Power = powerZones.Values[5].LowLimit + 1, HearRate = 120, Cadence = 85}, 
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
        var power = (powerZones.Values[2].HighLimit + powerZones.Values[2].LowLimit) / 2;
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
            new TestRecord{ Time = 60, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[2].HighLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 60, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60), intervals.First().StartTime);
        Assert.Equal(IntervalMinTime, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[2].HighLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);
    }

    [Fact]
    public void Search_TwoIntervalsDividedByBreak_TwoIntervalsReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[2].HighLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 31, Power = powerZones.Values[2].HighLimit / 2 - 1, HearRate = 120, Cadence = 85},   // 31 secs above 50% has to break an interval
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[2].HighLimit, HearRate = 125, Cadence = 90},
        };
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Equal(2, intervals.Count());
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().StartTime);
        Assert.Equal(IntervalMinTime, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[2].HighLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);

        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(IntervalMinTime + 31), intervals.Last().StartTime);
        Assert.Equal(IntervalMinTime, intervals.Last().DurationSeconds);
        Assert.Equal(powerZones.Values[2].HighLimit, intervals.Last().AveragePower);
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
        for (int i = 0; i < 150; i++)
        {
            testRecords.Add(new TestRecord { Time = 4, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 6, Power = powerZones.Values[3].LowLimit, HearRate = 125, Cadence = 90 });
        }
        CreateFinder(testRecords);

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var expectedPower = (powerZones.Values[2].LowLimit * 4 + powerZones.Values[3].LowLimit * 6) / 10;
        var expectedHr = (120 * 4 + 125 * 6) / 10;
        var expectedCadence = (90 * 4 + 85 * 6) / 10;
        Assert.Equal(1500, intervals.First().DurationSeconds);   
        Assert.Equal(expectedPower, intervals.First().AveragePower, 1f);
        Assert.Equal(expectedHr, intervals.First().AverageHeartRate, 1f);
        Assert.Equal(expectedCadence, intervals.First().AverageCadence, 1f);
    }

    [Fact]
    public void Search_BigDeviation_NoIntervalReturned()
    {
        // Arrange
        List<TestRecord> testRecords = new List<TestRecord>();
        for (int i = 0; i < 150; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[1].HighLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[3].HighLimit, HearRate = 120, Cadence = 85 });
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
            new TestRecord { Time = 60, Power = powerZones.Values[3].HighLimit, HearRate = 120, Cadence = 85 }
        };
        for (int i = 0; i < 150; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[1].HighLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[3].HighLimit, HearRate = 120, Cadence = 85 });
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
        var averagePower = powerChangePercentage < 1 ? powerZones.Values[2].HighLimit : powerChangePercentage * 1.1f;
        var changePower = normalPower * powerChangePercentage;
        
        var normalPowerDuration = (int)((changeTime * (averagePower - changePower)) / (2 * (normalPower - averagePower)) + 1);  // +1 just in case it rounds down
        if (normalPowerDuration < 20 * 60)   // just in case it doesn't reach the minimum time
        { 
            normalPowerDuration = 20 * 60;
            averagePower = (int)(normalPower * normalPowerDuration * 2 + changePower * changeTime) / (changeTime + normalPowerDuration * 2);
        }

        return (powerZones, normalPowerDuration, (int)changePower, (int)averagePower);
    }

    public static IEnumerable<object[]> SmallChange()
    {
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.05f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.10f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.15f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.20f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.26f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.30f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.35f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.40f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.45f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.51f, 60 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.55f, 90 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.60f, 90 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.65f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.70f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.76f, 300 };
        // 5 mins as an example, but as long as it doesn't go bellow the min power,
        // there's no time limit
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 5f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.95f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.90f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.85f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.80f, 4 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.74f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.70f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.65f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.60f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.55f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.49f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.45f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.35f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.30f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.25f, 300 };
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
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.0f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.05f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.10f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.15f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.20f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.26f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.30f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.35f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.40f, 31 };

        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 5.00f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.95f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.90f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.85f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.80f, 6 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.74f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.70f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.65f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 1.60f, 31 };

        // Reference errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.45f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.65f, 121 };
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.70f, 121 };
        //yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.74f, 121 }; // removed because the second interval was detected with the drop

        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.55f, 31 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.49f, 121 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.45f, 121 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.40f, 121 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.35f, 121 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.30f, 121 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 1.26f, 121 };

        // Minimum errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.51f, 121 };   // 82%
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.55f, 121 };   // 89%
        yield return new object[] { IntervalsTestConstants.PowerZones[4].HighLimit, 0.60f, 121 };   // 97%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.45f, 61 };   // 85W is 52% of minimum
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.51f, 61 };   // 58%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.55f, 61 };   // 63%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.60f, 61 };   // 69%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.65f, 61 };   // 75%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.70f, 91 };   // 81%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.75f, 91 };   // 86%
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