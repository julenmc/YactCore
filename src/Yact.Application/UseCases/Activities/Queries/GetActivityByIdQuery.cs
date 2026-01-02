using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Activities.Queries;

public record GetActivityByIdQuery (int Id) : IRequest<ActivityDto>;
