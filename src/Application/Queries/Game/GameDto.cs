using Educar.Backend.Application.Queries.Npc;
using Educar.Backend.Application.Queries.ProficiencyGroup;
using Educar.Backend.Application.Queries.Subject;

namespace Educar.Backend.Application.Queries.Game;

public class GameDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Lore { get; set; }
    public string? Purpose { get; set; }
    public List<SubjectDto> Subjects { get; set; } = new();
    public List<ProficiencyGroupCleanDto> ProficiencyGroups { get; set; } = new();
    public List<NpcCleanDto> Npcs { get; set; } = new();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Game, GameDto>()
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.GameSubjects.Select(gs => gs.Subject)))
                .ForMember(dest => dest.ProficiencyGroups,
                    opt => opt.MapFrom(src => src.GameProficiencyGroups.Select(gpg => gpg.ProficiencyGroup)))
                .ForMember(dest => dest.Npcs, opt => opt.MapFrom(src => src.GameNpcs.Select(gn => gn.Npc)));
        }
    }
}