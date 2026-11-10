namespace School_Admission_System_Backend.Shared
{
    // Thrown by handlers to signal an expected, user-facing failure with an HTTP
    // status code. The ExceptionHandlingMiddleware turns it into a simple
    // { "message": "..." } body with the matching status code.
    public sealed class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
