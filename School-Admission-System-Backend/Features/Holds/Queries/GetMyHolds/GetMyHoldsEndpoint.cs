using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetMyHolds
{
    public class GetMyHoldsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/me/holds", async (HttpRequest httpRequest, ISender sender) =>
            {
                var studentId = httpRequest.Headers["X-Student-Id"].ToString();

                var result = await sender.Send(new GetMyHoldsQuery(studentId));
                return result.ToHttpResult();
            }).WithTags("Holds");
        }
    }
}
