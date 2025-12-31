using AutoMapper;
using MediatR;
using Yact.Application.Commands.Activities;
using Yact.Application.Responses;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.Activities;

public class UpdateActivityHandler : IRequestHandler<UpdateActivityCommand, ActivityInfoDto>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public UpdateActivityHandler(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ActivityInfoDto> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = _mapper.Map<ActivitySummary>(command.ActivityDto) with { UpdateDate = DateTime.UtcNow };
        var updated = await _repository.UpdateAsync(activity);
        return _mapper.Map<ActivityInfoDto>(updated);
    }
}
