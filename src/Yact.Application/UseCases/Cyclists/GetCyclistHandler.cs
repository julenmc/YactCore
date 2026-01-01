using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Cyclists.Queries;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Cyclists;

public class GetCyclistHandler : IRequestHandler<GetCyclistsQuery, IEnumerable<CyclistDto>>
{
    private readonly ICyclistRepository _repository;
    private readonly IMapper _mapper;

    public GetCyclistHandler(
        ICyclistRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CyclistDto>> Handle (GetCyclistsQuery query, CancellationToken cancellationToken)
    {
        var cyclists = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<CyclistDto>>(cyclists);
    }
}
