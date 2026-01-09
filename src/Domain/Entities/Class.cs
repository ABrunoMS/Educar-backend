using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Class(string name, string description, ClassPurpose purpose) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public ClassPurpose Purpose { get; set; } = purpose;
    public Guid SchoolId { get; set; }
    public School School { get; set; } = null!;
    public IList<AccountClass> AccountClasses { get; set; } = new List<AccountClass>();
    public bool IsActive { get; set; } = true;
    public string? SchoolYear { get; set; }
    public string? SchoolShift { get; set; }
    public IList<ClassProduct> ClassProducts { get; private set; } = new List<ClassProduct>();
    public IList<ClassContent> ClassContents { get; private set; } = new List<ClassContent>();
    public IList<ClassQuest> ClassQuests { get; private set; } = [];
}
