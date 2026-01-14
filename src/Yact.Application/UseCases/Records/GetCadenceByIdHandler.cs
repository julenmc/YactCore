using MediatR;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Application.UseCases.Records.Queries;
using Yact.Domain.ValueObjects.Activity;
using Yact.Application.ReadModels.Activities;
using Yact.Application.Interfaces.Files;

namespace Yact.Application.UseCases.Records;

public class GetCadenceByIdHandler : IRequestHandler<GetCadenceByIdQuery, TimeSeriesResponseDto<int?>>
{
    private readonly IActivityRepository _repository;
    private readonly IActivityReaderService _activityReaderService;

    public GetCadenceByIdHandler(
        IActivityRepository repository, 
        IActivityReaderService activityReaderService)
    {
        _repository = repository;
        _activityReaderService = activityReaderService;
    }

    public async Task<TimeSeriesResponseDto<int?>> Handle(GetCadenceByIdQuery request, CancellationToken cancellationToken)
    {
        // Get activity
        var activity = await _repository.GetByIdAsync(ActivityId.From(request.Id));
        if (activity == null)
            throw new ArgumentException($"No activity found by ID {request.Id}");

        using var stream = File.Open(activity.Path.Value, FileMode.Open);
        var activityRead = await _activityReaderService.ReadFullActivityAsync(stream);
        if (activityRead.Records == null)
            throw new NoDataException($"Activity with ID {request.Id} has no records");

        return new TimeSeriesResponseDto<int?>       // TODO: mapper for this convertion
        {
            ActivityId = request.Id,
            Metric = "Cadence",
            Unit = "rpm",
            Records = activityRead.Records
                .Select(record => new TimeSeriesRecordDto<int?>
                {
                    Timestamp = record.DateTime,
                    Value = record.Performance.Cadence
                }).ToList()
        };
    }
}
