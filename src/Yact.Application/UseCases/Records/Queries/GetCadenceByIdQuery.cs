using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Records.Queries;

public record GetCadenceByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<int?>>;
