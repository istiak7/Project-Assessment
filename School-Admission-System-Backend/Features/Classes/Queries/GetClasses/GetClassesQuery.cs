using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClasses
{
    public record GetClassesQuery() : IRequest<Result>;
}
