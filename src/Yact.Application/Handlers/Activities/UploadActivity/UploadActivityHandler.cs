using MediatR;
using Yact.Application.Interfaces;
using Yact.Domain.Models;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Activities.UploadActivity;

public class UploadActivityHandler : IRequestHandler<UploadActivityCommand, int>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IActivityRepository _activityRepository;

    public UploadActivityHandler(
        IFileStorageService fileStorage,
        IActivityRepository activityRepository)
    {
        _fileStorage = fileStorage;
        _activityRepository = activityRepository;
    }

    public async Task<int> Handle(UploadActivityCommand request, CancellationToken cancellationToken)
    {
        // Save file
        var filePath = await _fileStorage.SaveFileAsync(
            request.FileStream,
            request.FileName,
            "activities");

        // Reset stream to read file
        request.FileStream.Position = 0;

        // Analyze file (intervals, climbs...)

        // Create entity with data
        var activity = new Activity
        {
            Name = request.FileName,
            Path = filePath,
            CreateDate = DateTime.Now,
        };

        // Save activity in DB
        await _activityRepository.AddAsync(activity);

        return activity.Id;
    }
}
