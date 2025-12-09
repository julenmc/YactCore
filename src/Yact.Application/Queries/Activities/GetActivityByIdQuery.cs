using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Activities;

public record GetActivityByIdQuery (int Id) : IRequest<ActivityInfoDto>;
