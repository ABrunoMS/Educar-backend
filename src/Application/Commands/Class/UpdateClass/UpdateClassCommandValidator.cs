using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Class.UpdateClass;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
    public UpdateClassCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.Purpose)
            .IsInEnum().WithMessage("Purpose must be a valid ClassPurpose.")
            .NotEqual(ClassPurpose.None).WithMessage("Purpose is required.");
    }
}