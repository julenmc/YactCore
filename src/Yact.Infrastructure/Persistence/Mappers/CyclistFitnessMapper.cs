using System.Text.Json;
using Yact.Application.ReadModels.Common;
using Yact.Infrastructure.Persistence.ReadModels;
using Models = Yact.Application.ReadModels.Cyclists;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class CyclistFitnessMapper
{
    internal static Models.CyclistFitnessReadModel ToModel(this CyclistFitnessReadModel fitness)
    {
        return new Models.CyclistFitnessReadModel()
        {
            Id = fitness.Id,
            UpdateDate = fitness.UpdateDate,
            Height = fitness.Height,
            Weight = fitness.Weight,
            Ftp = fitness.Ftp,
            Vo2Max = fitness.Vo2Max,
            PowerCurve = fitness.PowerCurveJson != null ?
                JsonSerializer.Deserialize<Dictionary<int, int>>(fitness.PowerCurveJson, (JsonSerializerOptions?)null)!
                : null,
            HrZones = fitness.HrZonesRaw != null ?
                JsonSerializer.Deserialize<Dictionary<string, ZoneReadModel>>(fitness.HrZonesRaw, (JsonSerializerOptions?)null)!
                : null,
            PowerZones = fitness.PowerZonesRaw != null ?
                JsonSerializer.Deserialize<Dictionary<string, ZoneReadModel>>(fitness.PowerZonesRaw, (JsonSerializerOptions?)null)!
                : null,
        };
    }
}
