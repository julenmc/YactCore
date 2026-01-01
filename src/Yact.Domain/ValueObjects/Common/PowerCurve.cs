namespace Yact.Domain.ValueObjects.Common;

public record PowerCurve(
    IReadOnlyDictionary<int, int> PowerBySeconds
);

