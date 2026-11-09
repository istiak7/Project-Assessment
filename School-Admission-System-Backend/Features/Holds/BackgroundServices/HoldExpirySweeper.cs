using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities.Common;

namespace School_Admission_System_Backend.Features.Holds.BackgroundServices
{
    public class HoldExpiryService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);
        private readonly IServiceScopeFactory _scopeFactory;

        public HoldExpiryService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var now = DateTime.UtcNow;
                    await db.Holds
                        .Where(h => h.Status == HoldStatus.Held && h.ExpiresAt <= now)
                        .ExecuteUpdateAsync(
                            setters => setters
                                .SetProperty(h => h.Status, HoldStatus.Expired)
                                .SetProperty(h => h.UpdatedAt, now),
                            stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
