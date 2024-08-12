using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;

public class ColumnFill : IExpectedAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.ColumnFill;
    public Dictionary<string, string> Matches { get; set; } = new Dictionary<string, string>();
}