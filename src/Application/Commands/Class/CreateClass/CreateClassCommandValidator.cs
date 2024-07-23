using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Class.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.Purpose)
            .IsInEnum().WithMessage("Purpose must be a valid ClassPurpose.")
            .NotEqual(ClassPurpose.None).WithMessage("Purpose is required.");

        RuleFor(v => v.SchoolId).NotEmpty().WithMessage("School ID is required.");
    }
}