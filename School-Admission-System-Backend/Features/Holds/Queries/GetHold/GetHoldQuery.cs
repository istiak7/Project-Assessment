using MediatR;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    public record GetHoldQuery(int HoldId) : IRequest<GetHoldResponse>;
}
