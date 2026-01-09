using System.Text.Json.Nodes;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.QuestStepContent;

public class QuestStepContentDto
{
    public Guid Id { get; set; }
    public QuestStepContentType? QuestStepContentType { get; set; }
    public QuestionType? QuestionType { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public JsonObject? ExpectedAnswers { get; set; }
    public decimal Weight { get; set; }
    public bool IsActive { get; set; }
    public int Sequence { get; set; }
    public Guid QuestStepId { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.QuestStepContent, QuestStepContentDto>()
                .ForMember(dest => dest.ExpectedAnswers, opt => opt.Ignore()); // Mapeamento manual na query
        }
    }
}

public class JsonObjectConverter : IValueConverter<JsonObject, JsonObject?>
{
    public JsonObject? Convert(JsonObject sourceMember, ResolutionContext context)
    {
        if (sourceMember == null) return null;
        
        try
        {
            // Cria uma cópia deep do JsonObject para evitar erro de "node already has a parent"
            var jsonString = sourceMember.ToJsonString();
            var parsed = JsonNode.Parse(jsonString);
            return parsed as JsonObject;
        }
        catch
        {
            return null;
        }
    }
}