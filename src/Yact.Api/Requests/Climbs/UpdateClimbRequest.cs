using MediatR;

namespace Yact.Api.Requests.Climbs;

public record UpdateClimbRequest(
    string Name) : IRequest<Guid>;
