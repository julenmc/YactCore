using Xunit.Abstractions;
using Yact.Domain.Entities;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Intervals;
using static Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer.IntervalsTestConstants;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

public sealed class IntervalsCollisionServiceTests
{
    private readonly ITestOutputHelper _output;

    public IntervalsCollisionServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void RemoveCollisions_NoIntervals_NothingReturned()
    {
        // Arrange
        var intervals = new List<Interval>();
        var collisionService = new IntervalsCollisionsService();

        // Act
        collisionService.RemoveCollisions(intervals);

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void RemoveCollisions_OneInterval_SameReturned()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records),
            records);
        var intervalList = new List<Interval>()
        {
            interval
        };
        var collisionService = new IntervalsCollisionsService();

        // Act
        collisionService.RemoveCollisions(intervalList);

        // Assert
        Assert.Single(intervalList);
    }

    [Fact]
    public void RemoveCollisions_TwoWithNoCollision_SameReturned()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        var firstOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
            records);
        var firstInterval = Interval.Create(
            IntervalId.NewId(),
            firstOriginalSummary,
            records);

        var secondOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + NuleIntervalValues.DefaultTime),
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + NuleIntervalValues.DefaultTime + MediumIntervalValues.DefaultTime),
            records);
        var secondInterval = Interval.Create(
            IntervalId.NewId(),
            secondOriginalSummary,
            records);

        var intervalList = new List<Interval>()
        {
            firstInterval,
            secondInterval
        };
        var collisionService = new IntervalsCollisionsService();

        // Act
        collisionService.RemoveCollisions(intervalList);

        // Assert
        Assert.Equal(2, intervalList.Count);
        Assert.Equal(firstOriginalSummary, intervalList[0].Summary);
        Assert.Equal(secondOriginalSummary, intervalList[1].Summary);
    }

    [Fact]
    public void RemoveCollisions_IsChild_SameReturned()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        var firstOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + MediumIntervalValues.DefaultTime),
            records);
        var firstInterval = Interval.Create(
            IntervalId.NewId(),
            firstOriginalSummary,
            records);

        var secondOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + MediumIntervalValues.DefaultTime),
            records);
        var secondInterval = Interval.Create(
            IntervalId.NewId(),
            secondOriginalSummary,
            records);

        var intervalList = new List<Interval>()
        {
            firstInterval,
            secondInterval
        };
        var collisionService = new IntervalsCollisionsService();

        // Act
        collisionService.RemoveCollisions(intervalList);

        // Assert
        Assert.Equal(2, intervalList.Count);
        Assert.Equal(firstOriginalSummary, intervalList[0].Summary);
        Assert.Equal(secondOriginalSummary, intervalList[1].Summary);
    }

    [Fact]
    public void RemoveCollisions_TwoCollidePrioFirst_SecondTrimmed()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 30, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        var firstOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + 30),
            records);
        var firstInterval = Interval.Create(
            IntervalId.NewId(),
            firstOriginalSummary,
            records);

        var secondOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + MediumIntervalValues.DefaultTime + 30),
            records);
        var secondInterval = Interval.Create(
            IntervalId.NewId(),
            secondOriginalSummary,
            records);

        var intervalList = new List<Interval>()
        {
            firstInterval,
            secondInterval
        };
        var collisionService = new IntervalsCollisionsService();
        collisionService.LogEventHandler += (s, e) =>
        {
            _output.WriteLine(e);
        };

        // Act
        collisionService.RemoveCollisions(intervalList);

        // Assert
        Assert.Equal(2, intervalList.Count);
        Assert.Equal(firstOriginalSummary, intervalList[0].Summary);
        Assert.NotEqual(secondOriginalSummary, intervalList[1].Summary);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + 30), intervalList[1].Summary.StartTime);
    }

    [Fact]
    public void RemoveCollisions_TwoCollidePrioSecond_FirstTrimmed()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 30, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        var firstOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(MediumIntervalValues.DefaultTime + 30),
            records);
        var firstInterval = Interval.Create(
            IntervalId.NewId(),
            firstOriginalSummary,
            records);

        var secondOriginalSummary = IntervalSummary.Create(
            FitnessDataCreation.DefaultStartDate.AddSeconds(MediumIntervalValues.DefaultTime),
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + MediumIntervalValues.DefaultTime + 30),
            records);
        var secondInterval = Interval.Create(
            IntervalId.NewId(),
            secondOriginalSummary,
            records);

        var intervalList = new List<Interval>()
        {
            firstInterval,
            secondInterval
        };
        var collisionService = new IntervalsCollisionsService();

        // Act
        collisionService.RemoveCollisions(intervalList);

        // Assert
        Assert.Equal(2, intervalList.Count);
        Assert.Equal(secondOriginalSummary, intervalList[1].Summary);
        Assert.NotEqual(firstOriginalSummary, intervalList[0].Summary);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(MediumIntervalValues.DefaultTime), intervalList[0].Summary.EndTime);
    }
}
