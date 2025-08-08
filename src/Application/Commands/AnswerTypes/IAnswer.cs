using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public interface IAnswer
{
    QuestionType QuestionType { get; }
}