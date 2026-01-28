using MediatR;
using Yact.Application.Interfaces.Repositories;
using Yact.Application.ReadModels.Cyclists;
using Yact.Application.UseCases.Cyclists.Queries;

namespace Yact.Application.UseCases.Cyclists;

public class GetCyclistsByLastName : IRequestHandler<GetCyclistsByLastNameQuery, IEnumerable<CyclistBasicReadModel>>
{
    private readonly ICyclistQueries _queries;

    public GetCyclistsByLastName(
        ICyclistQueries queries)
    {
        _queries = queries;
    }

    public async Task<IEnumerable<CyclistBasicReadModel>> Handle (GetCyclistsByLastNameQuery request, CancellationToken cancellationToken)
    {
        return await _queries.GetByLastNameAsync(request.LastName);
    }
}
