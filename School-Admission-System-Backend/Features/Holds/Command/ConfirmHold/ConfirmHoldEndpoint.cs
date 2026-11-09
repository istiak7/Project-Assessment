using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Holds.Command.ConfirmHold
{
    public class ConfirmHoldEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/holds/{holdId}/confirm",
                async (string holdId, HttpRequest httpRequest, ISender sender) =>
                {
                    if (!int.TryParse(holdId, out var id))
                        return Results.Json(new { message = "Hold not found." },
                            statusCode: StatusCodes.Status404NotFound);

                    var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                    var result = await sender.Send(new ConfirmHoldCommand(id, studentId));
                    return result.ToHttpResult();
                }).WithTags("Holds");
        }
    }
}
