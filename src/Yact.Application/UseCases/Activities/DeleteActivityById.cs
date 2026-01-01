using MediatR;
using Yact.Application.Interfaces;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Activities;

public class DeleteActivityById : IRequestHandler<DeleteActivityByIdCommand, int>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityReaderService _activityReaderService;

    public DeleteActivityById(
        IFileStorageService fileStorage,
        IActivityRepository activityRepository,
        IActivityReaderService activityReaderService)
    {
        _fileStorage = fileStorage;
        _activityRepository = activityRepository;
        _activityReaderService = activityReaderService;
    }

    public async Task<int> Handle(DeleteActivityByIdCommand request, CancellationToken cancellationToken)
    {
        // Delete from DB
        var activity = await _activityRepository.RemoveByIdAsync(request.Id);
        if (activity == null)
            return -1;  // returns -1 if activity was not found in DB

        // Delete from FD
        await _fileStorage.DeleteFileAsync(activity.Path.Value);
        return activity.Id.Value;
    }
}
