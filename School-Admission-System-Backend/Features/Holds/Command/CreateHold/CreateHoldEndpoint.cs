using MediatR;
using School_Admission_System_Backend.Shared;

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
                        throw new AppException(StatusCodes.Status404NotFound, "Class not found.");

                    var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                    var response = await sender.Send(new CreateHoldCommand(id, studentId));
                    return Results.Json(response, statusCode: StatusCodes.Status201Created);
                }).WithTags("Holds");
        }
    }
}
