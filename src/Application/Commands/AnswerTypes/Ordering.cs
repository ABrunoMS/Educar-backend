using System.Text.Json.Serialization;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class Ordering : IAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.Ordering;

    // List of items presented to the user
    [JsonPropertyName("items")]
    public List<string> Items { get; set; } = new();

    // Correct order represented as indexes into `Items` in the desired sequence
    [JsonPropertyName("correctOrder")]
    public List<int> CorrectOrder { get; set; } = new();
}
