using MediatR;
using Yact.Application.Interfaces.Files;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.Activities;

public class DeleteActivityById : IRequestHandler<DeleteActivityByIdCommand, Guid>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IActivityRepository _activityRepository;

    public DeleteActivityById(
        IFileStorageService fileStorage,
        IActivityRepository activityRepository)
    {
        _fileStorage = fileStorage;
        _activityRepository = activityRepository;
    }

    public async Task<Guid> Handle(DeleteActivityByIdCommand request, CancellationToken cancellationToken)
    {
        // Delete from DB
        var activity = await _activityRepository.RemoveByIdAsync(ActivityId.From(request.Id));
        if (activity == null)
            return Guid.Empty;  // returns empty if activity was not found in DB

        // Delete from FD
        await _fileStorage.DeleteFileAsync(activity.Path.Value);
        return activity.Id.Value;
    }
}
