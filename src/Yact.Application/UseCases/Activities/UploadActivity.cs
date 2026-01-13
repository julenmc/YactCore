using MediatR;
using Yact.Application.Interfaces;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Activities;

public class UploadActivity : IRequestHandler<UploadActivityCommand, Guid>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityReaderService _activityReaderService;

    public UploadActivity(
        IFileStorageService fileStorage,
        IActivityRepository activityRepository,
        IActivityReaderService activityReaderService)
    {
        _fileStorage = fileStorage;
        _activityRepository = activityRepository;
        _activityReaderService = activityReaderService;
    }

    public async Task<Guid> Handle(UploadActivityCommand request, CancellationToken cancellationToken)
    {      
        // Read file
        var summary = await _activityReaderService.ReadActivitySummaryAsync(request.FileStream);

        // Reset stream to read file
        request.FileStream.Position = 0;

        // Save file
        var path = await _fileStorage.SaveFileAsync(
            request.FileStream,
            request.FileName,
            "activities");

        var newActivityId = ActivityId.NewId();
        var activity = Activity.Create(ActivityId.NewId(), CyclistId.From(request.CyclistId), FilePath.From(path), summary);

        // Save activity in DB
        activity = await _activityRepository.AddAsync(activity);
        return activity.Id.Value;
    }
}
