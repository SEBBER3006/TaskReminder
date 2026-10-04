

namespace Task.Api.Models;

public class Project
{
    public int Id { get; set; }
    public int OwnerId  { get; set; } // Foreign key to the User (Owner)
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<TaskItem> Tasks { get; set; } = []; // Navigation property for related tasks
    public User Owner { get; set; } = null!; // Navigation property to the related User (Owner)

}
