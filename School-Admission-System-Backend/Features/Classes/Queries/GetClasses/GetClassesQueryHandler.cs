using MediatR;
using Microsoft.EntityFrameworkCore;
using School_Admission_System_Backend.Database;
using School_Admission_System_Backend.Entities.Common;
using School_Admission_System_Backend.Features.Classes.Shared;
using School_Admission_System_Backend.Shared;

namespace School_Admission_System_Backend.Features.Classes.Queries.GetClasses
{
    public class GetClassesQueryHandler : IRequestHandler<GetClassesQuery, Result>
    {
        private readonly AppDbContext _db;

        public GetClassesQueryHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(GetClassesQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var data = await _db.Classes
                .OrderBy(c => c.Id)
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
                .ToListAsync(cancellationToken);

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
