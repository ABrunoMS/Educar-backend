namespace Educar.Backend.Domain.Entities;

public class ProficiencyGroup(string name, string description) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public IList<ProficiencyGroupProficiency> ProficiencyGroupProficiencies { get; set; } =
        new List<ProficiencyGroupProficiency>();
    public IList<GameProficiencyGroup> GameProficiencyGroups { get; set; } = new List<GameProficiencyGroup>();
}