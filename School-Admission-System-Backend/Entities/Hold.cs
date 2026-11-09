using School_Admission_System_Backend.Entities.Common;

namespace School_Admission_System_Backend.Entities
{

    public class Hold : BaseEntity
    {
        public int AdmissionClassId { get; set; }
        public required string StudentId { get; set; }
        public HoldStatus Status { get; set; } = HoldStatus.Held;
        public DateTime ExpiresAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public AdmissionClass AdmissionClass { get; set; } = null!;
    }
}
