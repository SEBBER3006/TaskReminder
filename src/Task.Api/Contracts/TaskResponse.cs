namespace Task.Api.Contracts;

public sealed record TaskResponse(
   int Id,
   string Title,
   string? Description,
   bool IsCompleted,
   DateTime CreatedAt,
   DateTime? CompletedAt);
   