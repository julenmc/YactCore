using MediatR;
using Yact.Application.ReadModels.Cyclists;

namespace Yact.Application.UseCases.Cyclists.Queries;

public record GetCyclistsByLastNameQuery (string LastName) : IRequest<IEnumerable<CyclistBasicReadModel>>;
