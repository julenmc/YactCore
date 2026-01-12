namespace Yact.Application.Responses;

public record PowerCurveResponse(
    Dictionary<int, int> PowerBySeconds
);
