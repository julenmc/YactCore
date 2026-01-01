using Yact.Application.DTOs.Activity;

namespace Yact.Application.Interfaces;

public interface IActivityReaderService
{
    Task<ActivityReadData> ReadActivitySummaryAsync(Stream fileStream);
    Task<ActivityReadData> ReadFullActivityAsync(Stream fileStream);
}
