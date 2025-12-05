using MediatR;
using Yact.Application.DTOs;

namespace Yact.Application.Handlers.Records.GetPowerById;

public record GetHrByIdQuery (int Id) : IRequest<TimeSeriesResponseDto<int?>>;
