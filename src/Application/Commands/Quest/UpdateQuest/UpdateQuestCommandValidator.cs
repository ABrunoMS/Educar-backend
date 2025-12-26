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

        RuleFor(v => v.UsageTemplate).NotNull().WithMessage("UsageTemplate is required.");

        RuleFor(v => v.Type)
            .IsInEnum().WithMessage("Type must be valid.")
            .NotEqual(QuestType.None).WithMessage("Type is required.");

        RuleFor(v => v.MaxPlayers)
            .InclusiveBetween(2, 5).WithMessage("MaxPlayers should be between 2 and 5.");

        RuleFor(v => v.CombatDifficulty)
            .IsInEnum().WithMessage("CombatDifficulty must be a valid CombatDifficulty.")
            .NotEqual(CombatDifficulty.None).WithMessage("CombatDifficulty is required.");

        // RuleFor(v => v.SubjectId).NotEmpty().WithMessage("SubjectId is required.");
        // RuleFor(v => v.GradeId).NotEmpty().WithMessage("GradeId is required.");
    }
}