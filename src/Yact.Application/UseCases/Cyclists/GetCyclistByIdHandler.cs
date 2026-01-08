using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Cyclists.Queries;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Cyclists;

public class GetCyclistByIdHandler : IRequestHandler<GetCyclistByIdQuery, CyclistDto>
{
    private readonly ICyclistRepository _repository;
    private readonly IMapper _mapper;

    public GetCyclistByIdHandler(
        ICyclistRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CyclistDto> Handle(GetCyclistByIdQuery query, CancellationToken cancellationToken)
    {
        var cyclist = await _repository.GetByIdAsync(CyclistId.From(query.Id));
        return _mapper.Map<CyclistDto>(cyclist);
    }
}
