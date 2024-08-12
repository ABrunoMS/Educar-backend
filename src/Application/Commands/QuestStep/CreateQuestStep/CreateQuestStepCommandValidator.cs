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
            .Must(x => x > 0).WithMessage("Order is required and must be greater than 0.");
        // .MustAsync(BeUniqueOrder).WithMessage("Order must be unique for this Npc.");
    }
    // private async Task<bool> BeUniqueOrder(CreateQuestStepCommand command, int order, CancellationToken cancellationToken)
    // {
    //     return await _context.QuestSteps
    //         .Where(d => d.NpcId == command.NpcId)
    //         .AllAsync(d => d.Order != order, cancellationToken);
    // }
}