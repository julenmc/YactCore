using System.Text.Json;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Infrastructure.Persistence.Mappers;

internal class ZonesMapper
{
    internal static Dictionary<int, Zone>? MapFromJson(string? jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return null;

        try
        {
            var zoneArray = JsonSerializer.Deserialize<Zone[]>(jsonString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (zoneArray == null)
                return null;

            var dic = new Dictionary<int, Zone>();
            for (int i = 0; i < zoneArray.Length; i++) 
            {
                dic.Add(i + 1, zoneArray[i]);
            }

            return dic;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    internal static string MapToJson(Dictionary<int, Zone>? zones)
    {
        if (zones == null)
            return "[]";

        Zone[] zoneArray = new Zone[zones.Count];
        foreach (KeyValuePair<int, Zone> pair in zones)
        {
            zoneArray[pair.Key - 1] = pair.Value;
        }

        return JsonSerializer.Serialize(zoneArray,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
    }
}
