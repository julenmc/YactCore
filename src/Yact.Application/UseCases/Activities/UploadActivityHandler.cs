using MediatR;
using Microsoft.Extensions.Logging;
using Yact.Application.Commands.Activities;
using Yact.Application.Interfaces;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer;

namespace Yact.Application.UseCases.Activities;

public class UploadActivityHandler : IRequestHandler<UploadActivityCommand, int>
{
    private readonly IFileStorageService _fileStorage;
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityClimbRepository _activityClimbRepository;
    private readonly IClimbRepository _climbRepository;
    private readonly IActivityReaderService _activityReaderService;
    private readonly IRouteAnalyzerService _routeAnalyzer; 
    private readonly ILogger<UploadActivityHandler> _logger;

    public UploadActivityHandler(
        IFileStorageService fileStorage,
        IActivityRepository activityRepository,
        IActivityClimbRepository activityClimbRepository,
        IClimbRepository climbRepository,
        IActivityReaderService activityReaderService,
        IRouteAnalyzerService routeAnalyzerService,
        ILogger<UploadActivityHandler> logger)
    {
        _fileStorage = fileStorage;
        _activityRepository = activityRepository;
        _activityClimbRepository = activityClimbRepository;
        _climbRepository = climbRepository;
        _activityReaderService = activityReaderService;
        _routeAnalyzer = routeAnalyzerService;
        _logger = logger;
    }

    public async Task<int> Handle(UploadActivityCommand request, CancellationToken cancellationToken)
    {      
        // Read file
        var readData = await _activityReaderService.ReadActivityAsync(request.FileStream);

        // Analyze file (intervals, climbs...)
        if (readData.Records == null || readData.Records.Count == 0)
            throw new NoDataException();

        var activityClimbs = await _routeAnalyzer.AnalyzeRouteAsync(readData.Records.ToList());
        // var debugTrace = _routeAnalyzer.GetDebugTrace();

        // Save non existing climbs
        _logger.LogInformation($"{activityClimbs.Count} climbs found:");
        foreach (var climb in activityClimbs)
        {
            if (climb.ClimbId == 0)     // Means it wasn't matched with an existing climb
            {
                var newClimb = await _climbRepository.AddAsync(climb.Data);
                newClimb.Name = "Unknown";
                climb.MergeWith(newClimb);
            }
            _logger.LogInformation($"{climb.Data.Metrics.DistanceMeters}m at {climb.Data.Metrics.Slope}%");
        }

        // Reset stream to read file
        request.FileStream.Position = 0;

        // Save file
        readData.Data.Path = await _fileStorage.SaveFileAsync(
            request.FileStream,
            request.FileName,
            "activities");

        // Save activity in DB
        int activityId = await _activityRepository.AddAsync(readData.Data, request.CyclistId);
        await _activityClimbRepository.AddAsync(activityClimbs, activityId);   

        return readData.Data.Id;
    }
}
