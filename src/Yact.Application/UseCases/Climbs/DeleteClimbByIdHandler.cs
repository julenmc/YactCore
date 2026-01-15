using MediatR;
using Yact.Application.UseCases.Climbs.Commands;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Application.UseCases.Climbs;

public class DeleteClimbByIdHandler : IRequestHandler<DeleteClimbByIdCommand, Guid>
{
    private readonly IClimbRepository _climbRepository;
    private readonly IActivityRepository _activityRepository;

    public DeleteClimbByIdHandler(
        IActivityRepository activityRepository,
        IClimbRepository climbRepository)
    {
        _activityRepository = activityRepository;
        _climbRepository = climbRepository;
    }

    public async Task<Guid> Handle (DeleteClimbByIdCommand command, CancellationToken cancellationToken)
    {
        var climbId = ClimbId.From(command.Id);

        // First, remove activity climbs
        var activities = await _activityRepository.GetContainingClimbAsync(climbId);
        foreach (var activity in activities)
        {
            activity.RemoveClimb(climbId);
            await _activityRepository.UpdateAsync(activity);    // Can I do it only once? Now it makes a transaction with every activity
        }

        // Then I can delete the climb
        var deleted = await _climbRepository.RemoveByIdAsync(climbId);
        return deleted != null ? deleted.Id.Value : Guid.Empty;
    }
}
