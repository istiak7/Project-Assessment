using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Features.Holds.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetMyHolds
{
    public class GetMyHoldsQueryHandler : IRequestHandler<GetMyHoldsQuery, Result>
    {
        private readonly AppDbContext _db;

        public GetMyHoldsQueryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(GetMyHoldsQuery request, CancellationToken cancellationToken)
        {
            var rows = await _db.Holds
                .AsNoTracking()
                .Where(h => h.StudentId == request.StudentId)
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new
                {
                    h.Id,
                    h.AdmissionClassId,
                    ClassName = h.AdmissionClass.Name,
                    h.Status,
                    h.ExpiresAt
                })
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var data = rows.Select(h => new MyHoldResponse(
                h.Id.ToString(),
                h.AdmissionClassId.ToString(),
                h.ClassName,
                HoldStatusExtensions.Effective(h.Status, h.ExpiresAt, now).ToApiString(),
                h.ExpiresAt)).ToList();

            return new Result
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Status = "OK",
                Data = data
            };
        }
    }
}
