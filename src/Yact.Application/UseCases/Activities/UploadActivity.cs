using MediatR;
using Yact.Application.Interfaces;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.Activities;

public class UploadActivity : IRequestHandler<UploadActivityCommand, int>
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

    public async Task<int> Handle(UploadActivityCommand request, CancellationToken cancellationToken)
    {      
        // Read file
        var readData = await _activityReaderService.ReadActivitySummaryAsync(request.FileStream);
        var summary = ActivitySummary.Create(
            readData.Name,
            readData.Type,
            readData.Records.First().DateTime,
            readData.Records.Last().DateTime);

        // Reset stream to read file
        request.FileStream.Position = 0;

        // Save file
        var path = await _fileStorage.SaveFileAsync(
            request.FileStream,
            request.FileName,
            "activities");

        // Save activity in DB
        var entity = await _activityRepository.AddAsync(summary, path, request.CyclistId);

        return entity.Id.Value;
    }
}
