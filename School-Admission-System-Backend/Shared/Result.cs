namespace School_Admission_System_Backend.Shared
{
    public sealed class Result
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
