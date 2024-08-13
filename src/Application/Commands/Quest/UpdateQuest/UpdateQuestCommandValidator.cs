using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Quest.UpdateQuest;

public class UpdateQuestCommandValidator : AbstractValidator<UpdateQuestCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateQuestCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.UsageTemplate)
            .IsInEnum().WithMessage("UsageTemplate must be valid.")
            .NotEqual(QuestUsageTemplate.None).WithMessage("UsageTemplate is required.");

        RuleFor(v => v.Type)
            .IsInEnum().WithMessage("Type must be valid.")
            .NotEqual(QuestType.None).WithMessage("Type is required.");

        RuleFor(v => v.MaxPlayers)
            .InclusiveBetween(2, 5).WithMessage("MaxPlayers should be between 2 and 5.");

        RuleFor(v => v.TotalQuestSteps)
            .InclusiveBetween(1, 10).WithMessage("TotalQuestSteps should be between 1 and 10.")
            .MustAsync(BeGreaterThanOrEqualToCurrentSteps)
            .WithMessage("TotalQuestSteps cannot be less than the current number of quest steps.");

        RuleFor(v => v.CombatDifficulty)
            .IsInEnum().WithMessage("CombatDifficulty must be a valid CombatDifficulty.")
            .NotEqual(CombatDifficulty.None).WithMessage("CombatDifficulty is required.");

        RuleFor(v => v.SubjectId).NotEmpty().WithMessage("SubjectId is required.");
        RuleFor(v => v.GradeId).NotEmpty().WithMessage("GradeId is required.");
    }

    private async Task<bool> BeGreaterThanOrEqualToCurrentSteps(UpdateQuestCommand command, int? newTotalQuestSteps,
        CancellationToken cancellationToken)
    {
        if (newTotalQuestSteps == null)
        {
            return true; // No need to validate if the field is not being updated
        }

        var existingQuest = await _context.Quests
            .Include(q => q.QuestSteps)
            .FirstOrDefaultAsync(q => q.Id == command.Id, cancellationToken);

        if (existingQuest == null)
        {
            return false; // This should trigger a NotFoundException before reaching this rule
        }

        return newTotalQuestSteps >= existingQuest.QuestSteps.Count;
    }
}