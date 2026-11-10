using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClasses
{
    public class GetClassesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/classes", async (ISender sender) =>
                Results.Ok(await sender.Send(new GetClassesQuery()))).WithTags("Classes");
        }
    }
}
