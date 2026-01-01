using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Records.Queries;

public record GetPowerByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<double?>>;
