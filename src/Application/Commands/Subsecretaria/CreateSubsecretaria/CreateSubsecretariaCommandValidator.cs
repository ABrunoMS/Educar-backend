using FluentValidation;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Subsecretaria.CreateSubsecretaria;

public class CreateSubsecretariaCommandValidator : AbstractValidator<CreateSubsecretariaCommand>
{
    public CreateSubsecretariaCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");
    }
}
