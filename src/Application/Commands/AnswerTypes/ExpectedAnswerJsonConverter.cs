using System.Text.Json;
using System.Text.Json.Serialization;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Commands.AnswerTypes;

public class ExpectedAnswerJsonConverter : JsonConverter<IAnswer>
{
    public override IAnswer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        if (!root.TryGetProperty("question_type", out var questionTypeElement))
            throw new JsonException("Missing discriminator field 'questionType'");
        var questionType = Enum.Parse<QuestionType>(questionTypeElement.GetString() ?? string.Empty);

        var json = root.GetRawText();

        return questionType switch
        {
            QuestionType.MultipleChoice => JsonSerializer.Deserialize<MultipleChoice>(json, options),
            QuestionType.TrueOrFalse => JsonSerializer.Deserialize<TrueOrFalse>(json, options),
            QuestionType.SingleChoice => JsonSerializer.Deserialize<SingleChoice>(json, options),
            QuestionType.Dissertative => JsonSerializer.Deserialize<Dissertative>(json, options),
            QuestionType.ColumnFill => JsonSerializer.Deserialize<ColumnFill>(json, options),
            QuestionType.AlwaysCorrect => JsonSerializer.Deserialize<AlwaysCorrect>(json, options),
            _ => throw new JsonException("Unknown question type")
        };
    }

    public override void Write(Utf8JsonWriter writer, IAnswer value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}