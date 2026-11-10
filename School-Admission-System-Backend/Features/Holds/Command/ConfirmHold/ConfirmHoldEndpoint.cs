using MediatR;
using School_Admission_System_Backend.Shared;

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
                        throw new AppException(StatusCodes.Status404NotFound, "Hold not found.");

                    var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                    var response = await sender.Send(new ConfirmHoldCommand(id, studentId));
                    return Results.Ok(response);
                }).WithTags("Holds");
        }
    }
}
