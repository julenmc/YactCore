using System.Diagnostics;
using System.Security.Claims;
using System.Xml.Linq;
using Yact.Domain.Events;
using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.ActivityClimb;
using Yact.Domain.ValueObjects.Climb;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities;

public class Activity : AggregateRoot<ActivityId>
{
    public CyclistId CyclistId { get; }
    public FilePath Path { get; private set; }
    public ActivitySummary Summary { get; private set; }
    public ActivityRecords? Records { get; private set; }
    public ICollection<ActivityClimb> Climbs => _activityClimbs;

    private readonly List<ActivityClimb> _activityClimbs = new();

    private Activity() : base(default!)
    {

    }

    private Activity(
        ActivityId id, 
        CyclistId cyclistId, 
        FilePath path,
        ActivitySummary data) : base(id)
    {
        CyclistId = cyclistId;
        Path = path;
        Summary = data;
    }

    public static Activity Create(
        ActivityId activityId,
        CyclistId cyclistId,
        FilePath filePath,
        ActivitySummary data)
    {
        var activity = new Activity(
            activityId,
            cyclistId,
            filePath,
            data);

        activity.AddDomainEvent(new ActivityCreatedEvent(activityId, cyclistId));
        return activity;
    }

    public static Activity Load(
        ActivityId activityId,
        CyclistId cyclistId,
        FilePath filePath,
        ActivitySummary summary)
    {
        return new Activity(
            activityId,
            cyclistId,
            filePath,
            summary);
    }

    public void UpdateSummary(ActivitySummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        Summary = summary;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name), "Name can't be empty");
            
        Summary = Summary with { Name = name };
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            throw new ArgumentNullException(nameof(description), "Description can't be empty");

        Summary = Summary with { Description = description };
    }

    public void AddRecords(ActivityRecords records)
    {
        ArgumentNullException.ThrowIfNull(records);

        if (records.Values.Count == 0)
            throw new ArgumentException("Cannot load empty records");

        Records = records;
    }

    public void AddClimb(ClimbId climbId, double startPoint)
    {
        var activityClimb = ActivityClimb.Create(
            ActivityClimbId.NewId(),
            Id,
            climbId,
            startPoint);

        _activityClimbs.Add(activityClimb);
    }

    public void RemoveClimb(ClimbId climbId)
    {
        _activityClimbs.RemoveAll(c => c.ClimbId == climbId);
    }
}
