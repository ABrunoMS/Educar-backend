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

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Game, GameDto>()
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.GameSubjects.Select(gs => gs.Subject)));
        }
    }
}