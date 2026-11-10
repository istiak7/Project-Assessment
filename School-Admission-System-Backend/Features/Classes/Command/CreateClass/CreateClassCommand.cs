using MediatR;

namespace School_Admission_System_Backend.Features.Classes.Command.CreateClass
{
    public record CreateClassCommand(string Name, int TotalSeats) : IRequest<CreateClassResponse>;
}
