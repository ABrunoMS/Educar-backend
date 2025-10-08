using FluentValidation;

namespace Educar.Backend.Application.Commands.Regional.CreateRegional;

public class CreateRegionalCommandValidator : AbstractValidator<CreateRegionalCommand>
{
    public CreateRegionalCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.SubsecretariaId)
            .NotEmpty().WithMessage("SubsecretariaId é obrigatório.");
    }
}
