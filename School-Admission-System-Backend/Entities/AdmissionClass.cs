namespace School_Admission_System_Backend.Entities
{
    public class AdmissionClass : BaseEntity
    {
        public required string Name { get; set; }
        public int TotalSeats { get; set; }
        public ICollection<Hold> Holds { get; set; } = [];
    }
}
