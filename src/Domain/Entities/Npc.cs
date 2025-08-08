using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Npc : BaseAuditableEntity
{
    public Npc(string name, string lore, NpcType npcType, decimal goldDropRate, decimal goldAmount)
    {
        Name = name;
        Lore = lore;
        NpcType = npcType;
        GoldDropRate = goldDropRate;
        GoldAmount = goldAmount;
    }

    public string Name { get; set; }
    public string Lore { get; set; }
    public NpcType NpcType { get; set; }
    public decimal GoldDropRate { get; set; }
    public decimal GoldAmount { get; set; }
    public IList<NpcItem> NpcItems { get; set; } = new List<NpcItem>();
    public IList<Dialogue> Dialogues { get; set; } = new List<Dialogue>();
    public IList<GameNpc> GameNpcs { get; set; } = new List<GameNpc>();
    public IList<QuestStepNpc> QuestStepNpcs { get; set; } = new List<QuestStepNpc>();
    
}