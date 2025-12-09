using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Records;

public record GetHrByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<int?>>;
