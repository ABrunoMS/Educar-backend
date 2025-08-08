namespace Educar.Backend.Domain.Entities;

public class Grade(string name, string description) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public IList<Quest> Quests { get; set; } = new List<Quest>();
}