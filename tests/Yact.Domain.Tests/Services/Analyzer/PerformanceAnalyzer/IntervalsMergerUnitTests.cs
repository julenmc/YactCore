using Xunit.Abstractions;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using static Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer.IntervalsTestConstants;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

public sealed class IntervalsMergerUnitTests
{
    private readonly ITestOutputHelper _output;

    public IntervalsMergerUnitTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CreateAndInsertMerged_NoIntervals_ReturnEmpty()
    {
        // Arrange
        var records = new List<RecordData>();
        var intervals = new List<IntervalSummary>();
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void CreateAndInsertMerged_OneInterval_ReturnEqual()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var intervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records)
        };
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert
        Assert.Single(intervals);
    }

    [Fact]
    public void CreateAndInsertMerged_TwoDontCollide_NoInsertion()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var intervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + NuleIntervalValues.DefaultTime),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2 + NuleIntervalValues.DefaultTime),
                records),
        };
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert
        Assert.Equal(2, intervals.Count);
    }

    [Fact]
    public void CreateAndInsertMerged_TwoCanBeMerged_InsertsMerged()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = MediumIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var intervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + MediumIntervalValues.DefaultTime),
                records),
        };
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert
        Assert.Equal(3, intervals.Count);
    }

    [Fact]
    public void CreateAndInsertMerged_TwoCanBeMerged_ReturnCorrectMergedValues()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var intervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
                records),
        };
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert, the merged is inserted between both intervals
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[1].StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
            intervals[1].EndTime);
        Assert.Equal((ShortIntervalValues.MinPower + MediumIntervalValues.MaxPower) / 2,
            intervals[1].AveragePower);
    }

    [Fact]
    public void CreateAndInsertMerged_ThreeCanBeMerged_InsertsMerged()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var intervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 3),
                records),
        };
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert
        Assert.Equal(6, intervals.Count);   // There should be 2 merged between the smalls, and one big merged that contains all
    }

    [Fact]
    public void CreateAndInsertMerged_ThreeCanBeMerged_ReturnsCorrectMergedValues()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = MediumIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.MinPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var intervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 3),
                records),
        };
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert
        // The merged between 1 and 2
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[1].StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
            intervals[1].EndTime);
        Assert.Equal((ShortIntervalValues.MinPower + MediumIntervalValues.MaxPower) / 2,
            intervals[1].AveragePower);
        // The merged between 2 and 3
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime), 
            intervals[4].StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 3),
            intervals[4].EndTime);
        Assert.Equal((ShortIntervalValues.MinPower + MediumIntervalValues.MaxPower) / 2,
            intervals[4].AveragePower);
        // The full merge
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals[1].StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 3),
            intervals[3].EndTime);
        Assert.Equal((ShortIntervalValues.MinPower * 2 + MediumIntervalValues.MaxPower) / 3,
            intervals[3].AveragePower);
    }

    [Fact]
    public void CreateAndInsertMerged_TwoCantBeMerged_ReturnEqual()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultTime, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = LongIntervalValues.DefaultTime, Power = LongIntervalValues.DefaultTime, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var intervals = new List<IntervalSummary>()
        {
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                records),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + LongIntervalValues.DefaultTime),
                records),
        };
        var merger = new IntervalsMerger(records);

        // Act
        merger.CreateAndInsertMerged(intervals);

        // Assert
        Assert.Equal(2, intervals.Count);
    }
}