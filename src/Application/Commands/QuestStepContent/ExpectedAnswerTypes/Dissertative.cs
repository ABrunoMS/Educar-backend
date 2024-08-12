using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;

public class Dissertative : IExpectedAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.Dissertative;
    public string ExpectedText { get; set; } = string.Empty;
}