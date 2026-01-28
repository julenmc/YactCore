namespace Yact.Domain.Common.Activities.Intervals;

internal static class IntervalZones
{
    internal static readonly Dictionary<IntervalGroups, ushort> IntervalMinZones = new Dictionary<IntervalGroups, ushort>
    {
        {IntervalGroups.Short, 5},       // Short intervals must be at least at Z5
        {IntervalGroups.Medium, 3},      // Medium intervals must be at least at Z3
        {IntervalGroups.Long, 2}         // Long intervals must be at least at Z2
    };

    internal static readonly Dictionary<IntervalSearchGroups, ushort> SearchRequiredZones = new Dictionary<IntervalSearchGroups, ushort>
    {
        {IntervalSearchGroups.Short, 5},
        {IntervalSearchGroups.Medium, 3},
        {IntervalSearchGroups.Long, 2}
    };
}
