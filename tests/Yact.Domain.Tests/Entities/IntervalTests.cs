using Xunit.Abstractions;
using Yact.Domain.Entities;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using static Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer.IntervalsTestConstants;

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

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime), interval.Summary.EndTime);
        Assert.Equal(ShortIntervalValues.DefaultPower, interval.Summary.AveragePower);
    }

    [Fact]
    public void Trim_TrimStart_ReturnsCorrectValues()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = 20, Power = ShortIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + 20),
                records),
            records);

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate.AddSeconds(20),
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + 20));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(20), interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + 20), interval.Summary.EndTime);
        Assert.Equal(ShortIntervalValues.DefaultPower, interval.Summary.AveragePower);
    }

    [Fact]
    public void Trim_TrimEnd_ReturnsCorrectValues()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 20, Power = ShortIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime + 20),
                records),
            records);

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime), interval.Summary.EndTime);
        Assert.Equal(ShortIntervalValues.DefaultPower, interval.Summary.AveragePower);
    }

    [Fact]
    public void Trim_TrimSubInterval_ReturnsCorrectValues()
    {
        // Arrange
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.MaxPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = 20, Power = ShortIntervalValues.MaxPower + 50, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var interval = Interval.Create(
            IntervalId.NewId(),
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate,
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2),
                records),
            records);
        interval.AddSubInterval(
            IntervalSummary.Create(
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime),
                FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2 + 20),
                records),
            records);

        // Act
        interval.Trim(
            FitnessDataCreation.DefaultStartDate,
            FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, interval.Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2), interval.Summary.EndTime);
        Assert.Equal((ShortIntervalValues.DefaultPower + ShortIntervalValues.MaxPower) / 2, interval.Summary.AveragePower);
        Assert.Single(interval.SubIntervals);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime), interval.SubIntervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime * 2), interval.SubIntervals.First().Summary.EndTime);
        Assert.Equal(ShortIntervalValues.MaxPower, interval.SubIntervals.First().Summary.AveragePower);
    }

    #endregion
}
