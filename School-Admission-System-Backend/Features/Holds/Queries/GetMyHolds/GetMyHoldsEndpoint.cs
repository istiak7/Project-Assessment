using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetMyHolds
{
    public class GetMyHoldsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/me/holds", async (HttpRequest httpRequest, ISender sender) =>
            {
                var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                return Results.Ok(await sender.Send(new GetMyHoldsQuery(studentId)));
            }).WithTags("Holds");
        }
    }
}
