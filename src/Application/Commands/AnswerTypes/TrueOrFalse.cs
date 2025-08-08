using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class TrueOrFalse : IAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.TrueOrFalse;
    public bool Choice { get; set; }
}