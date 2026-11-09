using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Features.Holds.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    public class GetHoldQueryHandler : IRequestHandler<GetHoldQuery, Result>
    {
        private readonly AppDbContext _db;

        public GetHoldQueryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(GetHoldQuery request, CancellationToken cancellationToken)
        {
            var hold = await _db.Holds
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == request.HoldId, cancellationToken);

            if (hold is null)
            {
                return new Result
                {
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Hold not found."
                };
            }

            var now = DateTime.UtcNow;
            var response = new GetHoldResponse(
                hold.Id.ToString(),
                hold.AdmissionClassId.ToString(),
                hold.Effective(now).ToApiString(),
                hold.ExpiresAt);

            return new Result
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Status = "OK",
                Data = response
            };
        }
    }
}
