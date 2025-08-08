namespace Educar.Backend.Application.Commands.School.UpdateSchool;

public class UpdateSchoolCommandValidator : AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
    }
}