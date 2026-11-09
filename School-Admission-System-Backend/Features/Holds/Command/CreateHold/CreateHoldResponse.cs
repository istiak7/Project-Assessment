namespace School_Admission_System_Backend.Features.Holds.Command.CreateHold
{
    // { "holdId","classId","studentId","status","expiresAt" }
    public record CreateHoldResponse(
        string HoldId,
        string ClassId,
        string StudentId,
        string Status,
        DateTime ExpiresAt);
}
