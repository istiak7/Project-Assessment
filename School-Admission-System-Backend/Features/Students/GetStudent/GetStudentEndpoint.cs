using MediatR;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Students.GetStudent
{
    public class GetStudentEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/students/{id:int}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetStudentQuery { Id = id });
                return Results.Ok(result);
            });
        }
    }
}
