using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

public class QuestStepNpc : ISoftDelete
{
    public Guid QuestStepId { get; set; }
    public QuestStep QuestStep { get; set; } = null!;

    public Guid NpcId { get; set; }
    public Npc Npc { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}