using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Cyclists;

public record GetCyclistByIdQuery(int Id) : IRequest<CyclistDto>;
