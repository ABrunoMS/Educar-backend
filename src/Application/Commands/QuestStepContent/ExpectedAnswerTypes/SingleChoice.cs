using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;

public class SingleChoice : IExpectedAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.SingleChoice;
    public string CorrectOption { get; set; } = string.Empty;
}