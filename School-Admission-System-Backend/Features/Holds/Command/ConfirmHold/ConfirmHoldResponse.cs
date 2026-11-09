namespace School_Admission_System_Backend.Features.Holds.Command.ConfirmHold
{
    // { "holdId","status" }
    public record ConfirmHoldResponse(string HoldId, string Status);
}
