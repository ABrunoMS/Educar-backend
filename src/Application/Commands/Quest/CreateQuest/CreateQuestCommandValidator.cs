using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.Quest.CreateQuest;

public class CreateQuestCommandValidator : AbstractValidator<CreateQuestCommand>
{
    public CreateQuestCommandValidator()
    {
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
            .InclusiveBetween(1, 10).WithMessage("TotalQuestSteps should be between 1 and 10.");

        RuleFor(v => v.CombatDifficulty)
            .IsInEnum().WithMessage("CombatDifficulty must be a valid CombatDifficulty.")
            .NotEqual(CombatDifficulty.None).WithMessage("CombatDifficulty is required.");

        RuleFor(v => v.GameId).NotEmpty().WithMessage("GameId is required.");
        RuleFor(v => v.SubjectId).NotEmpty().WithMessage("SubjectId is required.");
        RuleFor(v => v.GradeId).NotEmpty().WithMessage("GradeId is required.");
    }
}