using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Records;

public record GetPowerByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<double?>>;
