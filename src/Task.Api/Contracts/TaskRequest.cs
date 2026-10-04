using System.ComponentModel.DataAnnotations;

namespace Task.Api.Contracts;

public sealed record TaskRequest
{
    [Required, MaxLength(200)] public string Title { get; init; } = "";
    [MaxLength(2000)] public string? Description { get; init; }
}