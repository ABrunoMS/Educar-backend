using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;

public class AlwaysCorrect : IExpectedAnswer
{
    // No specific fields needed as any answer is correct
    public QuestionType QuestionType { get; } = QuestionType.AlwaysCorrect;
}