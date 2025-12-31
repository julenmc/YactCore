using AutoMapper;
using MediatR;
using Yact.Application.Queries.Climbs;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Climbs;

public class GetClimbsHandler : IRequestHandler<GetClimbsQuery, IEnumerable<ClimbDto>>
{
    private readonly IMapper _mapper;
    private readonly IClimbRepository _repository;

    public GetClimbsHandler(
        IMapper mapper,
        IClimbRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<IEnumerable<ClimbDto>> Handle (GetClimbsQuery query, CancellationToken cancellationToken)
    {
        var climbs = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ClimbDto>>(climbs);
    }
}
