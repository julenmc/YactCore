using Xunit.Abstractions;
using Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;
using Yact.Domain.ValueObjects.Activity.Intervals;
using static Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer.IntervalsTestConstants;

namespace Yact.Domain.Tests.ValueObjects.Activity;

public sealed class IntervalSummaryTests
{
    private readonly ITestOutputHelper _output;

    public IntervalSummaryTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_SameInterval()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart;
        var secondEnd = firstEnd;
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.Same, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_SameStartEarlierEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart;
        var secondEnd = firstEnd.AddSeconds(10);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.IsParent, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_SameStartLaterEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart;
        var secondEnd = firstEnd.AddSeconds(-10);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.IsChild, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_EarlierStartSameEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart.AddSeconds(10);
        var secondEnd = firstEnd;
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.IsChild, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_EarlierStartEarlierEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart.AddSeconds(10);
        var secondEnd = firstEnd.AddSeconds(10);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.After, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_EarlierStartLaterEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart.AddSeconds(10);
        var secondEnd = firstEnd.AddSeconds(-10);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.IsChild, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_LaterStartSameEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart.AddSeconds(-10);
        var secondEnd = firstEnd;
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.IsParent, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_LaterStartEarlierEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart.AddSeconds(-10);
        var secondEnd = firstEnd.AddSeconds(10);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.IsParent, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_LaterStartLaterEnd()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart.AddSeconds(-10);
        var secondEnd = firstEnd.AddSeconds(-10);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.Before, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_NoCollisionBefore()
    {
        // Arrange
        var firstStart = FitnessDataCreation.DefaultStartDate;
        var firstEnd = firstStart.AddSeconds(30);
        var secondStart = firstStart.AddSeconds(40);
        var secondEnd = firstEnd.AddSeconds(40);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.None, collision);
    }

    [Fact]
    public void CheckCollisionWithOtherInterval_NoCollisionAfter()
    {
        // Arrange
        var secondStart = FitnessDataCreation.DefaultStartDate;
        var secondEnd = secondStart.AddSeconds(30);
        var firstStart = secondStart.AddSeconds(40);
        var firstEnd = secondEnd.AddSeconds(40);
        var testRecords = new List<TestRecord>
        {
            new TestRecord{ Time = ShortIntervalValues.DefaultTime, Power = ShortIntervalValues.DefaultPower, HearRate = 120, Cadence = 85},
        };
        var records = FitnessDataService.SetData(testRecords);
        var firstInterval = IntervalSummary.Create(firstStart, firstEnd, records);
        var secondInterval = IntervalSummary.Create(secondStart, secondEnd, records);

        // Act
        var collision = firstInterval.CheckCollisionWithOtherInterval(secondInterval);

        // Arrange
        Assert.Equal(IntervalSummary.Collision.None, collision);
    }
}
