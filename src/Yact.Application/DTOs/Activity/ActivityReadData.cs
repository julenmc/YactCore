namespace Yact.Application.DTOs.Activity;

public record ActivityReadData(
    string Name,
    string Type,
    IReadOnlyList<RecordsRawData> Records
);
