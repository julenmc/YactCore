using MediatR;
using Yact.Application.ReadModels.Climbs;

namespace Yact.Application.UseCases.Climbs.Queries;

public record GetClimbByIdQuery(Guid Id) : IRequest<ClimbAdvancedReadModel>;