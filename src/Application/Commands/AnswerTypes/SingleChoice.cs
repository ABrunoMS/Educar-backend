using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class SingleChoice : IAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.SingleChoice;
    public string Option { get; set; } = string.Empty;
}