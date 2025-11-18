namespace Educar.Backend.Domain.Entities;

public class BnccQuest : BaseAuditableEntity
{
    public Guid QuestId { get; set; }
    public Quest Quest { get; set; } = null!;
    
    public Guid BnccId { get; set; }
    public Bncc Bncc { get; set; } = null!;
}
