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

    #region SearchHighPower

    #region No Interval

    [Fact]
    public void SearchHighPower_NoRecords_NoIntervalReturned()
    {
        // Arrange
        var finder = new IntervalsHighPowerFinder(PowerZones.Create(IntervalsTestConstants.PowerZones));

        // Act
        var intervals = finder.SearchHighPower(new List<RecordData>());

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void SearchHighPower_MinimumTimeNotReached_NoIntervalReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 29, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void SearchHighPower_MinimumPowerNotReached_NoIntervalReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 40, Power = powerZones.Values[4].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // No Interval

    #region Simple Intervals

    [Fact]
    public void SearchHighPower_PowerAndDurationReached_OneIntervalReturned()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        List<TestRecord> testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 30, Power = powerZones.Values[5].LowLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

        // Assert
        Assert.Single(intervals);
        Assert.Equal(30, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[5].LowLimit, intervals.First().AveragePower);
        Assert.Equal(120, intervals.First().AverageHeartRate);
        Assert.Equal(85, intervals.First().AverageCadence);
    }

    [Fact]
    public void SearchHighPower_HighPowerBetweenBreaks_OneIntervalReturned()
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(30), intervals.First().StartTime);
        Assert.Equal(30, intervals.First().DurationSeconds);
        Assert.Equal(powerZones.Values[5].LowLimit, intervals.First().AveragePower);
        Assert.Equal(125, intervals.First().AverageHeartRate);
        Assert.Equal(90, intervals.First().AverageCadence);
    }

    [Fact]
    public void SearchHighPower_TwoHighPowerDividedByBreak_TwoIntervalsReturned()
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

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
    public void SearchHighPower_SmallIrregularPower_OneIntervalReturned()
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

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
    public void SearchHighPower_BigDeviation_NoIntervalReturned()
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

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
    public void SearchHighPower_IntervalWithMiddleSmallDrop_OneIntervalReturned(float powerDropPercentage, int dropTime)
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

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
    public void SearchHighPower_IntervalWithEndSmallDrop_OneIntervalReturned(float powerDropPercentage, int dropTime)
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

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
    public void SearchHighPower_IntervalWithMiddleBigDrop_TwoIntervalReturned(float powerDropPercentage, int dropTime)
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

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
    public void SearchHighPower_IntervalWithMiddleBigDrop_NoIntervalReturned(float powerDropPercentage, int dropTime)
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
        var finder = new IntervalsHighPowerFinder(powerZones);
        finder.LogEventHandler += (s, e) => { _output.WriteLine(e); };

        // Act
        var intervals = finder.SearchHighPower(records);

        // Assert
        Assert.Empty(intervals);
    }

    #endregion // Drops

    // Session stopped pending

    #endregion // SearchHighPower

    //#region Single Simple Interval

    //public static IEnumerable<object[]> SingleIntervalWithCustomConfig()
    //{
    //    yield return new object[] { ShortThresholds, ShortIntervalValues, IntervalSearchGroups.Short };
    //    yield return new object[] { MediumThresholds, MediumIntervalValues, IntervalSearchGroups.Medium };
    //    yield return new object[] { LongThresholds, LongIntervalValues, IntervalSearchGroups.Long };
    //}
    ///// <summary>
    ///// Verifies that an interval is found with it's corresponding configuration.
    ///// </summary>
    ///// <remarks>
    ///// For example: When searching with short configuration in a session with one short interval,
    ///// the interval will be found.
    ///// </remarks>
    //[Theory]
    //[MemberData(nameof(SingleIntervalWithCustomConfig))]
    //public void Search_SingleIntervalWithCustomConfiguration_OneReturned(Thresholds thresholds, IntervalValues values, IntervalSearchGroups searchGroup)
    //{
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    var finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup,
    //        thresholds: thresholds);

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime), intervals[0].StartTime);
    //    Assert.Equal(values.DefaultTime, intervals[0].DurationSeconds);
    //    Assert.Equal(values.DefaultPower, intervals[0].AveragePower);
    //}

    //public static IEnumerable<object[]> SingleIntervalWithDefaultConfig()
    //{
    //    yield return new object[] { ShortIntervalValues, IntervalSearchGroups.Short };
    //    yield return new object[] { MediumIntervalValues, IntervalSearchGroups.Medium };
    //    yield return new object[] { LongIntervalValues, IntervalSearchGroups.Long };
    //}
    ///// <summary>
    ///// Verifies that an interval is found with it's corresponding default configuration.
    ///// </summary>
    ///// <remarks>
    ///// For example: When searching with short default configuration in a session with one short interval,
    ///// the interval will be found.
    ///// </remarks>
    //[Theory]
    //[MemberData(nameof(SingleIntervalWithDefaultConfig))]
    //public void Search_SingleIntervalWithDefaultConfiguration_OneReturned(IntervalValues values, IntervalSearchGroups searchGroup)
    //{
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    var finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup);

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime), intervals[0].StartTime);
    //    Assert.Equal(values.DefaultTime, intervals[0].DurationSeconds);
    //    Assert.Equal(values.DefaultPower, intervals[0].AveragePower);
    //}

    //public static IEnumerable<object[]> LowerPowerThanExpected()
    //{
    //    yield return new object[] { MediumIntervalValues, ShortThresholds, IntervalSearchGroups.Short };
    //    yield return new object[] { LongIntervalValues, MediumThresholds, IntervalSearchGroups.Medium };
    //}
    ///// <summary>
    ///// Verifies that an interval with lower power than expected
    ///// is not found with any configuration.
    ///// </summary>
    ///// <remarks>
    ///// Even if this interval can be found with another configuration.
    ///// For example: When searching with short configuration in a session with one medium interval,
    ///// the interval will not be found.
    ///// </remarks>
    //[Theory]
    //[MemberData(nameof(LowerPowerThanExpected))]
    //public void Search_LowerPowerThanExpected_NotFound(IntervalValues values, Thresholds thresholds, IntervalSearchGroups searchGroup)
    //{
    //    // Arrange
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = values.DefaultTime, Power = values.MinPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup,
    //        thresholds: thresholds
    //    );
    //    finder.LogEventHandler += (s, e) =>
    //    {
    //        _output.WriteLine(e);
    //    };

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Empty(intervals);
    //}

    //#endregion // Single Simple Interval

    //#region Pauses

    ///// <summary>
    ///// Verifies that a short interval divided by a pause won't be found.
    ///// </summary>
    ///// <remarks>
    ///// When searching with short configuration in a session with one short interval
    ///// divided by a pause, the interval won't be found.
    ///// </remarks>
    //[Fact]
    //public void Search_DividedByPause_NotFound()
    //{
    //    // Arrange
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = 20, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 0, Power = 10, HearRate = 0, Cadence = 0},       // 10 second session stop
    //        new TestRecord{ Time = 20, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        IntervalSearchGroups.Short,
    //        thresholds: ShortThresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Empty(intervals);
    //}

    ///// <summary>
    ///// Verifies that a short interval divided by a pause will be found in two.
    ///// </summary>
    ///// <remarks>
    ///// When searching with short configuration in a session with one short interval
    ///// divided by a pause, two intervals will be found.
    ///// </remarks>
    //[Fact]
    //public void Search_DividedByPause_TwoFound()
    //{
    //    // Arrange
    //    int pauseTime = 10;
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 0, Power = pauseTime, HearRate = 0, Cadence = 0},       // 10 second session stop
    //        new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        IntervalSearchGroups.Short,
    //        thresholds: ShortThresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Equal(2, intervals.Count);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
    //    Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
    //    Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);

    //    Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + pauseTime), intervals[1].StartTime);
    //    Assert.Equal(ShortIntervalValues.DefaultTime, intervals[1].DurationSeconds);
    //    Assert.Equal(ShortIntervalValues.DefaultPower, intervals[1].AveragePower);
    //}

    ///// <summary>
    ///// Verifies that a short interval with a pause before its start is detected.
    ///// </summary>
    ///// <remarks>
    ///// When searching with short configuration in a session with one short interval
    ///// that has a pause a few seconds before the start, the interval will be found.
    ///// </remarks>
    //[Fact]
    //public void Search_PauseBeforeInterval_Found()
    //{
    //    // Arrange
    //    int pauseTime = 5;
    //    int timeGap = 5;    // Time between pause and interval
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 0, Power = pauseTime, HearRate = 0, Cadence = 0},
    //        new TestRecord{ Time = timeGap, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        IntervalSearchGroups.Short,
    //        thresholds: ShortThresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime + pauseTime + timeGap), intervals[0].StartTime);
    //    Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
    //    Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    //}

    ///// <summary>
    ///// Verifies that a short interval with a pause after its end is detected.
    ///// </summary>
    ///// <remarks>
    ///// When searching with short configuration in a session with one short interval
    ///// that has a pause a few seconds after the end, the interval will be found.
    ///// </remarks>
    //[Fact]
    //public void Search_PauseAfterInterval_Found()
    //{
    //    // Arrange
    //    int pauseTime = 5;
    //    int timeGap = 5;    // Time between pause and interval
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
    //        new TestRecord{ Time = timeGap, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 0, Power = pauseTime, HearRate = 0, Cadence = 0},
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        IntervalSearchGroups.Short,
    //        thresholds: ShortThresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
    //    Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
    //    Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    //}

    //#endregion // Pauses

    //#region Drops and Lifts

    //public static IEnumerable<object[]> CriticalPowerChange()
    //{
    //    // Drop
    //    yield return new object[] { ShortIntervalValues, ShortThresholds, IntervalSearchGroups.Short, 30, 0.8f };
    //    yield return new object[] { MediumIntervalValues, MediumThresholds, IntervalSearchGroups.Medium, 60, 0.75f };
    //    yield return new object[] { LongIntervalValues, LongThresholds, IntervalSearchGroups.Long, 240, 0.65f };

    //    // Lift
    //    yield return new object[] { ShortIntervalValues, ShortThresholds, IntervalSearchGroups.Short, 30, 1.2f };
    //    yield return new object[] { MediumIntervalValues, MediumThresholds, IntervalSearchGroups.Medium, 60, 1.25f };
    //    yield return new object[] { LongIntervalValues, LongThresholds, IntervalSearchGroups.Long, 240, 1.35f };
    //}
    ///// <summary>
    ///// Verifies that a interval with a critical power change is found as two separate intervals.
    ///// </summary>
    ///// <remarks>
    ///// When searching with any configuration in a session with one interval
    ///// divided by critical power change, the finder will find two separate intervals.
    ///// Even if the average power for the full interval is inside the limits.
    ///// </remarks>
    //[Theory]
    //[MemberData(nameof(CriticalPowerChange))]
    //public void Search_CriticalPowerChange_TwoFound(IntervalValues values, Thresholds thresholds, IntervalSearchGroups searchGroup, int dropTime, float dropPowerPercentage)
    //{
    //    // Arrange
    //    var dropPower = (int)(values.DefaultPower * dropPowerPercentage);
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup,
    //        thresholds: thresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Equal(2, intervals.Count);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
    //    Assert.Equal(values.DefaultTime, intervals[0].DurationSeconds);
    //    Assert.Equal(values.DefaultPower, intervals[0].AveragePower);

    //    Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(values.DefaultTime + dropTime), intervals[1].StartTime);
    //    Assert.Equal(values.DefaultTime, intervals[1].DurationSeconds);
    //    Assert.Equal(values.DefaultPower, intervals[1].AveragePower);
    //}

    //public static IEnumerable<object[]> LightPowerChange()
    //{
    //    // Drop
    //    yield return new object[] { ShortIntervalValues, ShortThresholds, IntervalSearchGroups.Short, 30, 0.9f };
    //    yield return new object[] { MediumIntervalValues, MediumThresholds, IntervalSearchGroups.Medium, 60, 0.85f };
    //    yield return new object[] { LongIntervalValues, LongThresholds, IntervalSearchGroups.Long, 240, 0.80f };
    //    // Lift
    //    yield return new object[] { ShortIntervalValues, ShortThresholds, IntervalSearchGroups.Short, 30, 1.1f };
    //    yield return new object[] { MediumIntervalValues, MediumThresholds, IntervalSearchGroups.Medium, 60, 1.15f };
    //    yield return new object[] { LongIntervalValues, LongThresholds, IntervalSearchGroups.Long, 240, 1.20f };
    //}
    ///// <summary>
    ///// Verifies that a interval with a light power change is found as one interval.
    ///// </summary>
    ///// <remarks>
    ///// When searching with any configuration in a session with one interval
    ///// divided by light power change, the finder will find a single interval.
    ///// </remarks>
    //[Theory]
    //[MemberData(nameof(LightPowerChange))]
    //public void Search_LightPowerChange_OneFound(IntervalValues values, Thresholds thresholds, IntervalSearchGroups searchGroup, int dropTime, float dropPowerPercentage)
    //{
    //    // Arrange
    //    var dropPower = (int)(values.DefaultPower * dropPowerPercentage);
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup,
    //        thresholds: thresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    int expectedPower = (values.DefaultTime * values.DefaultPower * 2 + dropTime * dropPower) / (values.DefaultTime * 2 + dropTime);
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
    //    Assert.Equal(values.DefaultTime * 2 + dropTime, intervals[0].DurationSeconds);
    //    Assert.Equal(expectedPower, intervals[0].AveragePower, 1f);
    //}

    //public static IEnumerable<object[]> CriticalShortPowerChange()
    //{
    //    // Drop
    //    yield return new object[] { ShortIntervalValues, ShortThresholds, IntervalSearchGroups.Short, 3, 0.6f };
    //    yield return new object[] { MediumIntervalValues, MediumThresholds, IntervalSearchGroups.Medium, 5, 0.5f };
    //    yield return new object[] { LongIntervalValues, LongThresholds, IntervalSearchGroups.Long, 10, 0.4f };
    //    // Lift
    //    yield return new object[] { ShortIntervalValues, ShortThresholds, IntervalSearchGroups.Short, 3, 1.4f };
    //    yield return new object[] { MediumIntervalValues, MediumThresholds, IntervalSearchGroups.Medium, 5, 1.5f };
    //    yield return new object[] { LongIntervalValues, LongThresholds, IntervalSearchGroups.Long, 10, 1.6f };
    //}
    ///// <summary>
    ///// Verifies that an interval with a short critical power drop is found as one interval.
    ///// </summary>
    ///// <remarks>
    ///// When searching with any configuration in a session with one interval
    ///// divided by a short critical power drop, the finder will find the full interval.
    ///// </remarks>
    //[Theory]
    //[MemberData(nameof(CriticalShortPowerChange))]
    //public void Search_CriticalShortPowerChange_OneFound(IntervalValues values, Thresholds thresholds, IntervalSearchGroups searchGroup, int dropTime, float dropPowerPercentage)
    //{
    //    var dropPower = (int)(values.DefaultPower * dropPowerPercentage);
    //    // Arrange
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = values.DefaultTime, Power = values.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup,
    //        thresholds: thresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    int expectedPower = (values.DefaultTime * values.DefaultPower * 2 + dropTime * dropPower) / (values.DefaultTime * 2 + dropTime);
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
    //    Assert.Equal(values.DefaultTime * 2 + dropTime, intervals[0].DurationSeconds);
    //    Assert.Equal(expectedPower, intervals[0].AveragePower, 1f);
    //}

    //#endregion // Drops and Lifts

    //#region Irregular

    ///// <summary>
    ///// Verifies that finds the exact point where the interval starts.
    ///// </summary>
    ///// <remarks>
    ///// Usually the tested intervals mantain the same power through the
    ///// interval period. This method tests if the finder can detect the exact 
    ///// start point even if the start power doesn't mantain constant.
    ///// </remarks>
    //[Fact]
    //public void Search_IrregularStart_Found()
    //{
    //    // Arrange
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 2, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 2, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},     // Should not be detected as part of the interval
    //        new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        IntervalSearchGroups.Short,
    //        thresholds: ShortThresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime + 4), intervals[0].StartTime);
    //    Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
    //    Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    //}

    ///// <summary>
    ///// Verifies that finds the exact point where the interval ends.
    ///// </summary>
    ///// <remarks>
    ///// Usually the tested intervals mantain the same power through the
    ///// interval period. This method tests if the finder can detect the exact 
    ///// end point even if the end power doesn't mantain constant.
    ///// </remarks>
    //[Fact]
    //public void Search_IrregularEnd_Found()
    //{
    //    // Arrange
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 2, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 2, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = 2, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},  // Should not be detected as part of the interval
    //        new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        IntervalSearchGroups.Short,
    //        thresholds: ShortThresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
    //    Assert.Equal(ShortIntervalValues.DefaultTime + 4, intervals[0].DurationSeconds);
    //    Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower, 4f);
    //}


    ///// <summary>
    ///// Verifies that an unstable interval won't be found.
    ///// </summary>
    ///// <remarks>
    ///// With the average power and size of the interval, the intervals should 
    ///// be detected. But given its changes, it can't be found.
    ///// </remarks>
    //[Fact]
    //public void Search_UnstableInterval_NotFound()
    //{
    //    // Arrange
    //    int totalChanges = 6;
    //    int constantPowerTime = 30;
    //    List<TestRecord> testRecords = new List<TestRecord>();
    //    for (int i = 0; i < totalChanges; i++)
    //    {
    //        testRecords.Add(
    //            new TestRecord { Time = constantPowerTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85 }
    //            );
    //        testRecords.Add(
    //            new TestRecord { Time = constantPowerTime, Power = LongIntervalValues.MinPower, HearRate = 120, Cadence = 85 }
    //            );
    //    }
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        IntervalSearchGroups.Medium,
    //        thresholds: MediumThresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Empty(intervals);
    //}

    //#endregion // Irregular

    //#region Limits

    //public static IEnumerable<object[]> SmallStepLift()
    //{
    //    yield return new object[] {
    //        ShortThresholds,
    //        IntervalSearchGroups.Short,
    //        new int[] { MediumIntervalValues.DefaultTime, ShortIntervalValues.DefaultTime },
    //        new int[] { MediumIntervalValues.MaxPower, ShortIntervalValues.DefaultPower } };
    //    yield return new object[] {
    //        MediumThresholds,
    //        IntervalSearchGroups.Medium,
    //        new int[] { LongIntervalValues.DefaultTime, MediumIntervalValues.DefaultTime },
    //        new int[] { LongIntervalValues.DefaultPower, MediumIntervalValues.DefaultPower } };
    //}
    ///// <summary>
    ///// Verifies that the interval limits are correctly defined even if there's an interval
    ///// next to it.
    ///// </summary>
    //[Theory]
    //[MemberData(nameof(SmallStepLift))]
    //public void Search_SmallStepLift_StartFoundCorrectly(
    //    Thresholds thresholds,
    //    IntervalSearchGroups searchGroup, int[] times, int[] powers)
    //{
    //    // Arrange
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = times[0], Power = powers[0], HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = times[1], Power = powers[1], HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup,
    //        thresholds: thresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(times[0]), intervals[0].StartTime);
    //    Assert.Equal(times[1], intervals[0].DurationSeconds);
    //    Assert.Equal(powers[1], intervals[0].AveragePower);
    //}

    //public static IEnumerable<object[]> SmallStepDrop()
    //{
    //    yield return new object[] {
    //        ShortThresholds,
    //        IntervalSearchGroups.Short,
    //        new int[] { ShortIntervalValues.DefaultTime, MediumIntervalValues.DefaultTime },
    //        new int[] { ShortIntervalValues.DefaultPower, MediumIntervalValues.MaxPower } };
    //    yield return new object[] {
    //        MediumThresholds,
    //        IntervalSearchGroups.Medium,
    //        new int[] { MediumIntervalValues.DefaultTime, LongIntervalValues.DefaultTime },
    //        new int[] { MediumIntervalValues.DefaultPower, LongIntervalValues.DefaultPower } };
    //}
    ///// <summary>
    ///// Verifies that the interval limits are correctly defined even if there's an interval
    ///// next to it.
    ///// </summary>
    //[Theory]
    //[MemberData(nameof(SmallStepDrop))]
    //public void Search_SmallStepDrop_EndFoundCorrectly(
    //    Thresholds thresholds,
    //    IntervalSearchGroups searchGroup, int[] times, int[] powers)
    //{
    //    // Arrange
    //    List<TestRecord> testRecords = new List<TestRecord>
    //    {
    //        new TestRecord{ Time = times[0], Power = powers[0], HearRate = 120, Cadence = 85},
    //        new TestRecord{ Time = times[1], Power = powers[1], HearRate = 120, Cadence = 85},
    //    };
    //    var records = FitnessDataService.SetData(testRecords);
    //    IntervalsFinder finder = new IntervalsFinder(
    //        PowerZones,
    //        searchGroup,
    //        thresholds: thresholds
    //    );

    //    // Act
    //    var intervals = finder.Search(records).ToList();

    //    // Assert
    //    Assert.Single(intervals);
    //    Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
    //    Assert.Equal(times[0], intervals[0].DurationSeconds);
    //    Assert.Equal(powers[0], intervals[0].AveragePower);
    //}

    //#endregion // Limits
}