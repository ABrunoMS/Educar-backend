namespace Educar.Backend.Application.Commands.BulkImport;

public class LinkAccountsToClientResult
{
    public bool Success { get; set; }
    public int TotalLinked { get; set; }
    public int TotalUpdated { get; set; }
    public List<string> Warnings { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
