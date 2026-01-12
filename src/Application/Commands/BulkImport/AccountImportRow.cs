namespace Educar.Backend.Application.Commands.BulkImport;

public class AccountImportRow
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
}
