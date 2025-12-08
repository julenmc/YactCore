using System.Text.Json;
using Yact.Infrastructure.Persistence.Models.Cyclist;
using Entities = Yact.Domain.Entities.Cyclist;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class CyclistFitnessMapper
{
    internal static Entities.CyclistFitness ToDomain(this CyclistFitness model)
    {
        if (model.PowerCurveJson == null)
            throw new ArgumentNullException(nameof(model.PowerCurveJson));

        var powerData = JsonSerializer.Deserialize<Dictionary<int, int>>(model.PowerCurveJson)
            ?? new Dictionary<int, int>();

        var powerCurve = new Entities.PowerCurve(powerData);

        return new Entities.CyclistFitness()
        {
            UpdateDate = model.UpdateDate,
            Height = model.Height,
            Weight = model.Weight,
            Ftp = model.Ftp,
            Vo2Max = model.Vo2Max,
            PowerCurve = powerCurve,
        };
    }

    internal static CyclistFitness ToModel(this Entities.CyclistFitness entity, Entities.Cyclist cyclist)
    {
        if (entity.PowerCurve == null)
            throw new ArgumentNullException(nameof(entity.PowerCurve));

        return new CyclistFitness
        {
            Id = 0,     // rewritten in DB
            CyclistId = cyclist.Id,
            UpdateDate = entity.UpdateDate,
            Height = entity.Height,
            Weight = entity.Weight,
            Ftp = entity.Ftp,
            Vo2Max = entity.Vo2Max,
            PowerCurveJson = JsonSerializer.Serialize(entity.PowerCurve.PowerBySeconds)
        };
    }
}
