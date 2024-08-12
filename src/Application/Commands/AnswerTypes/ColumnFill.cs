using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class ColumnFill : IAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.ColumnFill;
    public Dictionary<string, string> Matches { get; set; } = new();
}