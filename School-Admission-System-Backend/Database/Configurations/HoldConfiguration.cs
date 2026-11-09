using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School_Admission_System_Backend.Entities;

namespace School_Admission_System_Backend.Database.Configurations
{
    public class HoldConfiguration : IEntityTypeConfiguration<Hold>
    {
        public void Configure(EntityTypeBuilder<Hold> builder)
        {
            builder.ToTable("holds");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.StudentId).IsRequired().HasMaxLength(17);
            builder.Property(h => h.Status).IsRequired().HasConversion<int>();
            builder.Property(h => h.ExpiresAt).IsRequired();

            // Seat-count queries filter by class + status (GET /classes math).
            builder.HasIndex(h => new { h.AdmissionClassId, h.Status });

            // Resolves "this student's active hold in this class" (rule 3) and
            // backs GET /me/holds lookups.
            builder.HasIndex(h => new { h.StudentId, h.AdmissionClassId });
        }
    }
}
