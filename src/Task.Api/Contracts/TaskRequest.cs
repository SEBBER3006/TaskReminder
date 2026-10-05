using System.ComponentModel.DataAnnotations;
using Task.Api.Models;

namespace Task.Api.Contracts;



public sealed record TaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; init; } = "";

    [MaxLength(2000)]
    public string? Description { get; init; }

    [EnumDataType(typeof(TaskItemStatus))]
    public TaskItemStatus Status { get; init; } = TaskItemStatus.Todo;

    public DateTime? DueDate { get; init; }
}