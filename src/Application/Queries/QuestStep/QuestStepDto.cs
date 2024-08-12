using Educar.Backend.Application.Queries.Item;
using Educar.Backend.Application.Queries.Media;
using Educar.Backend.Application.Queries.Npc;
using Educar.Backend.Application.Queries.QuestStepContent;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.QuestStep;

public class QuestStepDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
    public QuestStepNpcType? NpcType { get; set; }
    public QuestStepNpcBehaviour? NpcBehaviour { get; set; }
    public QuestStepType? QuestStepType { get; set; }
    public IList<NpcCleanDto> Npcs { get; set; } = new List<NpcCleanDto>();
    public IList<MediaDto> Medias { get; set; } = new List<MediaDto>();
    public IList<ItemDto> Items { get; set; } = new List<ItemDto>();
    public IList<QuestStepContentDto> Contents { get; set; } = new List<QuestStepContentDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.QuestStep, QuestStepDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.QuestStepItems.Select(ni => ni.Item)))
                .ForMember(dest => dest.Npcs, opt => opt.MapFrom(src => src.QuestStepNpcs.Select(ni => ni.Npc)))
                .ForMember(dest => dest.Medias, opt => opt.MapFrom(src => src.QuestStepMedias.Select(ni => ni.Media)))
                .ForMember(dest => dest.Contents, opt => opt.MapFrom(src => src.Contents));
        }
    }
}