using Xunit.Abstractions;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

public sealed class HighDurationFinderUnitTests
{
    private readonly ITestOutputHelper _output;
    private const int IntervalMinTime = 20 * 60;    // 20 mins

    public HighDurationFinderUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region No Interval

    [Fact]
    public void Search_NoRecords_NoIntervalReturned()
    {
        // Arrange
        var finder = new IntervalsHighDurationFinder(PowerZones.Create(IntervalsTestConstants.PowerZones), new List<RecordData>());

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Search_MinimumTimeNotReached_NoIntervalReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = IntervalMinTime - 1, Power = powerZones.Values[2].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Search_MinimumPowerNotReached_NoIntervalReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 25 * 60, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Search_MaximumPowerReached_NoIntervalReturned()
    {
        // Asserts that this finder won't find high power intervals
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 25 * 60, Power = powerZones.Values[5].LowLimit, HearRate = 120, Cadence = 85}, 
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

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
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var power = (powerZones.Values[2].HighLimit + powerZones.Values[2].LowLimit) / 2;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = IntervalMinTime, Power = power, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

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
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 30, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[2].HighLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 30, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(30), intervals.First().StartTime);
        Assert.Equal(IntervalMinTime, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[2].HighLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);
    }

    [Fact]
    public void Search_TwoIntervalsDividedByBreak_TwoIntervalsReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[2].HighLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 31, Power = powerZones.Values[2].HighLimit / 2, HearRate = 120, Cadence = 85},   // 31 secs at 50% has to break an interval
            new TestRecord{ Time = IntervalMinTime, Power = powerZones.Values[2].HighLimit, HearRate = 125, Cadence = 90},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

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
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>();
        for (int i = 0; i < 150; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[3].LowLimit, HearRate = 125, Cadence = 90 });
        }
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var expectedPower = (powerZones.Values[2].LowLimit * 45 + powerZones.Values[3].LowLimit * 50) / 95;
        var expectedHr = (120 * 45 + 125 * 50) / 95;
        var expectedCadence = (90 * 50 + 85 * 45) / 95;
        Assert.Equal(1495, intervals.First().DurationSeconds);    // First 5 seconds don't count
        Assert.Equal(expectedPower, intervals.First().AveragePower);
        Assert.Equal(expectedHr, intervals.First().AverageHeartRate);
        Assert.Equal(expectedCadence, intervals.First().AverageCadence);
    }

    [Fact]
    public void Search_BigDeviation_NoIntervalReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>();
        for (int i = 0; i < 150; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[1].HighLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[3].HighLimit, HearRate = 120, Cadence = 85 });
        }
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // Irregular

    #region Drops

    private (PowerZones, int, int, int) DropSetUp(int normalPower, float powerDropPercentage, int dropTime)
    {
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var averagePower = powerZones.Values[2].HighLimit;
        var dropPower = normalPower * powerDropPercentage;
        
        var normalPowerDuration = (int)(dropTime * (averagePower - dropPower)) / (2 * (normalPower - averagePower)) + 1;  // +1 just in case it round down
        if (normalPowerDuration < 20 * 60)   // just in case it doesn't reach the minimum time
        { 
            normalPowerDuration = 20 * 60;
            averagePower = (int)(normalPower * normalPowerDuration * 2 + dropPower * dropTime) / (dropTime + normalPowerDuration * 2);
        }

        return (powerZones, normalPowerDuration, (int)dropPower, averagePower);
    }

    public static IEnumerable<object[]> SmallDrop()
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
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.51f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.55f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.65f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.70f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.75f, 300 };   
                                                    // 5 mins as an example, but as long as it doesn't go bellow the min power,
                                                    // there's no time limit
    }
    [Theory]
    [MemberData(nameof(SmallDrop))]
    public void Search_IntervalWithMiddleSmallDrop_OneIntervalReturned(int normalPower, float dropPowerPercentage, int dropTime)
    {
        // With a small drop in the middle, the interval is detected as one and will contain the drop.
        // Arrange
        var setup = DropSetUp(normalPower, dropPowerPercentage, dropTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var dropPower = setup.Item3;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var averagePower = setup.Item4;
        Assert.Equal(normalPowerDuration * 2 + dropTime, intervals.First().DurationSeconds);
        Assert.Equal(averagePower, intervals.First().AveragePower, 1f);
        var expectedHr = (125 * 2 * normalPowerDuration + 120 * dropTime) / (2 * normalPowerDuration + dropTime);
        var expectedCadence = (90 * 2 * normalPowerDuration + 85 * dropTime) / (2 * normalPowerDuration + dropTime);
        Assert.Equal(expectedHr, intervals.First().AverageHeartRate, 1f);
        Assert.Equal(expectedCadence, intervals.First().AverageCadence, 1f);
    }

    [Theory]
    [MemberData(nameof(SmallDrop))]
    public void Search_IntervalWithEndSmallDrop_OneIntervalReturned(int normalPower, float powerDropPercentage, int dropTime)
    {
        // With a small drop in the end, the interval won't contain the drop
        // Arrange
        var setup = DropSetUp(normalPower, powerDropPercentage, dropTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var dropPower = setup.Item3;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration * 2, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85 },
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

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

    private (PowerZones, int, int, int, int) BigDropSetUp(int normalPower, float powerDropPercentage, int dropTime)
    {
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var averagePower = powerZones.Values[5].LowLimit;
        var dropPower = averagePower * powerDropPercentage;
        
        var normalPowerDuration = (int)(dropTime * (averagePower - dropPower)) / (2 * (normalPower - averagePower)) + 1;  // +1 just in case it round down
        if (normalPowerDuration < 30)   // just in case it doesn't reach the minimum time
            normalPowerDuration = 30;

        return (powerZones, normalPowerDuration, normalPower, (int)dropPower, averagePower);
    }
    public static IEnumerable<object[]> BigDrop()
    {
        // Both reference and minimum errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.0f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.05f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.10f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.15f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.20f, 5 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.26f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.30f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.35f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.40f, 30 };

        // Reference errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.45f, 30 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.51f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.55f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.60f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.65f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.70f, 120 };
        yield return new object[] { IntervalsTestConstants.PowerZones[3].HighLimit, 0.74f, 120 };

        // Minimum errors jump
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.45f, 60 };   // 85W is 52% of minimum
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.51f, 60 };   // 58%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.55f, 60 };   // 63%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.60f, 60 };   // 69%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.65f, 60 };   // 75%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.70f, 90 };   // 81%
        yield return new object[] { IntervalsTestConstants.PowerZones[2].HighLimit, 0.75f, 90 };   // 86%
    }
    [Theory]
    [MemberData(nameof(BigDrop))]
    public void Search_IntervalWithMiddleBigDrop_TwoIntervalReturned(int normalPower, float powerDropPercentage, int dropTime)
    {
        // With a big drop in the middle, two different intervals will be detected and none will contain the drop
        // Arrange
        var setup = DropSetUp(normalPower, powerDropPercentage, dropTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var dropPower = setup.Item3;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

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

    [Theory]
    [MemberData(nameof(BigDrop))]
    public void Search_IntervalWithMiddleBigDrop_NoIntervalReturned(float powerDropPercentage, int dropTime)
    {
        // Interval gets divided and it's not enought for detection
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);

        var normalPower = powerZones.Values[5].HighLimit;
        var dropPower = powerZones.Values[5].LowLimit * powerDropPercentage;

        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = IntervalMinTime - 1, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = (int)dropPower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = IntervalMinTime - 1, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighDurationFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // Drops
}