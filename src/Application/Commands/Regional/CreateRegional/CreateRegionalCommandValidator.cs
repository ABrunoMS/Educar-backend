using FluentValidation;

namespace Educar.Backend.Application.Commands.Regional.CreateRegional;

public class CreateRegionalCommandValidator : AbstractValidator<CreateRegionalCommand>
{
    public CreateRegionalCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

        RuleFor(x => x.SubsecretariaId)
            .NotEmpty().WithMessage("SubsecretariaId é obrigatório.");
    }
}
