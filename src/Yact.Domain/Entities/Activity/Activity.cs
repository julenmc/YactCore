using Yact.Domain.Events.Activity;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities.Activity;

public class Activity : Entity<ActivityId>
{
    public CyclistId CyclistId { get; }
    public FilePath Path { get; private set; }
    public ActivitySummary Summary { get; private set; }
    public ActivityRecords? Records { get; private set; }

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

    public void UpdateSummary(ActivitySummary? summary)
    {
        ArgumentNullException.ThrowIfNull(summary);

        Summary = summary;
    }

    public void LoadRecords(ActivityRecords records)
    {
        ArgumentNullException.ThrowIfNull(records);

        if (records.Values.Count == 0)
            throw new ArgumentException("Cannot load empty records");

        Records = records;
    }
}
