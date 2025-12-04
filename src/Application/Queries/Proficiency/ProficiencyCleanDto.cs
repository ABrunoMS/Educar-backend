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

            // --- ADICIONE ESTE NOVO ---
            // Ensina a converter a tabela BNCC para este DTO
            CreateMap<Domain.Entities.Bncc, ProficiencyCleanDto>()
                //.ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name)) 
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description));
            // --------------------------
        }
    }
}