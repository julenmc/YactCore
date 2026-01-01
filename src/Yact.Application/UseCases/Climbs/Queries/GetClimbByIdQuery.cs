using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Climbs.Queries;

public record GetClimbByIdQuery(int Id) : IRequest<ClimbDto>;