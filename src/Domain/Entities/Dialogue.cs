namespace Educar.Backend.Domain.Entities;

public class Dialogue(string text, int order) : BaseAuditableEntity
{
    public string Text { get; set; } = text;
    public int Order { get; set; } = order;
    public Guid NpcId { get; set; }
    public Npc Npc { get; set; } = null!;
}