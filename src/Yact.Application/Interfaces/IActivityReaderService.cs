using Yact.Domain.Entities.Activity;

namespace Yact.Application.Interfaces;

public interface IActivityReaderService
{
    Task<Activity> ReadActivityAsync(Stream fileStream);
}
