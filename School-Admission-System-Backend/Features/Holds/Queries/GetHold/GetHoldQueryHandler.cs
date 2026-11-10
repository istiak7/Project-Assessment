using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Features.Holds.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetHold
{
    public class GetHoldQueryHandler : IRequestHandler<GetHoldQuery, GetHoldResponse>
    {
        private readonly AppDbContext _db;

        public GetHoldQueryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<GetHoldResponse> Handle(GetHoldQuery request, CancellationToken cancellationToken)
        {
            var hold = await _db.Holds
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == request.HoldId, cancellationToken);

            if (hold is null)
                throw new AppException(StatusCodes.Status404NotFound, "Hold not found.");

            var now = DateTime.UtcNow;
            return new GetHoldResponse(
                hold.Id.ToString(),
                hold.AdmissionClassId.ToString(),
                hold.Effective(now).ToApiString(),
                hold.ExpiresAt);
        }
    }
}
