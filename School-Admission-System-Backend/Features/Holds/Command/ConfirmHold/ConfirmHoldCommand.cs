using MediatR;

namespace School_Admission_System_Backend.Features.Holds.Command.ConfirmHold
{
    public record ConfirmHoldCommand(int HoldId, string StudentId) : IRequest<ConfirmHoldResponse>;
}
