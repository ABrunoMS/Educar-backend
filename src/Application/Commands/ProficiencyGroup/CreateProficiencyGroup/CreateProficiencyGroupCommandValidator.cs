using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.ProficiencyGroup.CreateProficiencyGroup;

public class CreateProficiencyGroupCommandValidator : AbstractValidator<CreateProficiencyGroupCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateProficiencyGroupCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .MustAsync(BeUniqueName).WithMessage("'{PropertyName}' must be unique.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _context.ProficiencyGroups
            .AllAsync(l => l.Name != name, cancellationToken);
    }
}