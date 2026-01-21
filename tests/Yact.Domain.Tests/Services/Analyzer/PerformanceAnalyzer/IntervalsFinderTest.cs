using Xunit.Abstractions;
using Yact.Domain.Common.Activities.Intervals;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Cyclist;

using static Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer.IntervalsTestConstants;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

/// <summary>
/// Contains the unit test of the <see cref="IntervalsFinder"/> class.
/// </summary>
/// <remarks>
/// This class verifies different scenarios in each of the three interval search times:
/// Long, medium and short
/// 
/// The test follow the convention:
/// <c>Configuration_Scenario_ExpectedResult</c>.
/// The assertion of the expected result will allways contain the interval count; and for each interval:
/// interval start date, time, average power and, if necessary, sub-interval count.
/// </remarks>
public sealed class FinderUnitTests
{
    private readonly ITestOutputHelper _output;

    public FinderUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Verifies whith invalid power zones an exception is thrown.
    /// </summary>
    /// <remarks>
    /// When giving an invalid power zone list to the constructor, 
    /// an exception is thrown.
    /// </remarks>
    [Fact]
    public void Short_PowerZoneMissing_ExceptionThrown()
    {
        // Arrange
        Dictionary<int, Zone> powerZones = new Dictionary<int, Zone>()  // No zone 7
        {
            { 1, new Zone(0, 1) },
            { 2, new Zone(2, 3) },
            { 3, new Zone(4, 5) },
            { 4, new Zone(6, 7) },
            { 5, new Zone(8, 9) },
            { 6, new Zone(10, 11) },
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new IntervalsFinder(
            powerZones,
            IntervalSearchGroups.Short));
    }

    /// <summary>
    /// Verifies that no interval is found.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with no valid intervals,
    /// no interval will be found.
    /// </remarks>
    [Fact]
    public void Short_NoIntervals_NotFound()
    {
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        var finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short,
            thresholds: ShortThresholds);

        // Act
        var intervals = finder.Search(records).ToList();

        // Assertions
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a short interval is found with short configuration.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval,
    /// the interval will be found.
    /// </remarks>
    [Fact]
    public void Short_SingleShortInterval_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        var finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds);

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime), intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a short interval is found with short configuration (default thresholds).
    /// </summary>
    /// <remarks>
    /// When searching with short configuration (default thresholds) in a session with one short interval,
    /// the interval will be found.
    /// </remarks>
    [Fact]
    public void ShortDefault_SingleShortInterval_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime), intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a short interval divided by a pause won't be found.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval
    /// divided by a pause, the interval won't be found.
    /// </remarks>
    [Fact]
    public void Short_DividedByPause_NotFound()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = 20, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 0, Power = 10, HearRate = 0, Cadence = 0},       // 10 second session stop
            new TestRecord{ Time = 20, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a short interval divided by a pause will be found in two.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval
    /// divided by a pause, two intervals will be found.
    /// </remarks>
    [Fact]
    public void Short_DividedByPause_TwoFound()
    {
        // Arrange
        int pauseTime = 10;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 0, Power = pauseTime, HearRate = 0, Cadence = 0},       // 10 second session stop
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Equal(2, intervals.Count);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);

        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + pauseTime), intervals[1].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[1].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[1].AveragePower);
    }

    /// <summary>
    /// Verifies that a short interval with a pause before its start is detected.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval
    /// that has a pause a few seconds before the start, the interval will be found.
    /// </remarks>
    [Fact]
    public void Short_PauseBeforeInterval_Found()
    {
        // Arrange
        int pauseTime = 5;
        int timeGap = 5;    // Time between pause and interval
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 0, Power = pauseTime, HearRate = 0, Cadence = 0},
            new TestRecord{ Time = timeGap, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime + pauseTime + timeGap), intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a short interval with a pause after its end is detected.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval
    /// that has a pause a few seconds after the end, the interval will be found.
    /// </remarks>
    [Fact]
    public void Short_PauseAfterInterval_Found()
    {
        // Arrange
        int pauseTime = 5;
        int timeGap = 5;    // Time between pause and interval
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 90},
            new TestRecord{ Time = timeGap, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 0, Power = pauseTime, HearRate = 0, Cadence = 0},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that already saved intervals won't be saved again.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval,
    /// if the found interval is already in the container, it won't be saved again.
    /// </remarks>
    [Fact]
    public void Short_AlreadyFound_NotSaved()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        List<IntervalSummary> foundIntervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime - 1),
                records)
        };
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            foundIntervals,
            ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a medium interval is not found with short configuration.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one medium interval,
    /// the interval will not be found.
    /// </remarks>
    [Fact]
    public void Short_SingleMediumInterval_NotFound()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a short interval at limit power but that doesn't pass the threshold
    /// isn't found with short configuration.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval
    /// with the average power at the limit, but that doesn't pass the finding low threshold, 
    /// the interval won't be found.
    /// </remarks>
    [Fact]
    public void Short_SingleShortBellowThreshold_NotFound()
    {
        // Arrange
        int intervalTime = 200;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = intervalTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a short interval at limit power is found with short configuration.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval
    /// with the average power at the limit, the interval will be found.
    /// </remarks>
    [Fact]
    public void Short_SingleShortAverageLimit_Found()
    {
        // Arrange
        int intervalTime = 200;
        int firstPower = ShortIntervalValues.MinPower;
        int secondPower = MediumIntervalValues.DefaultPower;
        int averagePower = MediumIntervalValues.MaxPower;
        int intervalFirstPeriod = (averagePower - secondPower) * intervalTime / (firstPower - secondPower);
        int intervalSecondPeriod = intervalTime - intervalFirstPeriod;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = intervalFirstPeriod, Power = firstPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = intervalSecondPeriod, Power = secondPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime), intervals[0].StartTime);
        Assert.Equal(intervalTime, intervals[0].DurationSeconds);
        Assert.Equal(averagePower, intervals[0].AveragePower, 10f);
    }

    /// <summary>
    /// Verifies that a short interval with a power drop is found as two separate intervals.
    /// </summary>
    /// <remarks>
    /// When searching with short configuration in a session with one short interval
    /// divided by a 30 second ~20% power drop, the finder will find two separate intervals.
    /// Even if the average power for the full interval is higher than the low limit.
    /// </remarks>
    [Fact]
    public void Short_ShortIntervalDrop_TwoFound()
    {
        // Arrange
        int dropTime = 30;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = dropTime, Power = MediumIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short,
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Equal(2, intervals.Count);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
        
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + dropTime), intervals[1].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[1].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[1].AveragePower);
    }

    /// <summary>
    /// Verifies that a medium interval with a high power is found with short configuration.
    /// </summary>
    /// <remarks>
    /// This tests the improbable scenario where a medium size interval with the power of a 
    /// short size interval appears in the session. When searching with short configuration, 
    /// the finder will find the interval.
    /// </remarks>
    [Fact]
    public void Short_MediumIntervalHighPower_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that finds the exact point where the interval starts.
    /// </summary>
    /// <remarks>
    /// Usually the tested intervals mantain the same power through the
    /// interval period. This method tests if the finder can detect the exact 
    /// start point even if the start power doesn't mantain constant.
    /// </remarks>
    [Fact]
    public void Short_IrregularStart_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 2, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 2, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},     // Should not be detected as part of the interval
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime + 4), intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that finds the exact point where the interval ends.
    /// </summary>
    /// <remarks>
    /// Usually the tested intervals mantain the same power through the
    /// interval period. This method tests if the finder can detect the exact 
    /// end point even if the end power doesn't mantain constant.
    /// </remarks>
    [Fact]
    public void Short_IrregularEnd_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 2, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 2, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 2, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},  // Should not be detected as part of the interval
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Short, 
            thresholds: ShortThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(ShortIntervalValues.DefaultTime + 4, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower, 4f);
    }

    /// <summary>
    /// Verifies that a short interval is not found with medium configuration.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one short interval,
    /// the interval won't be found.
    /// </remarks>
    [Fact]
    public void Medium_SingleShortInterval_NotFound()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium,
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a medium interval is found with medium configuration.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one medium interval,
    /// the interval will be found.
    /// </remarks>
    [Fact]
    public void Medium_SingleMediumInterval_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(MediumIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a medium interval is found with medium configuration (default thresholds).
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration (default thresholds) in a session with one medium interval,
    /// the interval will be found.
    /// </remarks>
    [Fact]
    public void MediumDefault_SingleMediumInterval_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime), intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(MediumIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a long interval with low poer isn't found with medium configuration.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one long interval with low power,
    /// the interval won't be found.
    /// </remarks>
    [Fact]
    public void Medium_SingleLongInterval_NotFound()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.MinPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium,
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();
        //var debugTrace = finder.GetDebugTrace();
        //foreach (var trace in debugTrace)
        //{
        //    _output.WriteLine(trace);
        //}

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a medium interval with a high power is found with medium configuration.
    /// </summary>
    /// <remarks>
    /// This tests the improbable scenario where a medium size interval with the power of a 
    /// short size interval appears in the session. When searching with medium configuration, 
    /// the finder will find the interval.
    /// </remarks>
    [Fact]
    public void Medium_MediumIntervalHighPower_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a long interval with a high power is found with medium configuration.
    /// </summary>
    /// <remarks>
    /// This tests the improbable scenario where a long size interval with the power of a 
    /// medium size interval appears in the session. When searching with medium configuration, 
    /// the finder will find the interval.
    /// </remarks>
    [Fact]
    public void Medium_LongIntervalHighPower_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(MediumIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a medium interval with a power drop is found as one interval.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one medium interval
    /// divided by a 60 second ~15% power drop, the finder will find the full interval.
    /// </remarks>
    [Fact]
    public void Medium_MediumIntervalDrop_OneFound()
    {
        // Arrange
        int dropTime = 60;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = dropTime, Power = LongIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        int expectedPower = (MediumIntervalValues.DefaultTime * MediumIntervalValues.MaxPower * 2 + dropTime * LongIntervalValues.MaxPower) / (MediumIntervalValues.DefaultTime * 2 + dropTime);
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime * 2 + dropTime, intervals[0].DurationSeconds);
        Assert.Equal(expectedPower, intervals[0].AveragePower, 1f);
    }

    /// <summary>
    /// Verifies that a medium interval with a short critical power drop is found as one interval.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one medium interval
    /// divided by a 5 second ~50% power drop, the finder will find the full interval.
    /// </remarks>
    [Fact]
    public void Medium_MediumIntervalCriticalDrop_OneFound()
    {
        // Arrange
        int dropTime = 20;
        int dropPower = 135;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        int expectedPower = (MediumIntervalValues.DefaultTime * MediumIntervalValues.MaxPower * 2 + dropTime * dropPower) / (MediumIntervalValues.DefaultTime * 2 + dropTime);
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime * 2 + dropTime, intervals[0].DurationSeconds);
        Assert.Equal(expectedPower, intervals[0].AveragePower, 1f);
    }

    /// <summary>
    /// Verifies that a medium interval with a power drop is found as two separate intervals.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one medium interval
    /// divided by a 60 second ~30% power drop, the finder will find two separate intervals.
    /// Even if the average power for the full interval is higher than the low limit.
    /// </remarks>
    [Fact]
    public void Medium_MediumIntervalDrop_TwoFound()
    {
        // Arrange
        int dropTime = 60;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = dropTime, Power = LongIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Equal(2, intervals.Count);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(MediumIntervalValues.MaxPower, intervals[0].AveragePower);
        
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(MediumIntervalValues.DefaultTime + dropTime), intervals[1].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[1].DurationSeconds);
        Assert.Equal(MediumIntervalValues.MaxPower, intervals[1].AveragePower);
    }

    /// <summary>
    /// Verifies that a medium interval with a power lift is found as one interval.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one medium interval
    /// divided by a 60 second ~15% power lift, the finder will find the full interval.
    /// </remarks>
    [Fact]
    public void Medium_MediumIntervalLift_OneFound()
    {
        // Arrange
        int liftTime = 60;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = liftTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        int expectedPower = (MediumIntervalValues.DefaultTime * MediumIntervalValues.DefaultPower * 2 + liftTime * ShortIntervalValues.MinPower) / (MediumIntervalValues.DefaultTime * 2 + liftTime);
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime * 2 + liftTime, intervals[0].DurationSeconds);
        Assert.Equal(expectedPower, intervals[0].AveragePower, 1f);
    }

    /// <summary>
    /// Verifies that a medium interval with a power lift is found as two separate intervals.
    /// </summary>
    /// <remarks>
    /// When searching with medium configuration in a session with one medium interval
    /// divided by a 60 second ~35% power lift, the finder will find two separate intervals.
    /// Even if the average power for the full interval is lower than the high limit.
    /// </remarks>
    [Fact]
    public void Medium_MediumIntervalLift_TwoFound()
    {
        // Arrange
        int liftTime = 60;
        int liftPower = 350;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = liftTime, Power = liftPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Equal(2, intervals.Count);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(MediumIntervalValues.DefaultPower, intervals[0].AveragePower);
        
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(MediumIntervalValues.DefaultTime + liftTime), intervals[1].StartTime);
        Assert.Equal(MediumIntervalValues.DefaultTime, intervals[1].DurationSeconds);
        Assert.Equal(MediumIntervalValues.DefaultPower, intervals[1].AveragePower);
    }

    /// <summary>
    /// Verifies that an unstable interval won't be found.
    /// </summary>
    /// <remarks>
    /// With the average power and size of the interval, the intervals should 
    /// be detected. But given its changes, it can't be found.
    /// </remarks>
    [Fact]
    public void Medium_UnstableMedium_Found()
    {
        // Arrange
        int totalChanges = 6;
        int constantPowerTime = 30;
        List<TestRecord> fitnessTestSections = new List<TestRecord>();
        for (int i = 0; i < totalChanges; i++)
        {
            fitnessTestSections.Add(
                new TestRecord { Time = constantPowerTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85 }
                );
            fitnessTestSections.Add(
                new TestRecord { Time = constantPowerTime, Power = LongIntervalValues.MinPower, HearRate = 120, Cadence = 85 }
                );
        }
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Medium, 
            thresholds: MediumThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a medium interval is not found with long configuration.
    /// </summary>
    /// <remarks>
    /// When searching with long configuration in a session with one medium interval,
    /// the interval won't be found.
    /// </remarks>
    [Fact]
    public void Long_SingleMediumInterval_NotFound()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long,
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Empty(intervals);
    }

    /// <summary>
    /// Verifies that a long interval is found with long configuration.
    /// </summary>
    /// <remarks>
    /// When searching with long configuration in a session with one long interval,
    /// the interval will be found.
    /// </remarks>
    [Fact]
    public void Long_SingleLongInterval_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(LongIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a long interval is found with long configuration (default thresholds).
    /// </summary>
    /// <remarks>
    /// When searching with long configuration (default thresholds) in a session with one long interval,
    /// the interval will be found.
    /// </remarks>
    [Fact]
    public void LongDefault_SingleLongInterval_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long
        );
        finder.LogEventHandler += (s, e) =>
        {
            _output.WriteLine(e);
        };

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(NuleIntervalValues.DefaultTime), intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(LongIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a very long interval is found with long configuration.
    /// </summary>
    /// <remarks>
    /// When searching with long configuration in a session with one very long interval,
    /// the interval will be found.
    /// </remarks>
    [Fact]
    public void Long_VeryLongInterval_Found()
    {
        // Arrange
        int intervalTime = 60 * 60 * 2; // 2 hours
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = intervalTime, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(intervalTime, intervals[0].DurationSeconds);
        Assert.Equal(LongIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a long interval with a high power is found with long configuration.
    /// </summary>
    /// <remarks>
    /// This tests the improbable scenario where a long size interval with the power of a 
    /// medium size interval appears in the session. When searching with long configuration, 
    /// the finder will find the interval.
    /// </remarks>
    [Fact]
    public void Long_LongIntervalHighPower_Found()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(MediumIntervalValues.DefaultPower, intervals[0].AveragePower);
    }

    /// <summary>
    /// Verifies that a long interval with a power drop is found as one interval.
    /// </summary>
    /// <remarks>
    /// When searching with long configuration in a session with one long interval
    /// divided by a 240 second ~20% power drop, the finder will find the full interval.
    /// </remarks>
    [Fact]
    public void Long_LongIntervalDrop_OneFound()
    {
        // Arrange
        int dropTime = 240;
        int dropPower = 165;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime / 2, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = LongIntervalValues.DefaultTime / 2, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        int expectedPower = (LongIntervalValues.DefaultTime * LongIntervalValues.DefaultPower + dropTime * dropPower) / (LongIntervalValues.DefaultTime + dropTime);
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime + dropTime, intervals[0].DurationSeconds);
        Assert.Equal(expectedPower, intervals[0].AveragePower, 1f);
    }

    /// <summary>
    /// Verifies that a long interval with a power drop is found as two separate intervals.
    /// </summary>
    /// <remarks>
    /// When searching with long configuration in a session with one long interval
    /// divided by a 240 second ~35% power drop, the finder will find two separate intervals.
    /// Even if the average power for the full interval is higher than the low limit.
    /// </remarks>
    [Fact]
    public void Long_LongIntervalDrop_TwoFound()
    {
        // Arrange
        int dropTime = 240;
        int dropPower = 140;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = dropTime, Power = dropPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Equal(2, intervals.Count);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(LongIntervalValues.MaxPower, intervals[0].AveragePower);
       
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(LongIntervalValues.DefaultTime + dropTime), intervals[1].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[1].DurationSeconds);
        Assert.Equal(LongIntervalValues.MaxPower, intervals[1].AveragePower);
    }

    /// <summary>
    /// Verifies that a long interval with a power lift is found as one interval.
    /// </summary>
    /// <remarks>
    /// When searching with long configuration in a session with one long interval
    /// divided by a 240 second ~25% power lift, the finder will find the full interval.
    /// </remarks>
    [Fact]
    public void Long_LongIntervalLift_OneFound()
    {
        // Arrange
        int liftTime = 240;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime / 2, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = liftTime, Power = MediumIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = LongIntervalValues.DefaultTime / 2, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        int expectedPower = (LongIntervalValues.DefaultTime * LongIntervalValues.DefaultPower + liftTime * MediumIntervalValues.MinPower) / (LongIntervalValues.DefaultTime + liftTime);
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime + liftTime, intervals[0].DurationSeconds);
        Assert.Equal(expectedPower, intervals[0].AveragePower, 1f);
    }

    /// <summary>
    /// Verifies that a long interval with a power lift is found as two separate intervals.
    /// </summary>
    /// <remarks>
    /// When searching with long configuration in a session with one long interval
    /// divided by a 240 second ~45% power lift, the finder will find two separate intervals.
    /// Even if the average power for the full interval is lower than the high limit.
    /// </remarks>
    [Fact]
    public void Long_LongIntervalLift_TwoFound()
    {
        // Arrange
        int liftTime = 240;
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = liftTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Equal(2, intervals.Count);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(LongIntervalValues.DefaultPower, intervals[0].AveragePower);
     
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(LongIntervalValues.DefaultTime + liftTime), intervals[1].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[1].DurationSeconds);
        Assert.Equal(LongIntervalValues.DefaultPower, intervals[1].AveragePower);
    }

    /// <summary>
    /// Verifies that a when the start of a short interval is just after
    /// the end of a long interval, the long one is found correctly,
    /// while the short is not found.
    /// </summary>
    /// <remarks>
    /// There will be two intervals, one long sized, the other short sized, the second will be at a higher power
    /// than the first one. The power differential will separate them. 
    /// </remarks>
    [Fact]
    public void Long_ShortAfterLongWithLongConfig_TwoFound()
    {
        // Arrange
        List<TestRecord> fitnessTestSections = new List<TestRecord>
        {
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(fitnessTestSections);
        IntervalsFinder finder = new IntervalsFinder(
            PowerZones,
            IntervalSearchGroups.Long, 
            thresholds: LongThresholds
        );

        // Act
        var intervals = finder.Search(records).ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[0].StartTime);
        Assert.Equal(LongIntervalValues.DefaultTime, intervals[0].DurationSeconds);
        Assert.Equal(LongIntervalValues.DefaultPower, intervals[0].AveragePower);
    }
}