using MediatR;
using Yact.Application.ReadModels.Activities;

namespace Yact.Application.UseCases.Records.Queries;

public record GetHrByIdQuery (Guid Id) : IRequest<TimeSeriesResponseDto<int?>>;
