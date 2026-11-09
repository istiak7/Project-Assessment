using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Holds.Command.CancelHold
{
    public class CancelHoldEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/holds/{holdId}",
                async (string holdId, HttpRequest httpRequest, ISender sender) =>
                {
                    if (!int.TryParse(holdId, out var id))
                        return Results.Json(new { message = "Hold not found." },
                            statusCode: StatusCodes.Status404NotFound);

                    var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                    var result = await sender.Send(new CancelHoldCommand(id, studentId));
                    return result.ToHttpResult();
                }).WithTags("Holds");
        }
    }
}
