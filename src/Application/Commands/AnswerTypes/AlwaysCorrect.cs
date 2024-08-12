using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class AlwaysCorrect : IAnswer
{
    // No specific fields needed as any answer is correct
    public QuestionType QuestionType { get; } = QuestionType.AlwaysCorrect;
}