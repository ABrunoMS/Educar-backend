using Educar.Backend.Domain.Common;

namespace Educar.Backend.Domain.Entities;

public class Regional : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid SubsecretariaId { get; set; }
    public Subsecretaria? Subsecretaria { get; set; }
    public ICollection<School> Schools { get; private set; } = new List<School>();
}
