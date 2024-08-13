namespace Educar.Backend.Domain.Entities;

public class Game(string name, string description, string lore, string purpose)
    : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string Lore { get; set; } = lore;
    public string Purpose { get; set; } = purpose;
    public IList<GameSubject> GameSubjects { get; set; } = new List<GameSubject>();
    public IList<GameProficiencyGroup> GameProficiencyGroups { get; set; } = new List<GameProficiencyGroup>();
    public IList<GameNpc> GameNpcs { get; set; } = new List<GameNpc>();
    public IList<Quest> Quests { get; set; } = new List<Quest>();
}