using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Cyclists.Queries;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Cyclists;

public class GetCyclistsByLastName : IRequestHandler<GetCyclistsByLastNameQuery, IEnumerable<CyclistResponse>>
{
    private readonly ICyclistRepository _repository;
    private readonly IMapper _mapper;

    public GetCyclistsByLastName(
        ICyclistRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CyclistResponse>> Handle (GetCyclistsByLastNameQuery request, CancellationToken cancellationToken)
    {
        var list = await _repository.GetByLastName(request.LastName);
        return _mapper.Map<IEnumerable<CyclistResponse>>(list);
    }
}
