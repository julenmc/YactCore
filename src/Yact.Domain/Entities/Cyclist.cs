using Yact.Domain.Exceptions.Cyclist;
using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities;

public class Cyclist : AggregateRoot<CyclistId>
{
    public string Name { get; init; }
    public string LastName { get; init; }
    public string FullName => $"{Name} {LastName}";
    public DateTime BirthDate { get; init; }
    public int Age => DateTime.UtcNow.Year - BirthDate.Year - (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
    public ICollection<CyclistFitness> FitnessHistory => _fitnessHistory;
    public CyclistFitness LatestFitness => _fitnessHistory
            .OrderByDescending(f => f.UpdateDate)
            .First();

    private readonly List<CyclistFitness> _fitnessHistory = new();

    private Cyclist(
        CyclistId id,
        string name,
        string lastName,
        DateTime birthDate) : base(id)
    {
        Name = name;
        LastName = lastName;
        BirthDate = birthDate;
    }

    public static Cyclist Create(
        CyclistId id,
        string name,
        string lastName,
        DateTime birthDate,
        Size size,
        ushort? ftp = null,
        float? vo2Max = null,
        IDictionary<int, int>? powerCurve = null,
        IDictionary<int, Zone>? hrZones = null,
        IDictionary<int, Zone>? powerZones = null)
    {
        var cyclist = new Cyclist(
            id,
            name,
            lastName,
            birthDate);

        // After first creation, a minimum fitness must be created
        cyclist.CreateFitness(
            size,
            ftp,
            vo2Max,
            powerCurve,
            hrZones,
            powerZones);

        return cyclist;
    }

    public static Cyclist Load(
        CyclistId id,
        string name,
        string lastName,
        DateTime birthDate)
    {
        var cyclist = new Cyclist(
            id,
            name,
            lastName,
            birthDate);

        return cyclist;
    }

    private CyclistFitnessId CreateFitness(
        Size size,
        ushort? ftp = null,
        float? vo2Max = null,
        IDictionary<int, int>? powerCurve = null,
        IDictionary<int, Zone>? hrZones = null,
        IDictionary<int, Zone>? powerZones = null)
    {
        var fitness = CyclistFitness.Create(
            this.Id,
            size,
            ftp,
            vo2Max,
            powerCurve,
            hrZones,
            powerZones);

        _fitnessHistory.Add(fitness);
        return fitness.Id;
    }

    public CyclistFitnessId UpdateFitness(
        Size? size,
        ushort? ftp = null,
        float? vo2Max = null,
        IDictionary<int, int>? powerCurve = null,
        IDictionary<int, Zone>? hrZones = null,
        IDictionary<int, Zone>? powerZones = null)
    {
        var fitness = CyclistFitness.CreateFromOld(
            LatestFitness,
            size,
            ftp,
            vo2Max,
            powerCurve,
            hrZones,
            powerZones);

        _fitnessHistory.Add(fitness);
        return fitness.Id;
    }

    public void RemoveFitness(CyclistFitnessId fitnessId)
    {
        var fitness = _fitnessHistory.FirstOrDefault(f => f.Id == fitnessId);
        if (fitness == null)
            throw new FitnessNotFoundException(fitnessId.Value);

        _fitnessHistory.Remove(fitness);
    }
}
