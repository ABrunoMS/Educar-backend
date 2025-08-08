using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

public class GameSubject : ISoftDelete
{
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}