using MediatR;
using School_Admission_System_Backend.Entities;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Repository;

namespace School_Admission_System_Backend.Features.Students.CreateStudent
{
    public class CreateStudentHandler(IBaseRepository<Student> _studentRepository)
        : IRequestHandler<CreateStudentCommand, Result>
    {
        public async Task<Result> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var existing = await _studentRepository.GetAsync(s => s.Email == request.Email,
                cancellationToken: cancellationToken);
            if (existing != null)
            {
                return new Result
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Status = "Error",
                    Message = "A student with this email already exists"
                };
            }

            var student = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _studentRepository.AddAsync(student, cancellationToken);
            await _studentRepository.SaveChangesAsync(cancellationToken);

            return new Result
            {
                IsSuccess = true,
                StatusCode = 201,
                Status = "Success",
                Message = "Student created successfully",
                Data = new { student.Id }
            };
        }
    }
}
