using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;

public class TrueOrFalse : IExpectedAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.TrueOrFalse;
    public bool CorrectAnswer { get; set; }
}