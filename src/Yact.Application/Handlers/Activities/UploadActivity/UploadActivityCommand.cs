using MediatR;

namespace Yact.Application.Handlers.Activities.UploadActivity;

public record UploadActivityCommand(
    Stream FileStream,
    string FileName
) : IRequest<int>;
