using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClass
{
    public class GetClassEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/classes/{classId}", async (string classId, ISender sender) =>
            {
                if (!int.TryParse(classId, out var id))
                    throw new AppException(StatusCodes.Status404NotFound, "Class not found.");

                return Results.Ok(await sender.Send(new GetClassQuery(id)));
            }).WithTags("Classes");
        }
    }
}
