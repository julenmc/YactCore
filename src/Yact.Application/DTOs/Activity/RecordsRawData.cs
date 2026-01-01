using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Application.DTOs.Activity;

public record RecordsRawData(
    DateTime DateTime,
    Coordinates Coordinates,
    Performance Performance
);
