using Xunit.Abstractions;
using Yact.Domain.Entities;
using Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.Entities;

public sealed class IntervalTests
{
    private readonly ITestOutputHelper _output;

    public IntervalTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region Trim Method

    [Fact]
    public void Trim_SameLimits_ReturnsSameInterval()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(60),
                records),
            records);

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(60));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60), interval.Summary.EndTime);
        Assert.Equal(powerZones.Values[5].HighLimit, interval.Summary.AveragePower);
    }

    [Fact]
    public void Trim_TrimStart_ReturnsCorrectValues()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 20, Power = powerZones.Values[6].HighLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 20),
                records),
            records);

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate.AddSeconds(20),
            FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 20));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(20), interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 20), interval.Summary.EndTime);
        Assert.Equal(powerZones.Values[5].HighLimit, interval.Summary.AveragePower);
    }

    [Fact]
    public void Trim_TrimEnd_ReturnsCorrectValues()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 20, Power = powerZones.Values[6].HighLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(59 + 20),
                records),
            records);

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(59));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(59), interval.Summary.EndTime);
        Assert.Equal(powerZones.Values[5].HighLimit, interval.Summary.AveragePower);
    }

    [Fact]
    public void Trim_TrimSubInterval_ReturnsCorrectValues()
    {
        // Arrange
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 60, Power = powerZones.Values[6].HighLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 20, Power = powerZones.Values[6].HighLimit + 50, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 59),
                records),
            records);
        interval.AddSubInterval(
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(60),
                FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 59 + 20),
                records),
            records);

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 59));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 59), interval.Summary.EndTime);
        Assert.Equal((powerZones.Values[5].HighLimit + powerZones.Values[6].HighLimit) / 2, interval.Summary.AveragePower);
        Assert.Single(interval.SubIntervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60), interval.SubIntervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60 + 59), interval.SubIntervals.First().Summary.EndTime);
        Assert.Equal(powerZones.Values[6].HighLimit, interval.SubIntervals.First().Summary.AveragePower);
    }

    #endregion

    #region Create Factory

    [Fact]
    public void Create_EmptyRecords_NoIntervalsReturned()
    {
        // Act
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var intervals = Interval.Create(
            PowerZones.Create(IntervalsTestConstants.PowerZones),
            new List<RecordData>());

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Create_LowValues_NoIntervalsReturned()
    {
        // Arrange 
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 300, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones.Create(IntervalsTestConstants.PowerZones),
            records);

        // Assert
        Assert.Empty(intervals);
    }

    public static IEnumerable<object[]> UniqueIntervals()
    {
        yield return new object[] { 60, IntervalsTestConstants.PowerZones[5].HighLimit };
        yield return new object[] { 300, IntervalsTestConstants.PowerZones[4].LowLimit };
        yield return new object[] { 1200, IntervalsTestConstants.PowerZones[3].LowLimit };
    }
    [Theory]
    [MemberData(nameof(UniqueIntervals))]
    public void Create_UniqueInterval_OneReturned(int time, int power)
    {
        // Arrange 
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = time, Power = power, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones.Create(IntervalsTestConstants.PowerZones),
            records);

        // Assert
        Assert.Single(intervals);
    }

    [Theory]
    [MemberData(nameof(UniqueIntervals))]
    public void Create_UniqueInterval_CorrectValuesReturned(int time, int power)
    {
        // Arrange 
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = time, Power = power, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones.Create(IntervalsTestConstants.PowerZones),
            records);

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(time - 1), intervals.First().Summary.EndTime);
        Assert.Equal(power, intervals.First().Summary.AveragePower);
    }

    [Fact]
    public void Create_TwoIntervalsSeparated_TwoReturned()
    {
        // Arrange 
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 300, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},

        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones.Create(IntervalsTestConstants.PowerZones),
            records);

        // Assert
        Assert.Equal(2, intervals.Count());
    }

    [Fact]
    public void Create_TwoIntervalsSeparated_CorrectValuesReturned()
    {
        // Arrange 
        var powerZones = PowerZones.Create(IntervalsTestConstants.PowerZones);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 300, Power = powerZones.Values[2].LowLimit, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 60, Power = powerZones.Values[5].HighLimit, HearRate = 120, Cadence = 85},

        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones.Create(IntervalsTestConstants.PowerZones),
            records);

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(60 - 1), intervals.First().Summary.EndTime);
        Assert.Equal(powerZones.Values[5].HighLimit, intervals.First().Summary.AveragePower);

        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(
                60 + 300), 
            intervals.Last().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(
                60 * 2 + 300 - 1),
            intervals.Last().Summary.EndTime);
        Assert.Equal(powerZones.Values[5].HighLimit, intervals.Last().Summary.AveragePower);
    }
    #endregion
}
