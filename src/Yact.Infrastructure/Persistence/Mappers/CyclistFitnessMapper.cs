using System.Text.Json;
using Yact.Domain.ValueObjects.Common;
using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Models.Cyclist;
using Entities = Yact.Domain.Entities.Cyclist;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class CyclistFitnessMapper
{
    internal static Entities.CyclistFitness ToDomain(this CyclistFitness model)
    {
        PowerCurve? powerCurve = null;
        Dictionary<int, Zone>? powerZones = null;
        Dictionary<int, Zone>? hrZones = null;

        if (model.PowerCurveJson != null)
        {
            var powerData = JsonSerializer.Deserialize<Dictionary<int, int>>(model.PowerCurveJson)
            ?? new Dictionary<int, int>();

            powerCurve = new PowerCurve
            (
                PowerBySeconds: powerData
            );
        }
        if (model.PowerZonesRaw != null)
        {
            powerZones = ZonesMapper.MapFromJson(model.PowerZonesRaw);
        }
        if (model.HrZonesRaw != null)
        {
            hrZones = ZonesMapper.MapFromJson(model.HrZonesRaw);
        }

        return new Entities.CyclistFitness()
        {
            Id = model.Id,
            CyclistId = model.CyclistId,
            UpdateDate = model.UpdateDate,
            Height = model.Height,
            Weight = model.Weight,
            Ftp = model.Ftp,
            Vo2Max = model.Vo2Max,
            PowerCurve = powerCurve,
            PowerZones = powerZones,
            HrZones = hrZones,
        };
    }

    internal static CyclistFitness ToModel(this Entities.CyclistFitness entity, int? cyclistID = null)
    {
        if (entity.PowerCurve == null)
            throw new ArgumentNullException(nameof(entity.PowerCurve));
        if (entity.PowerZones == null)
            throw new ArgumentNullException(nameof(entity.PowerZones));
        if (entity.HrZones == null)
            throw new ArgumentNullException (nameof(entity.HrZones));

        return new CyclistFitness
        {
            Id = 0,     // rewritten in DB
            CyclistId = cyclistID != null ? (int)cyclistID : entity.CyclistId,
            UpdateDate = entity.UpdateDate,
            Height = entity.Height,
            Weight = entity.Weight,
            Ftp = entity.Ftp,
            Vo2Max = entity.Vo2Max,
            PowerCurveJson = JsonSerializer.Serialize(entity.PowerCurve.PowerBySeconds),
            PowerZonesRaw = ZonesMapper.MapToJson(entity.PowerZones),
            HrZonesRaw = ZonesMapper.MapToJson(entity.HrZones)
        };
    }
}
