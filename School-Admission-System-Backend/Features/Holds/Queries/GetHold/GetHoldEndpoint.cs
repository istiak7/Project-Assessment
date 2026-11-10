using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    public class GetHoldEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/holds/{holdId}", async (string holdId, ISender sender) =>
            {
                if (!int.TryParse(holdId, out var id))
                    throw new AppException(StatusCodes.Status404NotFound, "Hold not found.");

                return Results.Ok(await sender.Send(new GetHoldQuery(id)));
            }).WithTags("Holds");
        }
    }
}
