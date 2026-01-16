using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Tests.Services.Analyzer.PerformanceAnalyzer;

internal class IntervalValues
{
    internal int DefaultTime { get; set; }
    internal int MaxPower { get; set; }
    internal int MinPower { get; set; }
    internal int DefaultPower { get => (MaxPower + MinPower) / 2; }
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
        { 1, new Zone(0, 129) },
        { 2, new Zone(130, NuleIntervalValues.MaxPower - 1) },
        { 3, new Zone(NuleIntervalValues.MaxPower,LongIntervalValues.MaxPower - 1)},
        { 4, new Zone(LongIntervalValues.MaxPower, MediumIntervalValues.MaxPower - 1)},
        { 5, new Zone(MediumIntervalValues.MaxPower, ShortIntervalValues.MaxPower - 1)},
        { 6, new Zone(ShortIntervalValues.MaxPower, ShortIntervalValues.MaxPower + 49)},
        { 7, new Zone(ShortIntervalValues.MaxPower + 50, 2000)}
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
