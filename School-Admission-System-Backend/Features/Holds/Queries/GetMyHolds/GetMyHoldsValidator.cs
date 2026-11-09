using FluentValidation;

namespace School_Admission_System_Backend.Features.Holds.Queries.GetMyHolds
{
    public class GetMyHoldsValidator : AbstractValidator<GetMyHoldsQuery>
    {
        public GetMyHoldsValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("X-Student-Id header is required.")
                .Matches(@"^\d{17}$").WithMessage("X-Student-Id must be a 17-digit number.");
        }
    }
}
