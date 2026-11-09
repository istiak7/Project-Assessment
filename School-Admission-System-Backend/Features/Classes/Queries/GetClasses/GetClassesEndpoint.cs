using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClasses
{
    public class GetClassesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/classes", async (ISender sender) =>
            {
                var result = await sender.Send(new GetClassesQuery());
                return result.ToHttpResult();
            }).WithTags("Classes");
        }
    }
}
