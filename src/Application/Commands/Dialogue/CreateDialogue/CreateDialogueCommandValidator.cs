using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Dialogue.CreateDialogue;

public class CreateDialogueCommandValidator : AbstractValidator<CreateDialogueCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDialogueCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Text).NotEmpty().WithMessage("Text is required.");
        RuleFor(v => v.Order)
            .Must(x => x > 0).WithMessage("Order is required and must be greater than 0.")
            .MustAsync(BeUniqueOrder).WithMessage("Order must be unique for this Npc.");
        RuleFor(v => v.NpcId).NotEmpty().WithMessage("NpcId is required.");
    }

    private async Task<bool> BeUniqueOrder(CreateDialogueCommand command, int order, CancellationToken cancellationToken)
    {
        return await _context.Dialogues
            .Where(d => d.NpcId == command.NpcId)
            .AllAsync(d => d.Order != order, cancellationToken);
    }
}