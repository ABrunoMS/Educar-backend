namespace Educar.Backend.Application.Commands.BulkImport;

public class ClassImportRow
{
    public string ClassName { get; set; } = string.Empty;
    public Guid SchoolId { get; set; }
}
