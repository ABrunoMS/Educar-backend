using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Proficiency.CreateProficiency;

public class CreateProficiencyCommandValidator : AbstractValidator<CreateProficiencyCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateProficiencyCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .MustAsync(BeUniqueProficiencyName).WithMessage("The specified name already exists.")
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.Purpose)
            .MaximumLength(255).WithMessage("Purpose must not exceed 255 characters.")
            .NotEmpty().WithMessage("Purpose is required.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");
    }

    private async Task<bool> BeUniqueProficiencyName(string name, CancellationToken cancellationToken)
    {
        return await _context.Proficiencies.AllAsync(l => l.Name != name, cancellationToken);
    }
}