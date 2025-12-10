using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.Smoothers.Altitude;

/// <summary>
/// Suavizado con pesos: los puntos más cercanos tienen más influencia.
/// Usa una función Gaussiana para el peso.
/// </summary>
public class WeightedDistanceAltitudeSmoother : IAltitudeSmootherService
{
    private readonly double _windowDistanceMeters;
    private readonly double _sigma;

    /// <summary>
    /// Crea un suavizador ponderado por distancia.
    /// </summary>
    /// <param name="windowDistanceMeters">Distancia de la ventana (típicamente 30-60m)</param>
    /// <param name="sigma">Desviación estándar para la gaussiana. 
    /// Menor = más peso al centro. Por defecto windowDistance/3</param>
    public WeightedDistanceAltitudeSmoother(
        double windowDistanceMeters = 40,
        double? sigma = null)
    {
        _windowDistanceMeters = windowDistanceMeters;
        _sigma = sigma ?? windowDistanceMeters / 3.0;
    }

    public void Smooth(List<RecordData> records)
    {
        if (records.Count < 2)
            return;

        var originalAltitudes = records
            .Select(r => r.Coordinates?.Altitude)
            .ToList();

        var halfWindow = _windowDistanceMeters / 2;

        for (int i = 0; i < records.Count; i++)
        {
            if (originalAltitudes[i] == null ||
                records[i].DistanceMeters == null)
                continue;

            var centerDistance = records[i].DistanceMeters!.Value;
            var weightedSum = 0.0;
            var totalWeight = 0.0;

            for (int j = 0; j < records.Count; j++)
            {
                if (originalAltitudes[j] == null ||
                    records[j].DistanceMeters == null)
                    continue;

                var pointDistance = records[j].DistanceMeters!.Value;
                var distanceFromCenter = Math.Abs(pointDistance - centerDistance);

                if (distanceFromCenter <= halfWindow)
                {
                    // Gaussian weight e^(-(d²)/(2σ²))
                    var weight = Math.Exp(-(distanceFromCenter * distanceFromCenter) /
                                         (2 * _sigma * _sigma));

                    weightedSum += originalAltitudes[j]!.Value * weight;
                    totalWeight += weight;
                }
            }

            if (totalWeight > 0 && records[i].Coordinates != null)
            {
                records[i].Coordinates!.Altitude = weightedSum / totalWeight;
            }
        }
    }
}
