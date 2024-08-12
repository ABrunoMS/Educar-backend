using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;

public interface IExpectedAnswer
{
    QuestionType QuestionType { get; }
}