using MediatR;
using School_Admission_System_Backend.Entities;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Repository;

namespace School_Admission_System_Backend.Features.Students.GetStudent
{
    public class GetStudentQueryHandler(IBaseRepository<Student> _studentRepository)
        : IRequestHandler<GetStudentQuery, Result>
    {
        public async Task<Result> Handle(GetStudentQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);
            if (student == null)
            {
                return new Result
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Status = "Error",
                    Message = "Student not found"
                };
            }

            var response = new StudentResponse
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };

            return new Result
            {
                IsSuccess = true,
                StatusCode = 200,
                Status = "Success",
                Message = "Student retrieved successfully",
                Data = response
            };
        }
    }
}
