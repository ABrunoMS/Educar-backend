namespace Educar.Backend.Application.Queries.Proficiency;

public class ProficiencyCleanDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Purpose { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Proficiency, ProficiencyCleanDto>();
        }
    }
}