using Yact.Domain.Events;
using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.ActivityClimb;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Entities;

public class Climb : AggregateRoot<ClimbId>
{
    public const int TopTimesMaxCount = 10;
    public ClimbDetails Data { get; private set; }
    public ClimbSummary Summary { get; private set; }

    private List<ClimbRecord> _topTimes;

    private Climb(
        ClimbId id,
        ClimbDetails data,
        ClimbSummary summary) : base(id)
    {
        Id = id;
        Data = data;
        Summary = summary;

        _topTimes = new List<ClimbRecord>();
    }

    public static Climb Load(
        ClimbId id,
        ClimbDetails data,
        ClimbSummary summary)
    {
        var climb = new Climb(id, data, summary);
        climb.AddDomainEvent(new ClimbLoadedEvent(id));

        return climb;
    }

    public static Climb Create(
        ClimbId id,
        ClimbDetails data,
        ClimbSummary summary)
    {
        var climb = new Climb(id, data, summary);
        climb.AddDomainEvent(new ClimbCreatedEvent(id));

        return climb;
    }

    public void SetTopTimes(List<ClimbRecord> topTimes)
    {
        if (topTimes.Count > TopTimesMaxCount)
        {
            throw new ArgumentException($"Top times count must be equal or lower than {TopTimesMaxCount}", nameof(topTimes));
        }
        _topTimes = topTimes;
    }

    public void CheckIfTopTime(ActivityClimbId activityClimbId, TimeSpan time, DateTime date)
    {
        var record = new ClimbRecord(activityClimbId, time, date);

        if (_topTimes.Count < TopTimesMaxCount || time < _topTimes.Max(t => t.Time))
        {
            _topTimes.Add(record);
            _topTimes = _topTimes.OrderBy(t => t.Time).Take(10).ToList();

            //AddDomainEvent(new NewTopTimeAchieved(Id, activityClimbId, time));
        }
    }
}
