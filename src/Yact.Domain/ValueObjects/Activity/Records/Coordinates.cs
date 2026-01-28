namespace Yact.Domain.ValueObjects.Activity.Records;

public record Coordinates
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double Altitude { get; init; }
}
