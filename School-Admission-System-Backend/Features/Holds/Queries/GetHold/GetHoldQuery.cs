using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    public record GetHoldQuery(int HoldId) : IRequest<Result>;
}
