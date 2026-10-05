using Task.Api.Models;

namespace Task.Api.Contracts;

public sealed record TaskResponse(
    int Id,
    int ProjectId,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt);
    