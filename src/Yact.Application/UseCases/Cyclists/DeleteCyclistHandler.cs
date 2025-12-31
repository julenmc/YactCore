using AutoMapper;
using MediatR;
using Yact.Application.Commands.Cyclists;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Cyclists;

public class DeleteCyclistHandler : IRequestHandler<DeleteCyclistCommand, int>
{
    private readonly ICyclistRepository _repository;
    private readonly IMapper _mapper;

    public DeleteCyclistHandler(
        ICyclistRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> Handle(DeleteCyclistCommand request, CancellationToken cancelationToken)
    {
        var cyclist = await _repository.RemoveByIdAsync(request.Id);
        return cyclist.Id;
    }
}
