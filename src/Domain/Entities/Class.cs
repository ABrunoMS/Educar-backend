using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Class(string name, string description, ClassPurpose purpose) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public ClassPurpose Purpose { get; set; } = purpose;
    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;
    public List<string>? Content { get; set; } = new();
    public IList<AccountClass> AccountClasses { get; set; } = new List<AccountClass>();
}