using Microsoft.EntityFrameworkCore;
using Task.Api.Contracts;
using Task.Api.Data;
using Task.Api.Models;

namespace Task.Api.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects/{projectId:int}/tasks")
                       .WithTags("Tasks");
        
        group.MapGet("/", GetAllTasks);
        group.MapGet("/{taskId:int}", GetTaskById);
        group.MapPost("/", CreateTask);
        group.MapPut("/{taskId:int}", UpdateTask);
        group.MapDelete("/{taskId:int}", DeleteTask);
        return app; 
    }

    private static async Task<IResult> GetAllTasks(int projectId, TaskApiDbContext db, CancellationToken ct)
    {
        if (!await ProjectExists(db, projectId, ct))
            return Results.NotFound();

        var tasks = await db.TaskItems   // Query the TaskItems table
            .AsNoTracking()                                 // Use AsNoTracking for read-only queries to improve performance
            .Where(t => t.ProjectId == projectId) // Filter tasks by projectId
            .OrderBy(t => t.Id)                   // Order tasks by Id for consistent results
            // Select only the necessary fields to create TaskResponse objects
            .Select(t => new TaskResponse( 
                t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.DueDate, t.CreatedAt, t.UpdatedAt))
            .ToListAsync(ct); // Execute the query asynchronously and return the results as a list

        return Results.Ok(tasks); // Return the list of tasks with an HTTP 200 OK response
    }

    private static async Task<IResult> GetTaskById(int projectId, int taskId, TaskApiDbContext db, CancellationToken ct)
    {
        if (!await ProjectExists(db, projectId, ct))
            return Results.NotFound();

        var task = await db.TaskItems
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.Id == taskId)
            .Select(t => new TaskResponse(
                t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.DueDate, t.CreatedAt, t.UpdatedAt))
            .FirstOrDefaultAsync(ct); // Retrieve the first matching task or null if not found

        return task is not null ? Results.Ok(task) : Results.NotFound();
    }

    private static async Task<IResult> CreateTask(int projectId, TaskRequest req, TaskApiDbContext db, CancellationToken ct)
    {
        if (!await ProjectExists(db, projectId, ct))
            return Results.NotFound();

        var task = new TaskItem
        {
            ProjectId = projectId,
            Title = req.Title,
            Description = req.Description,
            Status = req.Status,
            DueDate = req.DueDate
        };

        db.TaskItems.Add(task);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/projects/{projectId}/tasks/{task.Id}", ToResponse(task));
    }

    private static async Task<IResult> UpdateTask(int projectId, int taskId, TaskRequest req, TaskApiDbContext db, CancellationToken ct)
    {
        var task = await db.TaskItems.SingleOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId, ct);
        if (task is null)
            return Results.NotFound();

        task.Title = req.Title.Trim();
        task.Description = req.Description;
        task.Status = req.Status;
        task.DueDate = req.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return Results.Ok(ToResponse(task));
    }

    private static async Task<IResult> DeleteTask(int projectId, int taskId, TaskApiDbContext db, CancellationToken ct)
    {
        var task = await db.TaskItems.SingleOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId, ct);
        if (task is null)
            return Results.NotFound();

        db.TaskItems.Remove(task);
        await db.SaveChangesAsync(ct);

        return Results.NoContent();
    }

    // Helper method to delete a task
    private static Task<bool> ProjectExists(TaskApiDbContext db, int projectId, CancellationToken ct)
    {
        return db.Projects.AnyAsync(p => p.Id == projectId, ct);
    }

    // Helper method to convert TaskItem to TaskResponse
    private static TaskResponse ToResponse(TaskItem t) =>
        new(t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.DueDate, t.CreatedAt, t.UpdatedAt);
}
