using Educar.Backend.Application.Queries.ProficiencyGroup;
using Educar.Backend.Domain.Entities;

namespace Educar.Backend.Application.Queries.Proficiency;

public class ProficiencyDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Purpose { get; set; }
    public List<ProficiencyGroupCleanDto> ProficiencyGroups { get; set; } = new();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Proficiency, ProficiencyDto>()
                .ForMember(dest => dest.ProficiencyGroups,
                    opt => opt.MapFrom(src =>
                        src.ProficiencyGroupProficiencies.Select(pgp => pgp.ProficiencyGroup))
                );
        }
    }
}