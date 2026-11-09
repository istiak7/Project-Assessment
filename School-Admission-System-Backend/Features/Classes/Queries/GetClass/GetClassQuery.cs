using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClass
{
    public record GetClassQuery(int ClassId) : IRequest<Result>;
}
