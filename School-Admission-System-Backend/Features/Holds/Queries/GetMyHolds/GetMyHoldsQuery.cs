using MediatR;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetMyHolds
{
    public record GetMyHoldsQuery(string StudentId) : IRequest<List<MyHoldResponse>>;
}
