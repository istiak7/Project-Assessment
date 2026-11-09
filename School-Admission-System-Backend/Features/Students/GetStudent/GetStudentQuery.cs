using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Students.GetStudent
{
    public class GetStudentQuery : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
