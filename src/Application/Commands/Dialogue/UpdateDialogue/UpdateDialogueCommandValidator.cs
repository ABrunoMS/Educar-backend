using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Dialogue.UpdateDialogue;

public class UpdateDialogueCommandValidator : AbstractValidator<UpdateDialogueCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDialogueCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(v => v.Text).NotEmpty().WithMessage("Text is required.");
        RuleFor(v => v.Order)
            .GreaterThan(0).WithMessage("Order is required and must be greater than 0.")
            .MustAsync(BeUniqueOrder).WithMessage("Order must be unique for this Npc.");
        RuleFor(v => v.NpcId).NotEmpty().WithMessage("NpcId is required.");
    }

    private async Task<bool> BeUniqueOrder(UpdateDialogueCommand command, int? order,
        CancellationToken cancellationToken)
    {
        return await _context.Dialogues
            .Where(d => d.NpcId == command.NpcId && d.Id != command.Id) // Exclude the current dialogue being updated
            .AllAsync(d => d.Order != order, cancellationToken);
    }
}