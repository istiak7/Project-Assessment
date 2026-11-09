using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClass
{
    public class GetClassEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/classes/{classId}", async (string classId, ISender sender) =>
            {
                if (!int.TryParse(classId, out var id))
                    return Results.Json(new { message = "Class not found." }, statusCode: StatusCodes.Status404NotFound);

                var result = await sender.Send(new GetClassQuery(id));
                return result.ToHttpResult();
            }).WithTags("Classes");
        }
    }
}
