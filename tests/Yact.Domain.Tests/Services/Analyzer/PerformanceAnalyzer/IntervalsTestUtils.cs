using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

public class IntervalValues
{
    public int DefaultTime { get; set; }
    public int MaxPower { get; set; }
    public int MinPower { get; set; }
    public int DefaultPower { get => (MaxPower + MinPower) / 2; }
}

internal static class IntervalsTestConstants
{
    internal readonly static IntervalValues NuleIntervalValues = new IntervalValues()
    {
        DefaultTime = 300,
        MaxPower = 180,
        MinPower = 0
    };
    internal readonly static IntervalValues ShortIntervalValues = new IntervalValues()
    {
        DefaultTime = 90,
        MaxPower = 320,
        MinPower = 300
    };
    internal readonly static IntervalValues MediumIntervalValues = new IntervalValues()
    {
        DefaultTime = 360,
        MaxPower = 270,
        MinPower = 250
    };
    internal readonly static IntervalValues LongIntervalValues = new IntervalValues()
    {
        DefaultTime = 900,
        MaxPower = 220,
        MinPower = 190
    };
    internal const float ShortAcpDelta = 0.25f;   // 25% time delta accepted for short intervals
    internal const float MediumAcpDelta = 0.2f; // 20% time delta accepted for medium intervals
    internal const float LongAcpDelta = 0.1f;   // 10% time delta accepted for long intervals

    internal static readonly Dictionary<int, Zone> PowerZones = new Dictionary<int, Zone>{
        { 1, Zone.Create(0, 138) },
        { 2, Zone.Create(139, 189) },
        { 3, Zone.Create(190, 227)},
        { 4, Zone.Create(228, 265)},
        { 5, Zone.Create(266, 303)},
        { 6, Zone.Create(304, 379)},
        { 7, Zone.Create(380, 2000)}
    };

    internal readonly static Thresholds ShortThresholds = new Thresholds { CvStart = 0.15f, CvFollow = 0.20f, Range = 0.20f, MaRel = 0.15f };
    internal readonly static Thresholds MediumThresholds = new Thresholds { CvStart = 0.30f, CvFollow = 0.30f, Range = 0.50f, MaRel = 0.20f };
    internal readonly static Thresholds LongThresholds = new Thresholds { CvStart = 0.40f, CvFollow = 0.40f, Range = 1.00f, MaRel = 0.30f };
}

internal static class FitnessDataCreation
{
    internal static readonly DateTime DefaultStartDate = new DateTime(2025, 07, 14, 12, 00, 00);
}

internal record TestRecord
{
    internal int Time;
    internal int Power;
    internal int HearRate;
    internal int Cadence;
}

internal static class FitnessDataService
{
    internal static List<RecordData> SetData(List<TestRecord> fitnessTestSections)
    {
        List<RecordData> records = new List<RecordData>();
        DateTime startDate = FitnessDataCreation.DefaultStartDate;
        foreach (TestRecord section in fitnessTestSections)
        {
            // Check if session has stopped
            if (section.Time == 0)
            {
                startDate = startDate.AddSeconds(section.Power);        // Workaround to add stopped time to a session, the time is given with the power
                continue;
            }
            for (int i = 0; i < section.Time; i++)
            {
                records.Add(new RecordData
                {
                    Timestamp = startDate,
                    Performance = new Performance
                    {
                        Power = section.Power,
                        HeartRate = section.HearRate,
                        Cadence = section.Cadence,
                    },
                    Coordinates = new Coordinates(),
                    SmoothedAltitude = new SmoothedAltitude(),
                    DistanceMeters = 0,
                });
                startDate = startDate.AddSeconds(1);
            }
        }

        return records;
    }
}
