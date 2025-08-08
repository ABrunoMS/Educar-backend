namespace Educar.Backend.Application.Commands.Subject.UpdateSubject;

public class UpdateSubjectCommandValidator : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.")
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.Description).NotEmpty().WithMessage("Description is required.");
    }
}