using MediatR;
using Yact.Application.Interfaces.Repositories;
using Yact.Application.ReadModels.Activities;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Exceptions.Activity;

namespace Yact.Application.UseCases.Activities;

public class GetActivityById : IRequestHandler<GetActivityByIdQuery, ActivityAdvancedReadModel>
{
    private readonly IActivityQueries _queries;

    public GetActivityById(IActivityQueries queries)
    {
        _queries = queries;
    }

    public async Task<ActivityAdvancedReadModel> Handle(GetActivityByIdQuery request, CancellationToken cancellation)
    {
        var activity = await _queries.GetByIdAsync(request.Id);
        if (activity == null)
        {
            throw new ActivityNotFoundException(request.Id);
        }
        return activity;
    }
}
