using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Answer.CreateAnswer;

public class CreateAnswerCommandValidator : AbstractValidator<CreateAnswerCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateAnswerCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.AccountId)
            .NotEmpty().WithMessage("AccountId is required.");

        RuleFor(v => v.QuestStepContentId)
            .NotEmpty().WithMessage("QuestStepContentId is required.");

        RuleFor(v => v.IsCorrect)
            .NotNull().WithMessage("IsCorrect is required."); // Changed to NotNull because it's a boolean

        RuleFor(v => v.GivenAnswer)
            .MustAsync(ValidateGivenAnswer)
            .WithMessage("GivenAnswer is not valid.")
            .MustAsync(ValidateAnswerExists)
            .WithMessage("Answer already exists for this account.");
    }

    private async Task<bool> ValidateGivenAnswer(CreateAnswerCommand command, IAnswer givenAnswer,
        CancellationToken cancellationToken)
    {
        var questStepContent = await _context.QuestStepContents
            .FirstOrDefaultAsync(qsc => qsc.Id == command.QuestStepContentId, cancellationToken);

        return questStepContent != null && questStepContent.QuestionType == givenAnswer.QuestionType &&
               AnswerTypeValidator.ValidateAnswer(givenAnswer.QuestionType, givenAnswer);
    }

    private async Task<bool> ValidateAnswerExists(CreateAnswerCommand command, IAnswer givenAnswer,
        CancellationToken cancellationToken)
    {
        return await _context.Answers
            .FirstOrDefaultAsync(
                a => a.AccountId == command.AccountId && a.QuestStepContentId == command.QuestStepContentId,
                cancellationToken) == null;
    }
}