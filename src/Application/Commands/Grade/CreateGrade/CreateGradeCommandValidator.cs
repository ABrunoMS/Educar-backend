namespace Educar.Backend.Application.Commands.Grade.CreateGradeCommand;

public class CreateGradeCommandValidator : AbstractValidator<CreateGradeCommand>
{
    public CreateGradeCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.")
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.Description).NotEmpty().WithMessage("Description is required.");
    }
}