using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities;
using School_Admission_System_Backend.Entities.Common;
using School_Admission_System_Backend.Features.Holds.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Command.CreateHold
{
    public class CreateHoldHandler : IRequestHandler<CreateHoldCommand, Result>
    {
        private readonly AppDbContext _dbContext;
        private readonly HoldSettings _holdSettings;

        public CreateHoldHandler(AppDbContext db, HoldSettings holdSettings)
        {
            _dbContext = db;
            _holdSettings = holdSettings;
        }

        public async Task<Result> Handle(CreateHoldCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            // lock the class row to prevent race conditions when checking for available seats

            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT 1 FROM classes WHERE \"Id\" = {request.ClassId} FOR UPDATE",
                cancellationToken);


            // check if the class exists
            var admissionClass = await _dbContext.Classes
                .FirstOrDefaultAsync(c => c.Id == request.ClassId, cancellationToken);

            if (admissionClass is null)
                return Fail(StatusCodes.Status404NotFound, "Class not found.");



            var now = DateTime.UtcNow;

            var hasActive = await _dbContext.Holds.AnyAsync(h =>
                h.AdmissionClassId == request.ClassId &&
                h.StudentId == request.StudentId &&
                (h.Status == HoldStatus.Confirmed ||
                 (h.Status == HoldStatus.Held && h.ExpiresAt > now)),
                cancellationToken);

            if (hasActive)
                return Fail(StatusCodes.Status409Conflict,
                    "You already have an active application for this class.");




            // check if there are available seats

            var confirmedSeats = await _dbContext.Holds.CountAsync(h =>
                h.AdmissionClassId == request.ClassId &&
                h.Status == HoldStatus.Confirmed, cancellationToken);

            var heldSeats = await _dbContext.Holds.CountAsync(h =>
                h.AdmissionClassId == request.ClassId &&
                h.Status == HoldStatus.Held && h.ExpiresAt > now, cancellationToken);

            if (confirmedSeats + heldSeats >= admissionClass.TotalSeats)
                return Fail(StatusCodes.Status409Conflict, "No seats available for this class.");


            // create the hold

            var hold = new Hold
            {
                AdmissionClassId = request.ClassId,
                StudentId = request.StudentId,
                Status = HoldStatus.Held,
                ExpiresAt = now.AddSeconds(_holdSettings.DurationSeconds)
            };

            _dbContext.Holds.Add(hold);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new CreateHoldResponse(
                hold.Id.ToString(),
                hold.AdmissionClassId.ToString(),
                hold.StudentId,
                hold.Status.ToApiString(),
                hold.ExpiresAt);

            return new Result
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status201Created,
                Status = "Created",
                Data = response
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
