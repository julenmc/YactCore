using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.ClimbFinder;

public interface IClimbFinderService
{
    Task FindClimbs(IEnumerable<RecordData> records);
    string GetDebugTrace();
}
