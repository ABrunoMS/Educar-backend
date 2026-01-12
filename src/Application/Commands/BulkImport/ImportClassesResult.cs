namespace Educar.Backend.Application.Commands.BulkImport;

public class ImportClassesResult
{
    public bool Success { get; set; }
    public int TotalInserted { get; set; }
    public string? ErrorMessage { get; set; }
}
