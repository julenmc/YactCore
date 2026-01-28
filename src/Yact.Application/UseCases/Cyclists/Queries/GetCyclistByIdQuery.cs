using MediatR;
using Yact.Application.ReadModels.Cyclists;

namespace Yact.Application.UseCases.Cyclists.Queries;

public record GetCyclistByIdQuery(Guid Id) : IRequest<CyclistAdvancedReadModel>;
