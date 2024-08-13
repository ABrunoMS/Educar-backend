using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStep.CreateQuestStep;

public class CreateQuestStepCommandValidator : AbstractValidator<CreateQuestStepCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateQuestStepCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(v => v.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(v => v.NpcType)
            .NotEqual(QuestStepNpcType.None).WithMessage("NpcType is required.");
        RuleFor(v => v.NpcBehaviour)
            .NotEqual(QuestStepNpcBehaviour.None).WithMessage("NpcBehaviour is required.");
        RuleFor(v => v.Type)
            .NotEqual(QuestStepType.None).WithMessage("Type is required.");
        RuleFor(v => v.Order)
            .Must(x => x > 0).WithMessage("Order is required and must be greater than 0.")
            .MustAsync(BeUniqueOrder).WithMessage("Order must be unique for this Quest.");

        RuleFor(v => v.QuestId).NotEmpty().WithMessage("QuestId is required.")
            .MustAsync(BeWithinTotalQuestSteps)
            .WithMessage("Total Quest Steps must be less or equal Quest.TotalQuestSteps.");
    }

    private async Task<bool> BeUniqueOrder(CreateQuestStepCommand command, int order,
        CancellationToken cancellationToken)
    {
        return await _context.QuestSteps
            .Where(d => d.QuestId == command.QuestId)
            .AllAsync(d => d.Order != order, cancellationToken);
    }

    private async Task<bool> BeWithinTotalQuestSteps(Guid questId, CancellationToken cancellationToken)
    {
        var quest = await _context.Quests
            .Include(q => q.QuestSteps)
            .FirstOrDefaultAsync(q => q.Id == questId, cancellationToken);

        // have to be less than (not <=) because there will be a new step added
        return quest != null && quest.QuestSteps.Count < quest.TotalQuestSteps;
    }
}