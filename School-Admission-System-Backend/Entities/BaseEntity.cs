using static School_Admission_System_Backend.Entities.Common.EntityConstant;

namespace School_Admission_System_Backend.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int IsActive { get; set; } = 1;

        public void Active() => IsActive = (int)EntityStatus.Active;
        public void InActive() => IsActive = (int)EntityStatus.InActive;
        public void Delete() => IsActive = (int)EntityStatus.Deleted;
    }
}
