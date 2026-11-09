using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Middleware;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions.DependencyExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Registration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString")));

// MediatR Registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// FluentValidation Registration
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Validation Behavior Pipeline Registration
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

builder.Services.AddRepositories();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
    });
}

// Map endpoints dynamically (Vertical Slice IEndpoint auto-discovery)
var endpoints = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

foreach (var endpoint in endpoints)
{
    var instance = Activator.CreateInstance(endpoint) as IEndpoint;
    instance?.MapEndpoint(app);
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
