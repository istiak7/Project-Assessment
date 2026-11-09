using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School_Admission_System_Backend.Entities;

namespace School_Admission_System_Backend.Database.Configurations
{
    public class AdmissionClassConfiguration : IEntityTypeConfiguration<AdmissionClass>
    {
        public void Configure(EntityTypeBuilder<AdmissionClass> builder)
        {
            builder.ToTable("classes");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
            builder.Property(c => c.TotalSeats).IsRequired();

            builder.HasMany(c => c.Holds)
                   .WithOne(h => h.AdmissionClass)
                   .HasForeignKey(h => h.AdmissionClassId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
