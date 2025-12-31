using AutoMapper;
using MediatR;
using Yact.Application.Queries.Climbs;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Climbs;

public class GetClimbByIdHandler : IRequestHandler<GetClimbByIdQuery, ClimbDto>
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

    public async Task<ClimbDto> Handle (GetClimbByIdQuery query, CancellationToken cancellationToken)
    {
        var climb = await _repository.GetByIdAsync(query.Id);
        return _mapper.Map<ClimbDto>(climb);
    }
}
