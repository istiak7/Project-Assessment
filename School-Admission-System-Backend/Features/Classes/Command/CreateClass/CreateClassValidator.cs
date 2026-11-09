using FluentValidation;

namespace School_Admission_System_Backend.Features.Classes.Command.CreateClass
{
    public class CreateClassValidator : AbstractValidator<CreateClassCommand>
    {
        public CreateClassValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Class name is required.")
                .MaximumLength(150);

            RuleFor(x => x.TotalSeats)
                .GreaterThan(0).WithMessage("Total seats must be greater than zero.");
        }
    }
}
