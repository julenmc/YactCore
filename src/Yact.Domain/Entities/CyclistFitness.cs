using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities;

public class CyclistFitness : Entity<CyclistFitnessId>
{
    public CyclistId CyclistId { get; private set; }
    public DateTime UpdateDate { get; private set; }
    public Size Size { get; private set; }
    public ushort Ftp { get; private set; }
    public float Vo2Max { get; private set; }
    public IDictionary<int, int>? PowerCurve { get; private set; }
    public IDictionary<int, Zone>? HrZones { get; private set; }
    public IDictionary<int, Zone>? PowerZones { get; private set; }

    /// <summary>
    /// Empty constructor
    /// </summary>
    private CyclistFitness() : base(default!)
    {
        
    }

    private CyclistFitness(
        CyclistFitnessId id,
        CyclistId cyclistId,
        Size size,
        ushort ftp,
        float vo2Max,
        IDictionary<int, int>? curve = null,
        IDictionary<int, Zone>? hrZones = null,
        IDictionary<int, Zone>? powerZones = null,
        DateTime? updateTime = null
        ) : base(id)
    {
        CyclistId = cyclistId;
        UpdateDate = updateTime ?? DateTime.Now;    // When loading the time is known, when creating no
        Size = size;
        Ftp = ftp;
        Vo2Max = vo2Max;
        PowerCurve = curve != null ? curve.ToDictionary() : null;
        HrZones = hrZones != null ? hrZones.ToDictionary() : null;
        PowerZones = powerZones != null ? powerZones.ToDictionary() : null;
    }

    /// <summary>
    /// Method to create a new CyclistFitness from scratch
    /// </summary>
    /// <param name="cyclistId">ID of the cyclist</param>
    /// <param name="size">Size of the cyclist (height + weight)</param>
    /// <param name="ftp">FTP in Watts</param>
    /// <param name="vo2Max">VO2Max</param>
    /// <param name="curve">Power curve with its points</param>
    /// <param name="hrZones">Heart Rate Zones</param>
    /// <param name="powerZones">Power Zones</param>
    /// <returns></returns>
    public static CyclistFitness Create(
        CyclistId cyclistId,
        Size size,
        ushort? ftp = null,
        float? vo2Max = null,
        IDictionary<int, int>? curve = null,
        IDictionary<int, Zone>? hrZones = null,
        IDictionary<int, Zone>? powerZones = null)
    {
        return new CyclistFitness(
            CyclistFitnessId.NewId(),
            cyclistId,
            size,
            ftp ?? 0,
            vo2Max ?? 0,
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
    /// <param name="size">Size of the cyclist (height + weight) (optional)</param>
    /// <param name="ftp">FTP in Watts (optional)</param>
    /// <param name="vo2Max">VO2Max (optional)</param>
    /// <param name="curve">Power curve with its points (optional)</param>
    /// <param name="hrZones">Heart Rate Zones (optional)</param>
    /// <param name="powerZones">Power Zones (optional)</param>
    /// <returns></returns>
    public static CyclistFitness CreateFromOld(
        CyclistFitness old,
        Size? size = null,
        ushort? ftp = null,
        float? vo2Max = null,
        IDictionary<int, int>? curve = null,
        IDictionary<int, Zone>? hrZones = null,
        IDictionary<int, Zone>? powerZones = null)
    {
        return new CyclistFitness(
            id: CyclistFitnessId.NewId(),
            cyclistId: old.CyclistId,
            size: size ?? old.Size,
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
        Size size,
        ushort ftp,
        float vo2Max,
        IDictionary<int, int>? curve,
        IDictionary<int, Zone>? hrZones = null,
        IDictionary<int, Zone>? powerZones = null)
    {
        return new CyclistFitness(
            id,
            cyclistId,
            size,
            ftp,
            vo2Max,
            curve,
            hrZones,
            powerZones,
            updateTime);
    }
}
