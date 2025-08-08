namespace Educar.Backend.Application.Commands.Grade.UpdateGrade;

public class UpdateGradeCommandValidator: AbstractValidator<UpdateGradeCommand>
{
    public UpdateGradeCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");
        
        RuleFor(v => v.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.")
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.Description).NotEmpty().WithMessage("Description is required.");
    }
}