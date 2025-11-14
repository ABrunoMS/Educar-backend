using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Quest(
    string name,
    string description,
    bool usageTemplate,
    QuestType type,
    int maxPlayers,
    int totalQuestSteps,
    CombatDifficulty combatDifficulty)
    : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    // public QuestUsageTemplate UsageTemplate { get; set; } = usageTemplate;
    public bool UsageTemplate { get; set; } = usageTemplate;
    public QuestType Type { get; set; } = type;
    public int MaxPlayers { get; set; } = maxPlayers;
    public int TotalQuestSteps { get; set; } = totalQuestSteps;
    public CombatDifficulty CombatDifficulty { get; set; } = combatDifficulty;
    public Guid? GameId { get; set; }
    public Game? Game { get; set; }
    public Guid? GradeId { get; set; }
    public Grade? Grade { get; set; }
    public Guid? SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public Guid? QuestDependencyId { get; set; }
    public Quest? QuestDependency { get; set; }
    public IList<QuestStep> QuestSteps { get; set; } = new List<QuestStep>();
    public IList<QuestProficiency> QuestProficiencies { get; set; } = new List<QuestProficiency>();
}