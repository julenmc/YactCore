using MediatR;

namespace Yact.Application.Commands.Activities;

public record UploadActivityCommand(
    Stream FileStream,
    string FileName,
    int CyclistId
) : IRequest<int>;
