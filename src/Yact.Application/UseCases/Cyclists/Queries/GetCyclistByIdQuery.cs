using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Queries;

public record GetCyclistByIdQuery(Guid Id) : IRequest<CyclistDto>;
