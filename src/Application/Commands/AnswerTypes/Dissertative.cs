using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class Dissertative : IAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.Dissertative;
    public string Text { get; set; } = string.Empty;
}