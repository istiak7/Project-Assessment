namespace School_Admission_System_Backend.Features.Classes.Shared
{
    public record ClassResponse(
        string Id,
        string Name,
        int TotalSeats,
        int HeldSeats,
        int ConfirmedSeats,
        int AvailableSeats);
}
