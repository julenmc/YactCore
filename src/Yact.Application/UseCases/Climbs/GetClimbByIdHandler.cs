using AutoMapper;
using MediatR;
using Yact.Application.Interfaces.Queries;
using Yact.Application.ReadModels.Climbs;
using Yact.Application.UseCases.Climbs.Queries;

namespace Yact.Application.UseCases.Climbs;

public class GetClimbByIdHandler : IRequestHandler<GetClimbByIdQuery, ClimbAdvancedReadModel>
{
    private readonly IClimbQueries _queries;

    public GetClimbByIdHandler(
        IClimbQueries queries)
    {
        _queries = queries;
    }

    public async Task<ClimbAdvancedReadModel> Handle (GetClimbByIdQuery query, CancellationToken cancellationToken)
    {
        return await _queries.GetByIdAsync(query.Id);
    }
}
