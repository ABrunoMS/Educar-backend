using Educar.Backend.Application.Queries.Dialogue;
using Educar.Backend.Application.Queries.Game;
using Educar.Backend.Application.Queries.Item;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Npc;

public class NpcCleanDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Lore { get; set; }
    public NpcType? NpcType { get; set; }
    public decimal? GoldDropRate { get; set; }
    public decimal? GoldAmount { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Npc, NpcCleanDto>();
        }
    }
}