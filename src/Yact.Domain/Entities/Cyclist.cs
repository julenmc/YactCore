using Yact.Domain.Events;
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
    public CyclistFitness? FitnessData { get; private set; }

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
        string name,
        string lastName,
        DateTime birthDate)
    {
        var cyclist = new Cyclist(
            CyclistId.NewId(),
            name,
            lastName,
            birthDate);
        // Add domain event

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
        cyclist.AddDomainEvent(new CyclistLoadedEvent(id));

        return cyclist;
    }

    public void AddFitness(CyclistFitness fitness)
    {
        if (fitness.CyclistId.Equals(Id) == false)
        {
            throw new ArgumentException("Fitness cyclist ID reference must equal the ID of the cyclist", nameof(fitness.CyclistId));
        }
        FitnessData = fitness;
    }

    public void UpdateFitness(CyclistFitness fitness)
    {
        //ushort? height = null,
        //float? weight = null,
        //ushort? ftp = null,
        //float? vo2Max = null,
        //PowerCurve? curve = null,
        //Dictionary<int, Zone>? hrZones = null,
        //Dictionary< int, Zone >? powerZones = null)
    }
}
