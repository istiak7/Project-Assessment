namespace School_Admission_System_Backend.Features.Holds.Queries.GetMyHolds
{
    public record MyHoldResponse(
        string HoldId,
        string ClassId,
        string ClassName,
        string Status,
        DateTime ExpiresAt);
}
