using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

public class GameNpc : ISoftDelete
{
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public Guid NpcId { get; set; }
    public Npc Npc { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}