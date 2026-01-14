namespace Educar.Backend.Application.Commands.BulkImport;

public class QuestClassLinkRow
{
    public Guid QuestId { get; set; }
    public Guid ClassId { get; set; }
    public string ExpirationDate { get; set; } = string.Empty;
}
