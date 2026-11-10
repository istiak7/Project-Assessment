using MediatR;

namespace School_Admission_System_Backend.Features.Holds.Command.CancelHold
{
    public record CancelHoldCommand(int HoldId, string StudentId) : IRequest<Unit>;
}
