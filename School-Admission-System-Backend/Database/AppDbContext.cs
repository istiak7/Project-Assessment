using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Entities;

namespace School_Admission_System_Backend.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        // Stamp audit timestamps (UTC) so PostgreSQL 'timestamp with time zone'
        // columns always receive UTC DateTimes, and CreatedAt gives us a stable
        // first-come-first-served ordering.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        #region DbSets

        public DbSet<AdmissionClass> Classes { get; set; }
        public DbSet<Hold> Holds { get; set; }

        #endregion
    }
}
