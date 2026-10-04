namespace Task.Api.Models;

public class TaskItem
{
    public int Id { get; set; }
    public int ProjectId { get; set; } // Foreign key to the Project
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public enum TaskItemStatus { Todo, InProgress, Done }
    public TaskItemStatus Status { get; set; } = 0; // Default status is Todo
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Updated timestamp
    public Project Project { get; set; } = null!; // Navigation property to the related Project


}