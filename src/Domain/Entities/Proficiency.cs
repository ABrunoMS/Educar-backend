namespace Educar.Backend.Domain.Entities;

public class Proficiency(string name, string description, string purpose) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string Purpose { get; set; } = purpose;
    public IList<ProficiencyGroupProficiency> ProficiencyGroupProficiencies { get; set; } =
        new List<ProficiencyGroupProficiency>();
    
    public IList<QuestProficiency> QuestProficiencies { get; set; } = new List<QuestProficiency>();
}