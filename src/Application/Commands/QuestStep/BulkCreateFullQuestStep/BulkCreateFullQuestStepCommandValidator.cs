using Educar.Backend.Domain.Enums;
using FluentValidation;
using Educar.Backend.Application.Commands.AnswerTypes;
using Educar.Backend.Application.Commands.QuestStep.CreateFullQuestStep; // Para reusar o CreateFullQuestStepContentDto

namespace Educar.Backend.Application.Commands.QuestStep.BulkCreateFullQuestStep;

public class FullQuestStepDtoValidator : AbstractValidator<FullQuestStepDto>
{
    public FullQuestStepDtoValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0.");

        RuleFor(v => v.NpcType)
            .IsInEnum().WithMessage("NpcType is not valid.")
            .NotEqual(QuestStepNpcType.None).WithMessage("NpcType is required.");

        RuleFor(v => v.NpcBehaviour)
            .IsInEnum().WithMessage("NpcBehaviour is not valid.")
            .NotEqual(QuestStepNpcBehaviour.None).WithMessage("NpcBehaviour is required.");

        RuleFor(v => v.Type)
            .IsInEnum().WithMessage("QuestStepType is not valid.")
            .NotEqual(QuestStepType.None).WithMessage("QuestStepType is required.");

        RuleForEach(v => v.Contents).ChildRules(content =>
        {
            content.RuleFor(c => c.QuestStepContentType)
                .IsInEnum().WithMessage("QuestStepContentType is not valid.")
                .NotEqual(QuestStepContentType.None).WithMessage("QuestStepContentType is required.");

            content.RuleFor(c => c.QuestionType)
                .IsInEnum().WithMessage("QuestionType is not valid.")
                .NotEqual(QuestionType.None).WithMessage("QuestionType is required.");

            content.RuleFor(c => c.Description)
                .NotEmpty().WithMessage("Description is required.");

            content.RuleFor(c => c.ExpectedAnswers)
                .NotNull().WithMessage("ExpectedAnswers is required.");

            content.RuleFor(c => c.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.");

            content.RuleFor(c => c.ExpectedAnswers)
                .Must((c, expectedAnswers) =>
                    AnswerTypeValidator.ValidateAnswer(c.QuestionType, expectedAnswers))
                .WithMessage("ExpectedAnswers is not valid for the specified QuestionType.");
        });
    }
}


public class BulkCreateFullQuestStepCommandValidator : AbstractValidator<BulkCreateFullQuestStepCommand>
{
    public BulkCreateFullQuestStepCommandValidator()
    {
        RuleFor(v => v.QuestId)
            .NotEmpty().WithMessage("QuestId is required.");

        RuleFor(v => v.Steps)
            .NotEmpty().WithMessage("At least one step (Steps) is required.");

        RuleForEach(v => v.Steps)
            .SetValidator(new FullQuestStepDtoValidator());
    }
}