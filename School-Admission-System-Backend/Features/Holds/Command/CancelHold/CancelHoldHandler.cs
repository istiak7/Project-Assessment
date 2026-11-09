using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities.Common;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Command.CancelHold
{
    public class CancelHoldHandler : IRequestHandler<CancelHoldCommand, Result>
    {
        private readonly AppDbContext _db;

        public CancelHoldHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(CancelHoldCommand request, CancellationToken cancellationToken)
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

            if (hold.Status != HoldStatus.Cancelled)
            {
                hold.Status = HoldStatus.Cancelled;
                await _db.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return new Result
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status204NoContent,
                Status = "No Content"
            };
        }

        private static Result Fail(int statusCode, string message) => new()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Status = "Error",
            Message = message
        };
    }
}
