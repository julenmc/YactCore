namespace Yact.Domain.Entities.Cyclist;

public class PowerCurve
{
    private readonly Dictionary<int, int> _powerBySeconds;

    public PowerCurve(Dictionary<int, int> powerBySeconds)
    {
        _powerBySeconds = powerBySeconds ?? throw new ArgumentNullException(nameof(powerBySeconds));
        ValidatePowerCurve();
    }

    public IReadOnlyDictionary<int, int> PowerBySeconds => _powerBySeconds;

    public int GetPowerAtDuration(TimeSpan duration)
    {
        var seconds = (int)duration.TotalSeconds;
        return _powerBySeconds.TryGetValue(seconds, out var power) ? power : 0;
    }

    private void ValidatePowerCurve()
    {
        if (_powerBySeconds.Count == 0)
            throw new InvalidOperationException("Power curve cannot be empty");

        if (_powerBySeconds.Values.Any(p => p < 0))
            throw new InvalidOperationException("Power values cannot be negative");
    }
}

