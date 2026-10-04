using System.ComponentModel.DataAnnotations;

namespace Task.Api.Contracts;

public sealed record ProjectRequest
{
    [Required, MaxLength(100)] public string Name { get; init; } = "";
    [MaxLength(1000)] public string? Description { get; init; }

}