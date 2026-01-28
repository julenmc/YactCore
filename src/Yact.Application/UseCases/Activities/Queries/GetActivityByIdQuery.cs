using MediatR;
using Yact.Application.ReadModels.Activities;

namespace Yact.Application.UseCases.Activities.Queries;

public record GetActivityByIdQuery (Guid Id) : IRequest<ActivityAdvancedReadModel>;
