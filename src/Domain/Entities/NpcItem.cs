using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

public class NpcItem : ISoftDelete
{
    public Guid NpcId { get; set; }
    public Npc Npc { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}