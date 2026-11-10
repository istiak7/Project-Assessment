using MediatR;

namespace School_Admission_System_Backend.Features.Holds.Command.CreateHold
{
    public record CreateHoldCommand(int ClassId, string StudentId) : IRequest<CreateHoldResponse>;
}
