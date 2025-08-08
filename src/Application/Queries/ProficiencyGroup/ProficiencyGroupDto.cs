using Educar.Backend.Application.Queries.Proficiency;

namespace Educar.Backend.Application.Queries.ProficiencyGroup;

public class ProficiencyGroupDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<ProficiencyCleanDto> Proficiencies { get; set; } = new();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.ProficiencyGroup, ProficiencyGroupDto>()
                .ForMember(dest => dest.Proficiencies,
                    opt => opt.MapFrom(src =>
                        src.ProficiencyGroupProficiencies.Select(gs => gs.Proficiency))
                );
        }
    }
}