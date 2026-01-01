using MediatR;
using Yact.Application.Responses;
using Yact.Application.Interfaces;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Application.UseCases.Records.Queries;

namespace Yact.Application.UseCases.Records;

public class GetPowerByIdHandler : IRequestHandler<GetPowerByIdQuery, TimeSeriesResponseDto<double?>>
{
    private readonly IActivityRepository _repository;
    private readonly IActivityReaderService _activityReaderService;

    public GetPowerByIdHandler(
        IActivityRepository repository, 
        IActivityReaderService activityReaderService)
    {
        _repository = repository;
        _activityReaderService = activityReaderService;
    }

    public async Task<TimeSeriesResponseDto<double?>> Handle(GetPowerByIdQuery request, CancellationToken cancellationToken)
    {
        // Get activity
        var activityInfo = await _repository.GetByIdAsync(request.Id);
        if (activityInfo == null)
            throw new ArgumentException($"No activity found by ID {request.Id}");

        using var stream = File.Open(activityInfo.Path.Value, FileMode.Open);
        var activity = await _activityReaderService.ReadFullActivityAsync(stream);
        if (activity.Records.Count == 0)
            throw new NoDataException($"Activity with ID {request.Id} has no records");

        return new TimeSeriesResponseDto<double?>       // TODO: mapper for this convertion
        {
            ActivityId = request.Id,
            Metric = "Power",
            Unit = "W",
            Records = activity.Records
                .Select(record => new TimeSeriesRecordDto<double?>
                {
                    Timestamp = record.DateTime,
                    Value = record.Performance.Power
                }).ToList()
        };
    }
}
