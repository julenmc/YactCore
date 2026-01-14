using MediatR;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Application.UseCases.Records.Queries;
using Yact.Domain.ValueObjects.Activity;
using Yact.Application.ReadModels.Activities;
using Yact.Application.Interfaces.Files;

namespace Yact.Application.UseCases.Records;

public class GetHrByIdHandler : IRequestHandler<GetHrByIdQuery, TimeSeriesResponseDto<int?>>
{
    private readonly IActivityRepository _repository;
    private readonly IActivityReaderService _activityReaderService;

    public GetHrByIdHandler(
        IActivityRepository repository, 
        IActivityReaderService activityReaderService)
    {
        _repository = repository;
        _activityReaderService = activityReaderService;
    }

    public async Task<TimeSeriesResponseDto<int?>> Handle(GetHrByIdQuery request, CancellationToken cancellationToken)
    {
        // Get activity
        var activityInfo = await _repository.GetByIdAsync(ActivityId.From(request.Id));
        if (activityInfo == null)
            throw new ArgumentException($"No activity found by ID {request.Id}");

        using var stream = File.Open(activityInfo.Path.Value, FileMode.Open);
        var activity = await _activityReaderService.ReadFullActivityAsync(stream);
        if (activity.Records.Count == 0)
            throw new NoDataException($"Activity with ID {request.Id} has no records");

        return new TimeSeriesResponseDto<int?>       // TODO: mapper for this convertion
        {
            ActivityId = request.Id,
            Metric = "Hear-rate",
            Unit = "bpm",
            Records = activity.Records
                .Select(record => new TimeSeriesRecordDto<int?>
                {
                    Timestamp = record.DateTime,
                    Value = record.Performance.HeartRate
                }).ToList()
        };
    }
}
