using System.Text.Json.Nodes;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.QuestStepContent;

public class QuestStepContentDto
{
    public Guid Id { get; set; }
    public QuestStepContentType? QuestStepContentType { get; set; }
    public QuestionType? QuestionType { get; set; }
    public JsonObject? ExpectedAnswers { get; set; }
    public decimal Weight { get; set; }
    public Guid QuestStepId { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.QuestStepContent, QuestStepContentDto>();
        }
    }
}