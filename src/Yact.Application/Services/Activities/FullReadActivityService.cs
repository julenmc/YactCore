using Yact.Application.Interfaces;
using Yact.Domain.Entities;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;
using Yact.Domain.Services.Utils.Smoothers.Altitude;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Application.Services.Activities;

public class FullReadActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityReaderService _activityReaderService;

    public FullReadActivityService(
        IActivityRepository activityRepository,
        IActivityReaderService activityReaderService)
    {
        _activityRepository = activityRepository;
        _activityReaderService = activityReaderService;
    }

    public async Task<Activity> Execute(ActivityId activityId)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null)
        {
            throw new ActivityNotFoundException(activityId.Value);
        }
        var readData = await _activityReaderService.ReadFullActivityAsync(File.OpenRead(activity.Path.Value));
        if (readData.Records == null || readData.Records.Count == 0)
        {
            throw new NoDataException();
        }

        // First distance calculator, the smoother needs the distances
        var coordinates = readData.Records
            .Select(r => r.Coordinates)
            .ToList();
        HarversineDistanceCalculatorService distanceCalculator = new HarversineDistanceCalculatorService();
        WeightedDistanceAltitudeSmoother smoother = new WeightedDistanceAltitudeSmoother();
        var distances = distanceCalculator.CalculateDistances(coordinates);
        var smoothed = smoother.Smooth(coordinates, distances);

        // Put records in the entity
        var records = Enumerable
            .Range(0, readData.Records.Count)
            .Select(i => new RecordData{
                Timestamp = readData.Records[i].DateTime,
                Coordinates = readData.Records[i].Coordinates,
                Performance = readData.Records[i].Performance,
                DistanceMeters = distances[i],
                SmoothedAltitude = smoothed[i]
            });
        activity.AddRecords(new ActivityRecords(records.ToList()));
        if (activity.Records == null || activity.Records.Values.Count == 0)
        {
            throw new NoDataException();
        }

        // Update summary with analyzed data
        activity.UpdateSummary(ActivitySummary.CopyWithRecords(activity.Summary, activity.Records));

        // Update activity in repository
        return await _activityRepository.UpdateAsync(activity) ?? throw new ActivityNotFoundException(activityId.Value); ;
    }
}
