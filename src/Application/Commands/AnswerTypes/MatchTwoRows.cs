using System.Text.Json.Serialization;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class MatchTwoRows : IAnswer
{
    public QuestionType QuestionType { get; } = QuestionType.MatchTwoRows;

    // Left column items
    [JsonPropertyName("left")]
    public List<string> Left { get; set; } = new();

    // Right column items
    [JsonPropertyName("right")]
    public List<string> Right { get; set; } = new();

    // Mapping from left index -> right index representing correct matches
    [JsonPropertyName("matches")]
    public Dictionary<int, int> Matches { get; set; } = new();
}
