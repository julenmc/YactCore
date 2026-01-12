using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Climbs.Queries;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Application.UseCases.Climbs;

public class GetClimbByIdHandler : IRequestHandler<GetClimbByIdQuery, ClimbResponse>
{
    private readonly IMapper _mapper;
    private readonly IClimbRepository _repository;

    public GetClimbByIdHandler(
        IMapper mapper, 
        IClimbRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<ClimbResponse> Handle (GetClimbByIdQuery query, CancellationToken cancellationToken)
    {
        var climb = await _repository.GetByIdAsync(ClimbId.From(query.Id));
        return _mapper.Map<ClimbResponse>(climb);
    }
}
