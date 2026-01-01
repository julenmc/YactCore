using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Climbs.Queries;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Climbs;

public class GetClimbsByCoordinatesHandler : IRequestHandler<GetClimbsByCoordinatesQuery, IEnumerable<ClimbDto>>
{
    private readonly IMapper _mapper;
    private readonly IClimbRepository _repository;

    public GetClimbsByCoordinatesHandler(
        IMapper mapper, 
        IClimbRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<IEnumerable<ClimbDto>> Handle (GetClimbsByCoordinatesQuery query, CancellationToken cancellationToken)
    {
        var climbList = await _repository.GetByCoordinatesAsync(query.LatitudeMin, query.LatitudeMax, query.LongitudeMin, query.LongitudeMax);
        return _mapper.Map<IEnumerable<ClimbDto>>(climbList);
    }
}
