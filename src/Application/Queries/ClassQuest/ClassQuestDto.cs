using AutoMapper;

namespace Educar.Backend.Application.Queries.ClassQuest;

public class ClassQuestDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public Guid QuestId { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public bool IsExpired { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.ClassQuest, ClassQuestDto>()
                .ForMember(d => d.IsExpired, opt => opt.MapFrom(s => s.IsExpired));
        }
    }
}
