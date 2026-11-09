using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Holds.Command.CreateHold
{
    public class CreateHoldEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/classes/{classId}/holds",
                async (string classId, HttpRequest httpRequest, ISender sender) =>
                {
                    if (!int.TryParse(classId, out var id))
                        return Results.Json(new { message = "Class not found." },
                            statusCode: StatusCodes.Status404NotFound);

                    var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                    var result = await sender.Send(new CreateHoldCommand(id, studentId));
                    return result.ToHttpResult();
                }).WithTags("Holds");
        }
    }
}
