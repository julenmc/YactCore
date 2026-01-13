namespace Yact.Api.Requests.Activities;

public record UpdateActivityRequest (
    string? Name,
    string? Description);
