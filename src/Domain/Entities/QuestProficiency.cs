using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

public class QuestProficiency : ISoftDelete
{
    public Guid QuestId { get; set; }
    public Quest Quest { get; set; } = null!;

    public Guid ProficiencyId { get; set; }
    public Proficiency Proficiency { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}