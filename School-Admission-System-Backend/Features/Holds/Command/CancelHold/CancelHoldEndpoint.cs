using MediatR;
using School_Admission_System_Backend.Shared;

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
                        throw new AppException(StatusCodes.Status404NotFound, "Hold not found.");

                    var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                    await sender.Send(new CancelHoldCommand(id, studentId));
                    return Results.NoContent();
                }).WithTags("Holds");
        }
    }
}
