using Educar.Backend.Application.Queries.Content;
using Educar.Backend.Application.Queries.Game;
using Educar.Backend.Application.Queries.Grade;
using Educar.Backend.Application.Queries.Product;
using Educar.Backend.Application.Queries.Subject;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Quest;

public class QuestCleanDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? UsageTemplate { get; set; }
    public QuestType? Type { get; set; }
    public int? MaxPlayers { get; set; }
    public int? TotalQuestSteps { get; set; }
    public CombatDifficulty? CombatDifficulty { get; set; }
    public Guid? CreatedBy { get; set; }
    public GameCleanDto? Game { get; set; }
    public GradeDto? Grade { get; set; }
    public SubjectDto? Subject { get; set; }
    public QuestCleanDto? QuestDependency { get; set; }
    public ContentDto? Content { get; set; }
    public ProductDto? Product { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Quest, QuestCleanDto>();
        }
    }
}
