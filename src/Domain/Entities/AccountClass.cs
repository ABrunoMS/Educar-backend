using Educar.Backend.Infrastructure.Data;

namespace Educar.Backend.Domain.Entities;

public class AccountClass : ISoftDelete
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}