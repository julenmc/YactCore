namespace Yact.Domain.Entities.Activity;

public class Activity
{
    public required ActivityInfo Info { get; set; }
    public List<RecordData>? RecordData { get; set; }
}
