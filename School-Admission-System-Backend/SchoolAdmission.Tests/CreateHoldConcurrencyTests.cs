using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities;
using School_Admission_System_Backend.Entities.Common;
using School_Admission_System_Backend.Features.Holds.Command.CreateHold;
using School_Admission_System_Backend.Shared;

namespace SchoolAdmission.Tests;

public sealed class CreateHoldConcurrencyTests : IAsyncLifetime
{
    private const string DefaultConnectionString =
        "User ID=postgres;Password=istiakahmed;Host=localhost;Port=5432;Database=schooladmission_db;" +
        "Minimum Pool Size=4;Maximum Pool Size=50;Timeout=60;Command Timeout=60;Keepalive=10;";

    private static readonly HoldSettings LongHold = new() { DurationSeconds = 600 };
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly List<int> _createdClassIds = new();

    public CreateHoldConcurrencyTests()
    {
        var connectionString = Environment.GetEnvironmentVariable("SCHOOLADMISSION_TEST_DB") ?? DefaultConnectionString;
        _options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connectionString).Options;
    }

    public async Task InitializeAsync()
    {
        await using var db = NewContext();
        if (!await db.Database.CanConnectAsync())
            throw new InvalidOperationException("PostgreSQL instance is required for concurrency tests.");

        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await using var db = NewContext();
        foreach (var classId in _createdClassIds)
        {
            var cls = await db.Classes.FindAsync(classId);
            if (cls is not null) db.Classes.Remove(cls);
        }
        await db.SaveChangesAsync();
    }

    [Theory]
    [InlineData(15, 100)]
    [InlineData(3, 120)]
    [InlineData(5, 5)]
    public async Task CreateHold_NeverOversells_WhenStudentsRaceForLastSeats(int totalSeats, int racers)
    {

        var classId = await SeedClassAsync(totalSeats);
        var studentIds = Enumerable.Range(0, racers).Select(i => (10_000_000_000_000_000L + i).ToString()).ToArray();

        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        async Task<Result> RaceAsync(string studentId)
        {
            await gate.Task;
            await using var db = NewContext();
            return await new CreateHoldHandler(db, LongHold)
                .Handle(new CreateHoldCommand(classId, studentId), CancellationToken.None);
        }

        var tasks = studentIds.Select(RaceAsync).ToArray();


        gate.SetResult();
        var results = await Task.WhenAll(tasks);

        // Assert
        var expectedGranted = Math.Min(totalSeats, racers);
        var granted = results.Count(r => r.StatusCode == 201);
        var rejected = results.Count(r => r.StatusCode == 409 && r.Message.Contains("No seats available", StringComparison.OrdinalIgnoreCase));

        Assert.Equal(expectedGranted, granted);
        Assert.Equal(racers - expectedGranted, rejected);
        Assert.Equal(racers, granted + rejected);


        await using var verify = NewContext();
        var persistedHeld = await verify.Holds.CountAsync(h => h.AdmissionClassId == classId && h.Status == HoldStatus.Held);
        Assert.Equal(expectedGranted, persistedHeld);
    }

    private AppDbContext NewContext() => new(_options);

    private async Task<int> SeedClassAsync(int totalSeats)
    {
        await using var db = NewContext();
        var cls = new AdmissionClass { Name = $"Test Class {Guid.NewGuid():N}", TotalSeats = totalSeats };
        db.Classes.Add(cls);
        await db.SaveChangesAsync();
        _createdClassIds.Add(cls.Id);
        return cls.Id;
    }
}