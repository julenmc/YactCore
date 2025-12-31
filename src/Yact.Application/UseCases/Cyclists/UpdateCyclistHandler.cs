using AutoMapper;
using MediatR;
using Yact.Application.Commands.Cyclists;
using Yact.Application.Responses;
using Yact.Domain.Entities.Cyclist;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Cyclists;

public class UpdateCyclistHandler : IRequestHandler<UpdateCyclistCommand, CyclistDto>
{
    private readonly ICyclistRepository _repository;
    private readonly IMapper _mapper;
    
    public UpdateCyclistHandler(
        ICyclistRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CyclistDto> Handle(UpdateCyclistCommand request, CancellationToken cancellationToken)
    {
        var cyclist = _mapper.Map<Cyclist>(request.Cyclist);
        var updated = await _repository.UpdateAsync(cyclist);
        return _mapper.Map<CyclistDto>(updated);
    }
}
