using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Command.CreateHold
{
    public record CreateHoldCommand(int ClassId, string StudentId) : IRequest<Result>;
}
