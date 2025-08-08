using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.ProficiencyGroup.UpdateProficiencyGroup;

public class UpdateProficiencyGroupCommandValidator : AbstractValidator<UpdateProficiencyGroupCommand>
{
    private readonly IApplicationDbContext _context;


    public UpdateProficiencyGroupCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .MustAsync(BeUniqueName).WithMessage("Name must be unique.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }

    private async Task<bool> BeUniqueName(UpdateProficiencyGroupCommand command, string name,
        CancellationToken cancellationToken)
    {
        return await _context.ProficiencyGroups
            .Where(g => g.Id != command.Id)
            .AllAsync(g => g.Name != name, cancellationToken);
    }
}