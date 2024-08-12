using System.Text.Json.Nodes;

namespace Educar.Backend.Application.Queries.Answer;

public class AnswerDto
{
    public Guid Id { get; set; }
    public JsonObject? GivenAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public Guid QuestStepContentId { get; set; }
    public Guid AccountId { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Answer, AnswerDto>();
        }
    }
}