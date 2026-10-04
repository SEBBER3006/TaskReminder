namespace Task.Api.Contracts;
public sealed record ProjectResponse(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int TaskCount);