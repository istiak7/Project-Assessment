using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Command.CreateClass
{
    public class CreateClassEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/classes", async (CreateClassCommand command, ISender sender) =>
            {
                var response = await sender.Send(command);
                return Results.Json(response, statusCode: StatusCodes.Status201Created);
            }).WithTags("Classes");
        }
    }
}
