using MediatR;
using Yact.Application.ReadModels.Activities;

namespace Yact.Application.UseCases.Records.Queries;

public record GetPowerByIdQuery (Guid Id) : IRequest<TimeSeriesResponseDto<double?>>;
