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

        #region DbSets

        public DbSet<AdmissionClass> Classes { get; set; }
        public DbSet<Hold> Holds { get; set; }

        #endregion
    }
}
