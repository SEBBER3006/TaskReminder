using Microsoft.EntityFrameworkCore;
using Task.Api.Data;

var builder = WebApplication.CreateBuilder(args);

//Add Health Checks
builder.Services.AddHealthChecks();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TaskApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add health check endpoint
app.MapHealthChecks("/health");


app.Run();


