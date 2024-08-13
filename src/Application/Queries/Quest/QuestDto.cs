using Educar.Backend.Application.Queries.Game;
using Educar.Backend.Application.Queries.Grade;
using Educar.Backend.Application.Queries.Proficiency;
using Educar.Backend.Application.Queries.QuestStep;
using Educar.Backend.Application.Queries.Subject;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Quest;

public class QuestDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public QuestUsageTemplate? UsageTemplate { get; set; }
    public QuestType? Type { get; set; }
    public int? MaxPlayers { get; set; }
    public int? TotalQuestSteps { get; set; }
    public CombatDifficulty? CombatDifficulty { get; set; }
    public GameCleanDto? Game { get; set; }
    public GradeDto? Grade { get; set; }
    public SubjectDto? Subject { get; set; }
    public QuestCleanDto? QuestDependency { get; set; }
    public IList<QuestStepDto> QuestSteps { get; set; } = new List<QuestStepDto>();
    public IList<ProficiencyCleanDto> Proficiencies { get; set; } = new List<ProficiencyCleanDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Quest, QuestDto>()
                .ForMember(dest => dest.QuestSteps, opt => opt.MapFrom(src => src.QuestSteps))
                .ForMember(dest => dest.Proficiencies,
                    opt => opt.MapFrom(src => src.QuestProficiencies.Select(ni => ni.Proficiency)));
        }
    }
}