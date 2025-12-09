using MediatR;
using Yact.Application.Responses;
using Yact.Application.Interfaces;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Application.Queries.Records;

namespace Yact.Application.Handlers.Records;

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
        var activityInfo = await _repository.GetByIdAsync(request.Id);
        if (activityInfo == null)
            throw new ArgumentException($"No activity found by ID {request.Id}");

        using var stream = File.Open(activityInfo.Path, FileMode.Open);
        var activity = await _activityReaderService.ReadActivityAsync(stream);
        if (activity.RecordData == null)
            throw new NoDataException($"Activity with ID {request.Id} has no records");

        return new TimeSeriesResponseDto<int?>       // TODO: mapper for this convertion
        {
            ActivityId = request.Id,
            Metric = "Hear-rate",
            Unit = "bpm",
            Records = activity.RecordData
                .Select(record => new TimeSeriesRecordDto<int?>
                {
                    Timestamp = record.Timestamp,
                    Value = record.HeartRate
                }).ToList()
        };
    }
}
