using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.CreateQuestStepContent;

public class CreateQuestStepContentCommandValidator : AbstractValidator<CreateQuestStepContentCommand>
{
    public CreateQuestStepContentCommandValidator()
    {
        RuleFor(v => v.QuestStepId)
            .NotEmpty().WithMessage("QuestStepId is required.");

        RuleFor(v => v.QuestStepContentType)
            .IsInEnum().WithMessage("QuestStepContentType is not valid.")
            .NotEqual(QuestStepContentType.None).WithMessage("QuestStepContentType is required.");

        RuleFor(v => v.QuestionType)
            .IsInEnum().WithMessage("QuestionType is not valid.")
            .NotEqual(QuestionType.None).WithMessage("QuestionType is required.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.");

        RuleFor(v => v.Answers)
            .Must((command, expectedAnswers) =>
                AnswerTypeValidator.ValidateAnswer(command.QuestionType, expectedAnswers))
            .WithMessage("Answers is not valid for the specified QuestionType.");
    }
}