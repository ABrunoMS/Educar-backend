using System.Text.Json.Serialization;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.QuestStepContent.ExpectedAnswerTypes;

public class MultipleChoice : IExpectedAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.MultipleChoice;
    [JsonPropertyName("options")]
    public List<MultipleChoiceOption> Options { get; set; } = [];
}

public class MultipleChoiceOption
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("is_correct")]
    public bool IsCorrect { get; set; }
}