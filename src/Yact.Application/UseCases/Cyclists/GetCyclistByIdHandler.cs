using MediatR;
using Yact.Application.Interfaces.Repositories;
using Yact.Application.ReadModels.Cyclists;
using Yact.Application.UseCases.Cyclists.Queries;

namespace Yact.Application.UseCases.Cyclists;

public class GetCyclistByIdHandler : IRequestHandler<GetCyclistByIdQuery, CyclistAdvancedReadModel>
{
    private readonly ICyclistQueries _queries;

    public GetCyclistByIdHandler(
        ICyclistQueries queries)
    {
        _queries = queries;
    }

    public async Task<CyclistAdvancedReadModel> Handle(GetCyclistByIdQuery query, CancellationToken cancellationToken)
    {
        return await _queries.GetByIdAsync(query.Id);
    }
}
