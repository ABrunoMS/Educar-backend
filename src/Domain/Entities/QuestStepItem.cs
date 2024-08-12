using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

public class QuestStepItem : ISoftDelete
{
    public Guid QuestStepId { get; set; }
    public QuestStep QuestStep { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}