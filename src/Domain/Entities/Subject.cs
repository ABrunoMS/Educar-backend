namespace Educar.Backend.Domain.Entities;

public class Subject(string name, string description) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public IList<GameSubject> GameSubjects { get; set; } = new List<GameSubject>();
    public IList<Quest> Quests { get; set; } = new List<Quest>();
}