using Xunit.Abstractions;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

public sealed class HighPowerFinderUnitTests
{
    private readonly ITestOutputHelper _output;

    public HighPowerFinderUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region No Interval

    [Fact]
    public void Search_NoRecords_NoIntervalReturned()
    {
        // Arrange
        var finder = new IntervalsHighPowerFinder(PowerZones.Create(IntervalsTestConstants.PowerZones), new List<RecordData>());

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
            new TestRecord{ Time = 29, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
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
            new TestRecord{ Time = 40, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
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
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 30, Power = powerZones.Values[5].LowLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(30, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[5].LowLimit, intervals.First().AveragePower);
        Assert.Equal(120, intervals.First().AverageHeartRate);
        Assert.Equal(85, intervals.First().AverageCadence);
    }

    [Fact]
    public void Search_HighPowerBetweenBreaks_OneIntervalReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 30, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 30, Power = powerZones.Values[5].LowLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 30, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(30), intervals.First().StartTime);
        Assert.Equal(30, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[5].LowLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);
    }

    [Fact]
    public void Search_TwoHighPowerDividedByBreak_TwoIntervalsReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 30, Power = powerZones.Values[5].LowLimit, HearRate = 125, Cadence = 90},
            new TestRecord{ Time = 30, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 30, Power = powerZones.Values[5].LowLimit, HearRate = 125, Cadence = 90},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Equal(2, intervals.Count());
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().StartTime);
        Assert.Equal(30, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[5].LowLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);

        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60), intervals.Last().StartTime);
        Assert.Equal(30, intervals.Last().DurationSeconds);
        Assert.Equal(powerZones.Values[5].LowLimit, intervals.Last().AveragePower);
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
        for (int i = 0; i < 10; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[5].HighLimit, HearRate = 125, Cadence = 90 });
        }
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var expectedPower = (powerZones.Values[4].HighLimit * 45 + powerZones.Values[5].HighLimit * 50) / 95;
        var expectedHr = (120 * 45 + 125 * 50) / 95;
        var expectedCadence = (90 * 50 + 85 * 45) / 95;
        Assert.Equal(95, intervals.First().DurationSeconds);    // First 5 seconds don't count
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
        for (int i = 0; i < 10; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[4].LowLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85 });
        }
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

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
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>()
        {
            new TestRecord { Time = 10, Power = powerZones.Values[5].LowLimit, HearRate = 120, Cadence = 85 }
        };
        for (int i = 0; i < 10; i++)
        {
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[4].LowLimit, HearRate = 120, Cadence = 85 });
            testRecords.Add(new TestRecord { Time = 5, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85 });
        }
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // Irregular

    #region Drops

    private (PowerZones, int, int, int, int) SmallDropSetUp(float powerDropPercentage, int dropTime)
    {
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var normalPower = powerZones.Values[5].HighLimit;
        var averagePower = powerZones.Values[5].LowLimit;
        var dropPower = (normalPower * powerDropPercentage) < powerZones.Values[4].HighLimit ?            // I need the drop power to be lower than the allowed
            (normalPower * powerDropPercentage) : powerZones.Values[4].HighLimit;
        
        var normalPowerDuration = (int)(dropTime * (averagePower - dropPower)) / (2 * (normalPower - averagePower)) + 1;  // +1 just in case it round down
        if (normalPowerDuration < 15)   // just in case it doesn't reach the minimum time
        { 
            normalPowerDuration = 15;
            averagePower = (int)(normalPower * normalPowerDuration * 2 + dropPower * dropTime) / (dropTime + normalPowerDuration * 2);
        }

        return (powerZones, normalPowerDuration, normalPower, (int)dropPower, averagePower);
    }

    public static IEnumerable<object[]> SmallDrop()
    {
        yield return new object[] { 0.51f, 5 };
        yield return new object[] { 0.60f, 5 };
        yield return new object[] { 0.65f, 5 };
        yield return new object[] { 0.70f, 5 };
        yield return new object[] { 0.75f, 5 };     
        yield return new object[] { 0.80f, 15 };     
        yield return new object[] { 0.85f, 15 };
        yield return new object[] { 0.90f, 30 };
        yield return new object[] { 0.95f, 30 };    
    }
    [Theory]
    [MemberData(nameof(SmallDrop))]
    public void Search_IntervalWithMiddleSmallDrop_OneIntervalReturned(float powerDropPercentage, int dropTime)
    {
        // With a small drop in the middle, the interval is detected as one and will contain the drop
        // Arrange
        var setup = SmallDropSetUp(powerDropPercentage, dropTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var normalPower = setup.Item3;
        var dropPower = setup.Item4;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var averagePower = setup.Item5;
        Assert.Equal(normalPowerDuration * 2 + dropTime, intervals.First().DurationSeconds);
        Assert.Equal(averagePower, intervals.First().AveragePower, 1f);
        var expectedHr = (125 * 2 * normalPowerDuration + 120 * dropTime) / (2 * normalPowerDuration + dropTime);
        var expectedCadence = (90 * 2 * normalPowerDuration + 85 * dropTime) / (2 * normalPowerDuration + dropTime);
        Assert.Equal(expectedHr, intervals.First().AverageHeartRate, 1f);
        Assert.Equal(expectedCadence, intervals.First().AverageCadence, 1f);
    }

    [Theory]
    [MemberData(nameof(SmallDrop))]
    public void Search_IntervalWithEndSmallDrop_OneIntervalReturned(float powerDropPercentage, int dropTime)
    {
        // With a small drop in the middle, the interval won't contain the drop
        // Arrange
        var setup = SmallDropSetUp(powerDropPercentage, dropTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var normalPower = setup.Item3;
        var dropPower = setup.Item4;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration * 2, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85 },
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Single(intervals);
        var averagePower = setup.Item5;
        Assert.Equal(normalPowerDuration * 2, intervals.First().DurationSeconds);
        Assert.Equal(normalPower, intervals.First().AveragePower, 1f);
        Assert.Equal(125, intervals.First().AverageHeartRate, 1f);
        Assert.Equal(90, intervals.First().AverageCadence, 1f);
    }

    private (PowerZones, int, int, int, int) BigDropSetUp(float powerDropPercentage, int dropTime)
    {
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var normalPower = powerZones.Values[5].HighLimit;
        var averagePower = powerZones.Values[5].LowLimit;
        var dropPower = averagePower * powerDropPercentage;
        
        var normalPowerDuration = (int)(dropTime * (averagePower - dropPower)) / (2 * (normalPower - averagePower)) + 1;  // +1 just in case it round down
        if (normalPowerDuration < 30)   // just in case it doesn't reach the minimum time
            normalPowerDuration = 30;

        return (powerZones, normalPowerDuration, normalPower, (int)dropPower, averagePower);
    }
    public static IEnumerable<object[]> BigDrop()
    {
        yield return new object[] { 0.49f, 5 };
        yield return new object[] { 0.60f, 10 };
        yield return new object[] { 0.65f, 10 };
        yield return new object[] { 0.70f, 10 };
        yield return new object[] { 0.75f, 10 };
        yield return new object[] { 0.80f, 20 };
        yield return new object[] { 0.85f, 20 };
        yield return new object[] { 0.90f, 35 };
        yield return new object[] { 0.95f, 35 };
    }
    [Theory]
    [MemberData(nameof(BigDrop))]
    public void Search_IntervalWithMiddleBigDrop_TwoIntervalReturned(float powerDropPercentage, int dropTime)
    {
        // With a big drop in the middle, two different intervals will be detected and none will contain the drop
        // Arrange
        var setup = BigDropSetUp(powerDropPercentage, dropTime);
        var powerZones = setup.Item1;
        var normalPowerDuration = setup.Item2;
        var normalPower = setup.Item3;
        var dropPower = setup.Item4;
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = normalPowerDuration, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
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
            new TestRecord { Time = 29, Power = normalPower, HearRate = 125, Cadence = 90 },
            new TestRecord { Time = dropTime, Power = (int)dropPower, HearRate = 120, Cadence = 85 },
            new TestRecord { Time = 29, Power = normalPower, HearRate = 125, Cadence = 90 }
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones, records);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.Search();

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // Drops
}