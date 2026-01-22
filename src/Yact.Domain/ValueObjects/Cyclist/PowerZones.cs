namespace Yact.Domain.ValueObjects.Cyclist;

public record PowerZones
{
    private const int ZoneCount = 7;
    public IDictionary<int, Zone> Values { get; private set; }

    private PowerZones(IDictionary<int, Zone> values)
    {
        Values = values;
    }

    /// <summary>
    /// Factory to create a PowerZones Value Object.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <remarks>
    /// Check that the input satifies all the requirements
    /// </remarks>
    public static PowerZones Create(IDictionary<int, Zone> values)
    {
        // Check count
        if (values.Count != ZoneCount)
            throw new ArgumentException($"The zone count must be {ZoneCount} instead of {values.Count}");

        // Check zones values
        try
        {
            Zone lastZone = values[1];
            for (int i = 2; i <= ZoneCount; i++)
            {
                if (values[i].LowLimit != lastZone.HighLimit + 1)
                    throw new ArgumentException($"Zone low limit must be 1 higher than the high limit of the previous zone. Doesn't apply in zone {i}");
                lastZone = values[i];
            }
        }
        catch (Exception ex) 
        {
            throw new ArgumentException(ex.Message);
        }

        return new PowerZones(values);
    }
}
