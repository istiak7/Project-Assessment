using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Command.ConfirmHold
{
    public record ConfirmHoldCommand(int HoldId, string StudentId) : IRequest<Result>;
}
