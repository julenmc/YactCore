using MediatR;

namespace Yact.Application.UseCases.Activities.Commands;

public record UploadActivityCommand(
    Stream FileStream,
    string FileName,
    Guid CyclistId
) : IRequest<Guid>;
