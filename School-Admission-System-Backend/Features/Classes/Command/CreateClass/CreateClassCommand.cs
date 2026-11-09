using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Command.CreateClass
{
    public record CreateClassCommand(string Name, int TotalSeats) : IRequest<Result>;
}
