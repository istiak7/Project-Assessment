namespace School_Admission_System_Backend.Shared.Extensions
{
    public static class ResultExtensions
    {
        public static IResult ToHttpResult(this Result result)
        {
            if (result.StatusCode == StatusCodes.Status204NoContent)
                return Results.NoContent();

            if (result.IsSuccess)
                return Results.Json(result.Data, statusCode: result.StatusCode);

            return Results.Json(new { message = result.Message }, statusCode: result.StatusCode);
        }
    }
}
