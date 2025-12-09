using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Records;

public record GetCadenceByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<int?>>;
