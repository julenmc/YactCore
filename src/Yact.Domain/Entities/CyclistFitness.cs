using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Common;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities;

public class CyclistFitness : Entity<CyclistFitnessId>
{
    public CyclistId CyclistId { get; set; }
    public DateTime UpdateDate { get; set; }
    public ushort Height { get; set; }
    public float Weight { get; set; }
    public ushort Ftp { get; set; }
    public float Vo2Max { get; set; }
    public PowerCurve? PowerCurve { get; set; }
    public Dictionary<int, Zone>? HrZones { get; set; }
    public Dictionary<int, Zone>? PowerZones { get; set; }

    private CyclistFitness(
        CyclistFitnessId id,
        CyclistId cyclistId,
        ushort height,
        float weight,
        ushort ftp,
        float vo2Max,
        PowerCurve? curve,
        Dictionary<int, Zone>? hrZones = null,
        Dictionary<int, Zone>? powerZones = null,
        DateTime? updateTime = null
        ) : base(id)
    {
        CyclistId = cyclistId;
        UpdateDate = updateTime ?? DateTime.Now;    // When loading the time is known, when creating no
        Height = height;
        Weight = weight;
        Ftp = ftp;
        Vo2Max = vo2Max;
        PowerCurve = curve;
        HrZones = hrZones;
        PowerZones = powerZones;
    }

    /// <summary>
    /// Method to create a new CyclistFitness from scratch
    /// </summary>
    /// <param name="cyclistId">ID of the cyclist</param>
    /// <param name="height">Height in cm</param>
    /// <param name="weight">Weight in kg</param>
    /// <param name="ftp">FTP in Watts</param>
    /// <param name="vo2Max">VO2Max</param>
    /// <param name="curve">Power curve with its points</param>
    /// <param name="hrZones">Heart Rate Zones</param>
    /// <param name="powerZones">Power Zones</param>
    /// <returns></returns>
    public static CyclistFitness Create(
        CyclistId cyclistId,
        ushort height,
        float weight,
        ushort ftp,
        float vo2Max,
        PowerCurve curve,
        Dictionary<int, Zone>? hrZones = null,
        Dictionary<int, Zone>? powerZones = null)
    {
        return new CyclistFitness(
            CyclistFitnessId.NewId(),
            cyclistId,
            height,
            weight,
            ftp,
            vo2Max,
            curve,
            hrZones,
            powerZones);
    }

    /// <summary>
    /// Method to create a new CyclistFitness with an old one.
    /// Only the introduced parameters will be modified. When the parameter
    /// is null, the new entity will be created with the old entities parameter.
    /// </summary>
    /// <param name="old">The old CyclistFitness entity</param>
    /// <param name="height">Height in cm (optional)</param>
    /// <param name="weight">Weight in kg (optional)</param>
    /// <param name="ftp">FTP in Watts (optional)</param>
    /// <param name="vo2Max">VO2Max (optional)</param>
    /// <param name="curve">Power curve with its points (optional)</param>
    /// <param name="hrZones">Heart Rate Zones (optional)</param>
    /// <param name="powerZones">Power Zones (optional)</param>
    /// <returns></returns>
    public static CyclistFitness CreateFromOld(
        CyclistFitness old,
        ushort? height = null,
        float? weight = null,
        ushort? ftp = null,
        float? vo2Max = null,
        PowerCurve? curve = null,
        Dictionary<int, Zone>? hrZones = null,
        Dictionary<int, Zone>? powerZones = null)
    {
        return new CyclistFitness(
            id: CyclistFitnessId.NewId(),
            cyclistId: old.CyclistId,
            height: height ?? old.Height,
            weight: weight ?? old.Weight,
            ftp: ftp ?? old.Ftp,
            vo2Max: vo2Max ?? old.Vo2Max,
            curve: curve ?? old.PowerCurve,
            hrZones: hrZones ?? old.HrZones,
            powerZones: powerZones ?? old.PowerZones);
    }

    public static CyclistFitness Load(
        CyclistFitnessId id,
        CyclistId cyclistId,
        DateTime updateTime,
        ushort height,
        float weight,
        ushort ftp,
        float vo2Max,
        PowerCurve? curve,
        Dictionary<int, Zone>? hrZones = null,
        Dictionary<int, Zone>? powerZones = null)
    {
        return new CyclistFitness(
            id,
            cyclistId,
            height,
            weight,
            ftp,
            vo2Max,
            curve,
            hrZones,
            powerZones,
            updateTime);
    }
}
