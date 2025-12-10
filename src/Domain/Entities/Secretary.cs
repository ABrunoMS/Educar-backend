using Educar.Backend.Domain.Common;

namespace Educar.Backend.Domain.Entities;

public class Secretary : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Subsecretaria> Subsecretarias { get; private set; } = new List<Subsecretaria>();
}
