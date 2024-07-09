namespace Educar.Backend.Domain.Entities;

public class Client(string name) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string? Description { get; set; }
    public IList<Contract>? Contracts { get; private set; } = new List<Contract>();
    public IList<Account>? Accounts { get; private set; } = new List<Account>();
}