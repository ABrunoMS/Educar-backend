using FluentValidation;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Subsecretaria.CreateSubsecretaria;

public class CreateSubsecretariaCommandValidator : AbstractValidator<CreateSubsecretariaCommand>
{
    public CreateSubsecretariaCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");
    }
}
