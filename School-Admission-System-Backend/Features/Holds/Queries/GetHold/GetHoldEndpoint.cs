using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    public class GetHoldEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/holds/{holdId}", async (string holdId, ISender sender) =>
            {
                if (!int.TryParse(holdId, out var id))
                    return Results.Json(new { message = "Hold not found." },
                        statusCode: StatusCodes.Status404NotFound);

                var result = await sender.Send(new GetHoldQuery(id));
                return result.ToHttpResult();
            }).WithTags("Holds");
        }
    }
}
