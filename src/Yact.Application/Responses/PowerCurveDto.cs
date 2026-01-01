namespace Yact.Application.Responses;

public record PowerCurveDto(
    Dictionary<int, int> PowerBySeconds
);
