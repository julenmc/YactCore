using MediatR;

namespace Yact.Application.UseCases.Activities.Commands;

public record UploadActivityCommand(
    Stream FileStream,
    string FileName,
    int CyclistId
) : IRequest<int>;
