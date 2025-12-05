using MediatR;
using Yact.Application.Interfaces;
using Yact.Domain.Entities.Activity;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Activities.UploadActivity;

public class UploadActivityHandler : IRequestHandler<UploadActivityCommand, int>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityReaderService _activityReaderService;

    public UploadActivityHandler(
        IFileStorageService fileStorage,
        IActivityRepository activityRepository,
        IActivityReaderService activityReaderService)
    {
        _fileStorage = fileStorage;
        _activityRepository = activityRepository;
        _activityReaderService = activityReaderService;
    }

    public async Task<int> Handle(UploadActivityCommand request, CancellationToken cancellationToken)
    {      
        // Read file
        var activity = await _activityReaderService.ReadActivityAsync(request.FileStream);
        activity.Info.Name = request.FileName;
        activity.Info.CreateDate = DateTime.Now;

        // Analyze file (intervals, climbs...)

        // Reset stream to read file
        request.FileStream.Position = 0;

        // Save file
        activity.Info.Path = await _fileStorage.SaveFileAsync(
            request.FileStream,
            request.FileName,
            "activities");

        // Save activity in DB
        await _activityRepository.AddAsync(activity.Info);

        return activity.Info.Id;
    }
}
