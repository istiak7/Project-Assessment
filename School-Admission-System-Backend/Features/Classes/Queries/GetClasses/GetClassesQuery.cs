using MediatR;
using School_Admission_System_Backend.Features.Classes.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClasses
{
    public record GetClassesQuery() : IRequest<List<ClassResponse>>;
}
