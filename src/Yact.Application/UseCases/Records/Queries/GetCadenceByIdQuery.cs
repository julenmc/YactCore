using MediatR;
using Yact.Application.ReadModels.Activities;

namespace Yact.Application.UseCases.Records.Queries;

public record GetCadenceByIdQuery (Guid Id) : IRequest<TimeSeriesResponseDto<int?>>;
