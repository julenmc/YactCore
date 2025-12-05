using MediatR;
using Yact.Application.DTOs;

namespace Yact.Application.Handlers.Records.GetPowerById;

public record GetPowerByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<double?>>;
