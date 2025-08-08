using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Item : BaseAuditableEntity
{
    public Item(string name, string lore, ItemType itemType, ItemRarity itemRarity, decimal sellValue, string reference2D, string reference3D, decimal dropRate)
    {
        Name = name;
        Lore = lore;
        ItemType = itemType;
        ItemRarity = itemRarity;
        SellValue = sellValue;
        Reference2D = reference2D;
        Reference3D = reference3D;
        DropRate = dropRate;
    }

    public string Name { get; set; }
    public string Lore { get; set; }
    public ItemType ItemType { get; set; }
    public ItemRarity ItemRarity { get; set; }
    public decimal SellValue { get; set; }
    public Guid? DismantleId { get; set; }
    public Item? Dismantle { get; set; }
    public string Reference2D { get; set; }
    public string Reference3D { get; set; }
    public decimal DropRate { get; set; }
    public IList<NpcItem> NpcItems { get; set; } = new List<NpcItem>();
    public IList<QuestStepItem> QuestStepItems { get; set; } = new List<QuestStepItem>();
}