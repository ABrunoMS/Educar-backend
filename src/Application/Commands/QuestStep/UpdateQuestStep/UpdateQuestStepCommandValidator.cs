using Educar.Backend.Application.Commands.QuestStep.CreateQuestStep;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStep.UpdateQuestStep;

public class UpdateQuestStepCommandValidator : AbstractValidator<UpdateQuestStepCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateQuestStepCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(v => v.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(v => v.Description).NotEmpty().WithMessage("Description is required.");
    }
}