using Yact.Application.DTOs.Activity;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.Interfaces.Files;

public interface IActivityReaderService
{
    Task<ActivitySummary> ReadActivitySummaryAsync(Stream fileStream);
    Task<ActivityReadData> ReadFullActivityAsync(Stream fileStream);
}
