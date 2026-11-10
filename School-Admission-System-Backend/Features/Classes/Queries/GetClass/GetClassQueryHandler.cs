using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities.Common;
using School_Admission_System_Backend.Features.Classes.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClass
{
    public class GetClassQueryHandler : IRequestHandler<GetClassQuery, ClassResponse>
    {
        private readonly AppDbContext _db;

        public GetClassQueryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ClassResponse> Handle(GetClassQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var data = await _db.Classes
                .Where(c => c.Id == request.ClassId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.TotalSeats,
                    HeldSeats = c.Holds.Count(h => h.Status == HoldStatus.Held && h.ExpiresAt > now),
                    ConfirmedSeats = c.Holds.Count(h => h.Status == HoldStatus.Confirmed)
                })
                .Select(c => new ClassResponse(
                    c.Id.ToString(),
                    c.Name,
                    c.TotalSeats,
                    c.HeldSeats,
                    c.ConfirmedSeats,
                    c.TotalSeats - c.HeldSeats - c.ConfirmedSeats))
                .FirstOrDefaultAsync(cancellationToken);

            if (data is null)
                throw new AppException(StatusCodes.Status404NotFound, "Class not found.");

            return data;
        }
    }
}
