using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities.Common;
using School_Admission_System_Backend.Features.Holds.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Command.ConfirmHold
{
    public class ConfirmHoldHandler : IRequestHandler<ConfirmHoldCommand, Result>
    {
        private readonly AppDbContext _db;

        public ConfirmHoldHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(ConfirmHoldCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            await _db.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT 1 FROM holds WHERE \"Id\" = {request.HoldId} FOR UPDATE",
                cancellationToken);

            var hold = await _db.Holds
                .FirstOrDefaultAsync(h => h.Id == request.HoldId, cancellationToken);

            if (hold is null)
                return Fail(StatusCodes.Status404NotFound, "Hold not found.");

            if (hold.StudentId != request.StudentId)
                return Fail(StatusCodes.Status403Forbidden, "This application belongs to another student.");

            if (hold.Status == HoldStatus.Confirmed)
                return Ok(hold.Id.ToString());

            if (hold.Status != HoldStatus.Held)
                return Fail(StatusCodes.Status409Conflict, "This application can no longer be confirmed.");

            if (hold.ExpiresAt <= DateTime.UtcNow)
                return Fail(StatusCodes.Status409Conflict, "This application has expired.");

            hold.Status = HoldStatus.Confirmed;
            hold.ConfirmedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Ok(hold.Id.ToString());
        }

        private static Result Ok(string holdId) => new()
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Status = "OK",
            Data = new ConfirmHoldResponse(holdId, HoldStatus.Confirmed.ToApiString())
        };

        private static Result Fail(int statusCode, string message) => new()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Status = "Error",
            Message = message
        };
    }
}
