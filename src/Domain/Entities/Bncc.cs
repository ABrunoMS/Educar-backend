namespace Educar.Backend.Domain.Entities;

public class Bncc(
    string description,
    bool isActive
)
    : BaseAuditableEntity
{
    public string Description { get; set; } = description;
    public bool IsActive { get; set; } = isActive;
    
    public IList<BnccQuest> BnccQuests { get; set; } = new List<BnccQuest>();
}