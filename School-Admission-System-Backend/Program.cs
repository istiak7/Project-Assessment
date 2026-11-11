using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Features.Holds.BackgroundServices;
using School_Admission_System_Backend.Middleware;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions.DependencyExtensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbConnectionString =
    builder.Configuration["DB_CONNECTION_STRING"]
    ?? builder.Configuration.GetConnectionString("DbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dbConnectionString));

// MediatR Registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// FluentValidation Registration
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Validation Behavior Pipeline Registration
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

builder.Services.AddRepositories();

var holdDurationSeconds =
    int.TryParse(builder.Configuration["HOLD_DURATION_SECONDS"], out var seconds) && seconds > 0
        ? seconds
        : 60;

builder.Services.AddSingleton(new HoldSettings { DurationSeconds = holdDurationSeconds });

builder.Services.AddHostedService<HoldExpiryService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Lightweight liveness endpoint used by Docker/Compose health checks.
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

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
