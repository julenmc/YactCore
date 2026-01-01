using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Climbs.Queries;

public record GetClimbsByCoordinatesQuery(
    float LatitudeMin,
    float LatitudeMax,
    float LongitudeMin,
    float LongitudeMax) : IRequest<IEnumerable<ClimbDto>>;