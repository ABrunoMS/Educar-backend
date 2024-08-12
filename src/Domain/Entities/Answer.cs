using System.Text.Json.Nodes;

namespace Educar.Backend.Domain.Entities;

public class Answer(JsonObject givenAnswer, bool isCorrect) : BaseAuditableEntity
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid QuestStepContentId { get; set; }
    public QuestStepContent QuestStepContent { get; set; } = null!;

    public JsonObject GivenAnswer { get; set; } = givenAnswer;
    public bool IsCorrect { get; set; } = isCorrect;
}