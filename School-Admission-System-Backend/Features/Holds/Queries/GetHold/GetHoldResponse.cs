namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    // { "holdId","classId","status","expiresAt" }
    public record GetHoldResponse(
        string HoldId,
        string ClassId,
        string Status,
        DateTime ExpiresAt);
}
