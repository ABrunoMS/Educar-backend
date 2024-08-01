using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Item;

public class ItemDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Lore { get; set; }
    public ItemType? ItemType { get; set; }
    public ItemRarity? ItemRarity { get; set; }
    public decimal? SellValue { get; set; }
    public ItemDto? Dismantle { get; set; }
    public string? Reference2D { get; set; }
    public string? Reference3D { get; set; }
    public decimal? DropRate { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Item, ItemDto>();
        }
    }
}