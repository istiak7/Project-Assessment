using MediatR;
using School_Admission_System_Backend.Features.Classes.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClass
{
    public record GetClassQuery(int ClassId) : IRequest<ClassResponse>;
}
