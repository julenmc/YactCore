using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Records.Queries;

public record GetHrByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<int?>>;
