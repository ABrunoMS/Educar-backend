using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Media(string name, MediaPurpose purpose, MediaType type, bool agreement, string url, string objectName)
    : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public MediaPurpose Purpose { get; set; } = purpose;
    public MediaType Type { get; set; } = type;
    public string? References { get; set; }
    public string? Author { get; set; }
    public bool Agreement { get; set; } = agreement;
    public string Url { get; set; } = url;
    public string ObjectName { get; set; } = objectName;
    
    public IList<QuestStepMedia> QuestStepMedias { get; set; } = new List<QuestStepMedia>();
}