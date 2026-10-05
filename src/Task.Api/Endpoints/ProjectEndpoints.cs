using Microsoft.EntityFrameworkCore;
using Task.Api.Contracts;
using Task.Api.Data;
using Task.Api.Models;

namespace Task.Api.Endpoints;

public static class ProjectEndpoints
{
    private const int TemporaryOwnerId = 1;

    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects");

        group.MapGet("/", GetAllProjects);
        group.MapGet("/{id:int}", GetProjectById);
        group.MapPost("/", CreateProject);
        group.MapPut("/{id:int}", UpdateProject);
        group.MapDelete("/{id:int}", DeleteProject);

        return app;

    }

    private static async Task<IResult> GetAllProjects(TaskApiDbContext db, CancellationToken ct)
    {
        var projects = await db.Projects
            .Select(p => new ProjectResponse(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                p.Tasks.Count))
            .ToListAsync(ct);

        return Results.Ok(projects);
    }

    private static async Task<IResult> GetProjectById(int id, TaskApiDbContext db, CancellationToken ct)
    {
        var project = await db.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectResponse(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                p.Tasks.Count))
            .FirstOrDefaultAsync(ct);

        return project is not null ? Results.Ok(project) : Results.NotFound();
    }

    private static async Task<IResult> CreateProject(ProjectRequest req, TaskApiDbContext db, CancellationToken ct)
    {
        var project = new Project
        {
            OwnerId = TemporaryOwnerId,
            Name = req.Name.Trim(),
            Description = req.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };


        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);

        var response = new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt, 0);
        return Results.Created($"/api/projects/{project.Id}", response);
    }

    private static async Task<IResult> UpdateProject(int id, ProjectRequest req, TaskApiDbContext db, CancellationToken ct)
    {
        var project = await db.Projects.FindAsync([id], ct); 
        if (project is null)
        {
            return Results.NotFound();
        }

        project.Name = req.Name.Trim(); // Update the project name
        project.Description = req.Description ?? string.Empty; // Update the project description
        await db.SaveChangesAsync(ct); // Save changes to the database

        var taskCount = await db.TaskItems.CountAsync(t => t.ProjectId == id, ct);
        return Results.Ok(new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt, taskCount));
    }

    private static async Task<IResult> DeleteProject(int id, TaskApiDbContext db, CancellationToken ct)
    {
        var project = await db.Projects.FindAsync([id], ct);
        if (project is null)
        {
            return Results.NotFound();
        }

        db.Projects.Remove(project);
        await db.SaveChangesAsync(ct);

        return Results.NoContent(); // Return 204 No Content to indicate successful deletion
    }
}