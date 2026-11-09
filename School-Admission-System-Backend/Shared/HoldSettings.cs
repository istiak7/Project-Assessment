namespace School_Admission_System_Backend.Shared
{
    // Bound from the HOLD_DURATION_SECONDS environment variable (default 600).
    // Controls how long a seat stays HELD before it auto-expires.
    public sealed class HoldSettings
    {
        public int DurationSeconds { get; set; } = 600;
    }
}
