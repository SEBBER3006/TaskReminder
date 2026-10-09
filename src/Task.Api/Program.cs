using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Scalar.AspNetCore;
using Task.Api.Data;
using Task.Api.Endpoints;
var builder = WebApplication.CreateBuilder(args);

//Add Health Checks
builder.Services.AddHealthChecks();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Validation services
builder.Services.AddValidation();

builder.Services.AddDbContext<TaskApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Configure JSON serialization to handle enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

app.UseHttpsRedirection();

// Add health check endpoint
app.MapHealthChecks("/health");
// Map the endpoints for tasks and projects
app.MapTaskEndpoints();
app.MapProjectEndpoints();


app.Run();


