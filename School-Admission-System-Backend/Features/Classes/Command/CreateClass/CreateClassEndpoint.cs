using MediatR;
using School_Admission_System_Backend.Shared;
using School_Admission_System_Backend.Shared.Extensions;

namespace School_Admission_System_Backend.Features.Classes.Command.CreateClass
{
    public class CreateClassEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/classes", async (CreateClassCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.ToHttpResult();
            }).WithTags("Classes");
        }
    }
}
