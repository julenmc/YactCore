namespace Yact.Domain.Common.Activities.Intervals;

internal static class IntervalTimes
{
    public const ushort IntervalMinTime = 30;      // 30 seconds
    public const ushort SprintMinTime = 5;
    internal const ushort MediumIntervalMinTime = 240;      // Less than 4 minutes is considered a short interval
    internal const ushort LongIntervalMinTime = 600;        // Less than 10 minutes is considered a medium interval
    internal const int ShortWindowSize = 10;
    internal const int MediumWindowSize = 30;
    internal const int LongWindowSize = 60;

    internal static readonly Dictionary<IntervalSearchGroups, int> IntervalMinTimes = new Dictionary<IntervalSearchGroups, int>
    {
        {IntervalSearchGroups.Short, IntervalMinTime},            // Short intervals must be at least of 30 seconds
        {IntervalSearchGroups.Medium, MediumIntervalMinTime},     // Medium intervals must be at least of 4 minutes
        {IntervalSearchGroups.Long, LongIntervalMinTime}          // Long intervals must be at least of 10 minutes
    };

    internal static readonly Dictionary<IntervalSearchGroups, int> IntervalSearchWindows = new Dictionary<IntervalSearchGroups, int>
    {
        {IntervalSearchGroups.Short, ShortWindowSize},
        {IntervalSearchGroups.Medium, MediumWindowSize},
        {IntervalSearchGroups.Long, LongWindowSize}
    };
}
