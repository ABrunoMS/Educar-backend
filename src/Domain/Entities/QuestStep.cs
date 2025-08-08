using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class QuestStep(
    string name,
    string description,
    int order,
    QuestStepNpcType npcType,
    QuestStepNpcBehaviour npcBehaviour,
    QuestStepType questStepType)
    : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public int Order { get; set; } = order;
    public QuestStepNpcType NpcType { get; set; } = npcType;
    public QuestStepNpcBehaviour NpcBehaviour { get; set; } = npcBehaviour;
    public QuestStepType QuestStepType { get; set; } = questStepType;
    public IList<QuestStepContent> Contents { get; set; } = new List<QuestStepContent>();
    public IList<QuestStepNpc> QuestStepNpcs { get; set; } = new List<QuestStepNpc>();
    public IList<QuestStepMedia> QuestStepMedias { get; set; } = new List<QuestStepMedia>();
    public IList<QuestStepItem> QuestStepItems { get; set; } = new List<QuestStepItem>();
    
    public Guid QuestId { get; set; }
    public Quest Quest { get; set; } = null!;
}