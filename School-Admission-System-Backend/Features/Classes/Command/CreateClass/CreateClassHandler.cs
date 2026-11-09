using MediatR;
using School_Admission_System_Backend.Entities;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Repository;

namespace School_Admission_System_Backend.Features.Classes.Command.CreateClass
{
    public class CreateClassHandler : IRequestHandler<CreateClassCommand, Result>
    {
        private readonly IBaseRepository<AdmissionClass> _classes;

        public CreateClassHandler(IBaseRepository<AdmissionClass> classes)
        {
            _classes = classes;
        }

        public async Task<Result> Handle(CreateClassCommand request, CancellationToken cancellationToken)
        {
            var entity = new AdmissionClass
            {
                Name = request.Name,
                TotalSeats = request.TotalSeats
            };

            await _classes.AddAsync(entity, cancellationToken);
            await _classes.SaveChangesAsync(cancellationToken);

            var response = new CreateClassResponse(entity.Id.ToString(), entity.Name, entity.TotalSeats);

            return new Result
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status201Created,
                Status = "Created",
                Data = response
            };
        }
    }
}
