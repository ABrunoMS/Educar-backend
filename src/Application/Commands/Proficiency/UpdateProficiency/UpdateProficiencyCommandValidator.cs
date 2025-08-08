using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Proficiency.UpdateProficiency;

public class UpdateProficiencyCommandValidator : AbstractValidator<UpdateProficiencyCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProficiencyCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required.");
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

    private async Task<bool> BeUniqueProficiencyName(UpdateProficiencyCommand command, string name,
        CancellationToken cancellationToken)
    {
        return await _context.Proficiencies
            .Where(g => g.Id != command.Id) //
            .AllAsync(l => l.Name != name, cancellationToken);
    }
}