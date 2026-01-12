using System.Text.Json;
using Yact.Domain.ValueObjects.Common;
using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Models;
using Entities = Yact.Domain.Entities;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class CyclistFitnessMapper
{
    internal static Entities.CyclistFitness ToDomain(this CyclistFitness model)
    {
        Dictionary<int, int>? powerCurve = null;
        Dictionary<int, Zone>? powerZones = null;
        Dictionary<int, Zone>? hrZones = null;

        if (model.PowerCurveJson != null)
        {
            powerCurve = JsonSerializer.Deserialize<Dictionary<int, int>>(model.PowerCurveJson)
            ?? new Dictionary<int, int>();
        }
        if (model.PowerZonesRaw != null)
        {
            powerZones = ZonesMapper.MapFromJson(model.PowerZonesRaw);
        }
        if (model.HrZonesRaw != null)
        {
            hrZones = ZonesMapper.MapFromJson(model.HrZonesRaw);
        }

        return Entities.CyclistFitness.Load(
            id: CyclistFitnessId.From(model.Id),
            cyclistId: CyclistId.From(model.CyclistId),
            updateTime: model.UpdateDate,
            size: new Size(model.Height, model.Weight),
            ftp: model.Ftp,
            vo2Max: model.Vo2Max,
            curve: powerCurve,
            powerZones: powerZones,
            hrZones: hrZones
        );
    }

    internal static CyclistFitness ToModel(this Entities.CyclistFitness entity)
    {
        if (entity.PowerCurve == null)
            throw new ArgumentNullException(nameof(entity.PowerCurve));
        if (entity.PowerZones == null)
            throw new ArgumentNullException(nameof(entity.PowerZones));
        if (entity.HrZones == null)
            throw new ArgumentNullException (nameof(entity.HrZones));

        return new CyclistFitness
        {
            Id = entity.Id.Value,
            CyclistId = entity.CyclistId.Value,
            UpdateDate = entity.UpdateDate,
            Height = entity.Size.HeightCm,
            Weight = entity.Size.WeightKg,
            Ftp = entity.Ftp,
            Vo2Max = entity.Vo2Max,
            PowerCurveJson = JsonSerializer.Serialize(entity.PowerCurve),
            PowerZonesRaw = ZonesMapper.MapToJson(entity.PowerZones),
            HrZonesRaw = ZonesMapper.MapToJson(entity.HrZones)
        };
    }
}
