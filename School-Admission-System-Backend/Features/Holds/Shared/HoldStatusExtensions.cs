using School_Admission_System_Backend.Entities;
using School_Admission_System_Backend.Entities.Common;

namespace School_Admission_System_Backend.Features.Holds.Shared
{
    public static class HoldStatusExtensions
    {
        // A Held hold whose ExpiresAt has passed is treated as EXPIRED on read,
        // even if a background sweeper has not persisted that transition yet.
        // This is what makes expiry automatic (mandatory rule 2).
        public static HoldStatus Effective(HoldStatus status, DateTime expiresAt, DateTime nowUtc)
            => status == HoldStatus.Held && expiresAt <= nowUtc
                ? HoldStatus.Expired
                : status;

        public static HoldStatus Effective(this Hold hold, DateTime nowUtc)
            => Effective(hold.Status, hold.ExpiresAt, nowUtc);

        // Enum -> contract string (HELD / CONFIRMED / EXPIRED / CANCELLED).
        public static string ToApiString(this HoldStatus status) => status switch
        {
            HoldStatus.Held => "HELD",
            HoldStatus.Confirmed => "CONFIRMED",
            HoldStatus.Expired => "EXPIRED",
            HoldStatus.Cancelled => "CANCELLED",
            _ => status.ToString().ToUpperInvariant()
        };
    }
}
