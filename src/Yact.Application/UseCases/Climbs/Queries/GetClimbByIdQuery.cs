using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Climbs.Queries;

public record GetClimbByIdQuery(Guid Id) : IRequest<ClimbResponse>;