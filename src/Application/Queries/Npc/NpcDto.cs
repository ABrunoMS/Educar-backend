using Educar.Backend.Application.Queries.Dialogue;
using Educar.Backend.Application.Queries.Item;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Npc;

public class NpcDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Lore { get; set; }
    public NpcType? NpcType { get; set; }
    public decimal? GoldDropRate { get; set; }
    public decimal? GoldAmount { get; set; }
    public List<ItemDto>? Items { get; set; }
    public List<DialogueDto>? Dialogues { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Npc, NpcDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.NpcItems.Select(ni => ni.Item)))
                .ForMember(dest => dest.Dialogues, opt => opt.MapFrom(src => src.Dialogues));
        }
    }
}