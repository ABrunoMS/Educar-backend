using Educar.Backend.Domain.Common;

namespace Educar.Backend.Domain.Entities;

public class Subsecretaria : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public Client? Client { get; set; }
    public ICollection<Regional> Regionais { get; private set; } = new List<Regional>();
}
