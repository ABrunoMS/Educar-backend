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

        RuleFor(v => v.QuestId).NotEmpty().WithMessage("QuestId is required.");
    }
}