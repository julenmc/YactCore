using Yact.Application.DTOs.Activity;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.Interfaces;

public interface IActivityReaderService
{
    Task<ActivitySummary> ReadActivitySummaryAsync(Stream fileStream);
    Task<ActivityReadData> ReadFullActivityAsync(Stream fileStream);
}
