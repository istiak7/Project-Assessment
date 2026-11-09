namespace School_Admission_System_Backend.Entities
{
    // Sample domain entity for the School Admission System.
    // Extend / replace as the project grows.
    public class Student : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}
