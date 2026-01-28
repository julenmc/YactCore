using Xunit.Abstractions;
using Yact.Domain.Entities;
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

    #region Create Factory

    [Fact]
    public void Create_EmptyRecords_NoIntervalsReturned()
    {
        // Act
        var intervals = Interval.Create(
            PowerZones,
            new List<RecordData>(),
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Create_LowValues_NoIntervalsReturned()
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Empty(intervals);
    }

    public static IEnumerable<object[]> UniqueIntervals()
    {
        yield return new object[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.DefaultPower };
        yield return new object[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.DefaultPower };
        yield return new object[] { LongIntervalValues.DefaultTime, LongIntervalValues.DefaultPower };
    }
    [Theory]
    [MemberData(nameof(UniqueIntervals))]
    public void Create_UniqueInterval_OneReturned(int time, int power)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = time, Power = power, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Single(intervals);
    }

    [Theory]
    [MemberData(nameof(UniqueIntervals))]
    public void Create_UniqueInterval_CorrectValuesReturned(int time, int power)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = time, Power = power, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(time - 1), intervals.First().Summary.EndTime);
        Assert.Equal(power, intervals.First().Summary.AveragePower);
    }

    [Fact]
    public void Create_TwoIntervalsSeparated_TwoReturned()
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},

        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Equal(2, intervals.Count());
    }

    [Fact]
    public void Create_TwoIntervalsSeparated_CorrectValuesReturned()
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = NuleIntervalValues.DefaultTime, Power = NuleIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},

        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(ShortIntervalValues.DefaultTime - 1), intervals.First().Summary.EndTime);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals.First().Summary.AveragePower);

        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(
                ShortIntervalValues.DefaultTime + NuleIntervalValues.DefaultTime), 
            intervals.Last().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(
                ShortIntervalValues.DefaultTime * 2 + NuleIntervalValues.DefaultTime - 1),
            intervals.Last().Summary.EndTime);
        Assert.Equal(ShortIntervalValues.DefaultPower, intervals.Last().Summary.AveragePower);
    }

    public static IEnumerable<object[]> TwoMergeableIntervals()
    {
        yield return new object[] {
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MaxPower },
            new int[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.MinPower } };
        yield return new object[] {
            new int[] { LongIntervalValues.DefaultTime, LongIntervalValues.MaxPower },
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MinPower } };
    }
    [Theory]
    [MemberData(nameof(TwoMergeableIntervals))]
    public void Create_TwoIntervalsMergeable_OneReturned(int[] firstValues, int[] secondValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Single(intervals);
    }

    [Theory]
    [MemberData(nameof(TwoMergeableIntervals))]
    public void Create_TwoIntervalsMergeable_CorrectValuesReturned(int[] firstValues, int[] secondValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds),
            (s,e) =>
            {
                _output.WriteLine(e);
            });

        // Assert
        var expectedPower = (firstValues[0] * firstValues[1] + secondValues[0] * secondValues[1]) / (firstValues[0] + secondValues[0]);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] - 1), intervals.First().Summary.EndTime);
        Assert.Equal(expectedPower, intervals.First().Summary.AveragePower);
        // Sub interval
        Assert.Single(intervals.First().SubIntervals);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0]), 
            intervals.First().SubIntervals.First().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] - 1), 
            intervals.First().SubIntervals.First().Summary.EndTime);
        Assert.Equal(firstValues[1], intervals.First().SubIntervals.First().Summary.AveragePower);
    }

    public static IEnumerable<object[]> TwoNotMergeableIntervals()
    {
        yield return new object[] {
            new int[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.MaxPower },
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MinPower } };
        yield return new object[] {
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.DefaultPower },
            new int[] { LongIntervalValues.DefaultTime, LongIntervalValues.DefaultTime } };
    }
    [Theory]
    [MemberData(nameof(TwoNotMergeableIntervals))]
    public void Create_TwoIntervalsNotMergeable_TwoReturned(int[] firstValues, int[] secondValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Equal(2, intervals.Count());
    }

    [Theory]
    [MemberData(nameof(TwoNotMergeableIntervals))]
    public void Create_TwoIntervalsNotMergeable_CorrectValuesReturned(int[] firstValues, int[] secondValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] - 1), intervals.First().Summary.EndTime);
        Assert.Equal(firstValues[1], intervals.First().Summary.AveragePower);

        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0]), intervals.Last().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] - 1), intervals.Last().Summary.EndTime);
        Assert.Equal(secondValues[1], intervals.Last().Summary.AveragePower);
    }

    public static IEnumerable<object[]> ThreeMergeableIntervals()
    {
        yield return new object[] {
            new int[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.DefaultPower },
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MaxPower },
            new int[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.DefaultPower } };
        yield return new object[] {
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.DefaultPower },
            new int[] { LongIntervalValues.DefaultTime, LongIntervalValues.MaxPower },
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.DefaultPower } };
    }
    [Theory]
    [MemberData(nameof(ThreeMergeableIntervals))]
    public void Create_ThreeIntervalsMergeable_OneReturned(int[] firstValues, int[] secondValues, int[] thirdValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = thirdValues[0], Power = thirdValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Single(intervals);
    }

    [Theory]
    [MemberData(nameof(ThreeMergeableIntervals))]
    public void Create_ThreeIntervalsMergeable_CorrectValuesReturned(int[] firstValues, int[] secondValues, int[] thirdValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = thirdValues[0], Power = thirdValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds),
            (s, e) =>
            {
                _output.WriteLine(e);
            });

        // Assert
        var expectedPower = (firstValues[0] * firstValues[1] + secondValues[0] * secondValues[1] + thirdValues[0] * thirdValues[1]) / (firstValues[0] + secondValues[0] + thirdValues[0]);
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] + thirdValues[0] - 1), intervals.First().Summary.EndTime);
        Assert.Equal(expectedPower, intervals.First().Summary.AveragePower);
        // Sub intervals
        Assert.Equal(2, intervals.First().SubIntervals.Count());
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().SubIntervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] - 1), intervals.First().SubIntervals.First().Summary.EndTime);
        Assert.Equal(firstValues[1], intervals.First().SubIntervals.First().Summary.AveragePower);

        Assert.Equal(
            FitnessDataCreation.DefaultStartDate
                .AddSeconds(firstValues[0] + secondValues[0]), 
            intervals.First().SubIntervals.Last().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate
                .AddSeconds(firstValues[0] + secondValues[0] + thirdValues[0] - 1),
            intervals.First().SubIntervals.Last().Summary.EndTime);
        Assert.Equal(thirdValues[1], intervals.First().SubIntervals.Last().Summary.AveragePower);
    }

    public static IEnumerable<object[]> ThreeIntervalsFirstNotMergeable()
    {
        yield return new object[] {
            new int[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.MaxPower },
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.DefaultPower },
            new int[] { ShortIntervalValues.DefaultTime, MediumIntervalValues.MaxPower } };
        yield return new object[] {
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MaxPower },
            new int[] { LongIntervalValues.DefaultTime, LongIntervalValues.DefaultPower },
            new int[] { MediumIntervalValues.DefaultTime, LongIntervalValues.MaxPower } };
    }
    [Theory]
    [MemberData(nameof(ThreeIntervalsFirstNotMergeable))]
    public void Create_ThreeIntervalsFirstNotMergeable_TwoReturned(int[] firstValues, int[] secondValues, int[] thirdValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = thirdValues[0], Power = thirdValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Equal(2, intervals.Count());
    }

    [Theory]
    [MemberData(nameof(ThreeIntervalsFirstNotMergeable))]
    public void Create_ThreeIntervalsFirstNotMergeable_CorrectValuesReturned(int[] firstValues, int[] secondValues, int[] thirdValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = thirdValues[0], Power = thirdValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        // First interval
        Assert.Equal(FitnessDataCreation.DefaultStartDate, intervals.First().Summary.StartTime);
        Assert.Equal(FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] - 1), intervals.First().Summary.EndTime);
        Assert.Equal(firstValues[1], intervals.First().Summary.AveragePower);

        // Merged interval
        var expectedPower = (float)(secondValues[0] * secondValues[1] + thirdValues[0] * thirdValues[1]) / (secondValues[0] + thirdValues[0]);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0]), 
            intervals.Last().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] + thirdValues[0] - 1), 
            intervals.Last().Summary.EndTime);
        Assert.Equal(expectedPower, intervals.Last().Summary.AveragePower);
        // Sub interval
        Assert.Single(intervals.Last().SubIntervals);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0]), 
            intervals.Last().SubIntervals.First().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] + thirdValues[0] - 1), 
            intervals.Last().SubIntervals.First().Summary.EndTime);
        Assert.Equal(thirdValues[1], intervals.Last().SubIntervals.First().Summary.AveragePower);
    }

    public static IEnumerable<object[]> ThreeIntervalsLastNotMergeable()
    {
        yield return new object[] {
            new int[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.MinPower },
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MaxPower },
            new int[] { ShortIntervalValues.DefaultTime, ShortIntervalValues.MaxPower } };
        yield return new object[] {
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MinPower },
            new int[] { LongIntervalValues.DefaultTime, LongIntervalValues.MaxPower },
            new int[] { MediumIntervalValues.DefaultTime, MediumIntervalValues.MaxPower } };
    }
    [Theory]
    [MemberData(nameof(ThreeIntervalsFirstNotMergeable))]
    public void Create_ThreeIntervalsLastNotMergeable_TwoReturned(int[] firstValues, int[] secondValues, int[] thirdValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = thirdValues[0], Power = thirdValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        Assert.Equal(2, intervals.Count());
    }

    [Theory]
    [MemberData(nameof(ThreeIntervalsFirstNotMergeable))]
    public void Create_ThreeIntervalsLastNotMergeable_CorrectValuesReturned(int[] firstValues, int[] secondValues, int[] thirdValues)
    {
        // Arrange 
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = firstValues[0], Power = firstValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = secondValues[0], Power = secondValues[1], HearRate = 120, Cadence = 85},
            new TestRecord{ Time = thirdValues[0], Power = thirdValues[1], HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);

        // Act
        var intervals = Interval.Create(
            PowerZones,
            records,
            IntervalGroupThresholds.Create(ShortThresholds, MediumThresholds, LongThresholds));

        // Assert
        // Merged interval
        var expectedPower = (float)(secondValues[0] * secondValues[1] + firstValues[0] * firstValues[1]) / (secondValues[0] + firstValues[0]);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate,
            intervals.First().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] - 1),
            intervals.First().Summary.EndTime);
        Assert.Equal(expectedPower, intervals.First().Summary.AveragePower);
        // Sub interval
        Assert.Single(intervals.First().SubIntervals);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate,
            intervals.First().SubIntervals.First().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] - 1),
            intervals.First().SubIntervals.First().Summary.EndTime);
        Assert.Equal(firstValues[1], intervals.First().SubIntervals.First().Summary.AveragePower);

        // Last interval
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0]), 
            intervals.Last().Summary.StartTime);
        Assert.Equal(
            FitnessDataCreation.DefaultStartDate.AddSeconds(firstValues[0] + secondValues[0] + thirdValues[0] - 1), 
            intervals.Last().Summary.EndTime);
        Assert.Equal(thirdValues[1], intervals.Last().Summary.AveragePower);
    }

    #endregion
}
