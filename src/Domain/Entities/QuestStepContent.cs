using System.Text.Json.Nodes;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class QuestStepContent(
    QuestStepContentType questStepContentType,
    QuestionType questionType,
    string description,
    JsonObject expectedAnswers,
    decimal weight)
    : BaseAuditableEntity
{
    public QuestStepContentType QuestStepContentType { get; set; } = questStepContentType;
    public QuestionType QuestionType { get; set; } = questionType;
    public string Description { get; set; } = description;
    public JsonObject ExpectedAnswers { get; set; } = expectedAnswers;
    public decimal Weight { get; set; } = weight;
    public Guid QuestStepId { get; set; }
    public QuestStep QuestStep { get; set; } = null!;
}