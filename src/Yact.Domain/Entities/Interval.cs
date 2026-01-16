using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Activity.Intervals;

namespace Yact.Domain.Entities;

public class Interval : Entity<IntervalId>
{
    public IntervalData Data { get; }

    private Interval(
        IntervalId id,
        IntervalData data) : base(id)
    {
        Data = data;
    }

    internal static IEnumerable<Interval> Create()
    {
        throw new NotImplementedException();
    }
}
