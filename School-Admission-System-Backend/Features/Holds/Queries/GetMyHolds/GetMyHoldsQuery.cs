using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetMyHolds
{
    public record GetMyHoldsQuery(string StudentId) : IRequest<Result>;
}
