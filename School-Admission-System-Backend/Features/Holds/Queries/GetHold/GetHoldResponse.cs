namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    public record GetHoldResponse(
        string HoldId,
        string ClassId,
        string Status,
        DateTime ExpiresAt);
}
