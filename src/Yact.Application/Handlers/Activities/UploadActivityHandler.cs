using MediatR;
using Microsoft.Extensions.Logging;
using Yact.Application.Commands.Activities;
using Yact.Application.Interfaces;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer;

namespace Yact.Application.Handlers.Activities;

public class UploadActivityHandler : IRequestHandler<UploadActivityCommand, int>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityClimbRepository _activityClimbRepository;
    private readonly IActivityReaderService _activityReaderService;
    private readonly IRouteAnalyzerService _routeAnalyzer; 
    private readonly ILogger<UploadActivityHandler> _logger;

    public UploadActivityHandler(
        IFileStorageService fileStorage,
        IActivityRepository activityRepository,
        IActivityClimbRepository activityClimbRepository,
        IActivityReaderService activityReaderService,
        IRouteAnalyzerService routeAnalyzerService,
        ILogger<UploadActivityHandler> logger)
    {
        _fileStorage = fileStorage;
        _activityRepository = activityRepository;
        _activityClimbRepository = activityClimbRepository;
        _activityReaderService = activityReaderService;
        _routeAnalyzer = routeAnalyzerService;
        _logger = logger;
    }

    public async Task<int> Handle(UploadActivityCommand request, CancellationToken cancellationToken)
    {      
        // Read file
        var activity = await _activityReaderService.ReadActivityAsync(request.FileStream);
        activity.Info.Name = request.FileName;
        activity.Info.CreateDate = DateTime.Now;

        // Analyze file (intervals, climbs...)
        if (activity.RecordData == null || activity.RecordData.Count == 0)
            throw new NoDataException();

        var activityClimbs = await _routeAnalyzer.AnalyzeRouteAsync(activity.RecordData);
        // var debugTrace = _routeAnalyzer.GetDebugTrace();

        _logger.LogInformation($"{activityClimbs.Count} climbs found:");
        foreach (var climb in activityClimbs)
        {
            _logger.LogInformation($"{climb.Data.Metrics.DistanceMeters}m at {climb.Data.Metrics.Slope}%");
        }

        // Reset stream to read file
        request.FileStream.Position = 0;

        // Save file
        activity.Info.Path = await _fileStorage.SaveFileAsync(
            request.FileStream,
            request.FileName,
            "activities");

        // Save activity in DB
        int activityId = await _activityRepository.AddAsync(activity.Info, request.CyclistId);
        await _activityClimbRepository.AddAsync(activityClimbs, activityId);   

        return activity.Info.Id;
    }
}
