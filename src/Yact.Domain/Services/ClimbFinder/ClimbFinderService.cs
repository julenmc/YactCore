using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.ClimbFinder;

public class ClimbFinderService : IClimbFinderService
{
    private string _debugTrace;

    public ClimbFinderService() 
    {
        _debugTrace = string.Empty;
    }

    public string GetDebugTrace()
    {
        return _debugTrace;
    }

    public async Task FindClimbs(IEnumerable<RecordData> records)
    {
        if (records.Count() == 0)
            throw new ArgumentException($"No records given");

        _debugTrace = string.Empty;

        foreach (RecordData record in records)
        {
            // TODO: Implement climb finding logic
        }
    }
}
