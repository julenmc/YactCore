using Yact.Application.Responses;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.Mapping;

internal static class ActivityMapper
{
    internal static Activity ToDomain(this ActivityDto model)
    {
        return Activity.Load(
            ActivityId.From(model.Id),
            CyclistId.From(model.CyclistId),
            FilePath.From(model.Path),
            ActivitySummary.Create(
                name: model.Name,
                description: model.Description ?? "",
                startDate: model.StartDate,
                endDate: model.EndDate,
                distance: model.DistanceMeters,
                elevation: model.ElevationMeters,
                type: model.Type ?? "Unknown")
            );
    }

    internal static ActivityDto ToModel(this Activity entity)
    {
        return new ActivityDto
        {
            Id = entity.Id.Value,
            CyclistId = entity.CyclistId.Value,
            Name = entity.Summary.Name,
            Description = entity.Summary.Description,
            Path = entity.Path.Value,
            StartDate = entity.Summary.StartDate,
            EndDate = entity.Summary.EndDate,
            DistanceMeters = entity.Summary.DistanceMeters,
            ElevationMeters = entity.Summary.ElevationMeters,
            Type = entity.Summary.Type
        };
    }
}
