using AutoMapper;

namespace Educar.Backend.Application.Queries.BnccQuest;

public class BnccQuestDto
{
    // public Guid Id { get; set; }
    // public Guid QuestId { get; set; }
    // public Guid BnccId { get; set; }
    public string BnccDescription { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.BnccQuest, BnccQuestDto>()
                .ForMember(dest => dest.BnccDescription, opt => opt.MapFrom(src => src.Bncc.Description));
        }
    }
}