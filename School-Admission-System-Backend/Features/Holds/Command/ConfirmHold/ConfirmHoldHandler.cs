using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities.Common;
using School_Admission_System_Backend.Features.Holds.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Command.ConfirmHold
{
    public class ConfirmHoldHandler : IRequestHandler<ConfirmHoldCommand, ConfirmHoldResponse>
    {
        private readonly AppDbContext _db;

        public ConfirmHoldHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ConfirmHoldResponse> Handle(ConfirmHoldCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT 1 FROM holds WHERE \"Id\" = {request.HoldId} FOR UPDATE",
                cancellationToken);

            var hold = await _db.Holds
                .FirstOrDefaultAsync(h => h.Id == request.HoldId, cancellationToken);

            if (hold is null)
                throw new AppException(StatusCodes.Status404NotFound, "Hold not found.");

            if (hold.StudentId != request.StudentId)
                throw new AppException(StatusCodes.Status403Forbidden, "This application belongs to another student.");

            // Confirming an already-confirmed hold is idempotent: no extra seat is taken.
            if (hold.Status == HoldStatus.Confirmed)
                return Confirmed(hold.Id);

            if (hold.Status != HoldStatus.Held)
                throw new AppException(StatusCodes.Status409Conflict, "This application can no longer be confirmed.");

            if (hold.ExpiresAt <= DateTime.UtcNow)
                throw new AppException(StatusCodes.Status409Conflict, "This application has expired.");

            hold.Status = HoldStatus.Confirmed;
            hold.ConfirmedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Confirmed(hold.Id);
        }

        private static ConfirmHoldResponse Confirmed(int holdId) =>
            new(holdId.ToString(), HoldStatus.Confirmed.ToApiString());
    }
}
