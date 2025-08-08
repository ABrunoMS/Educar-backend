namespace Educar.Backend.Application.Commands.School.CreateSchool;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("ClientId is required.");
    }
}