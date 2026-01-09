using System.ComponentModel.DataAnnotations.Schema;

namespace Educar.Backend.Domain.Entities;

public class ClassQuest : BaseAuditableEntity
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
    
    public Guid QuestId { get; set; }
    public Quest Quest { get; set; } = null!;
    
    public DateTime ExpirationDate { get; set; }
    
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
}
