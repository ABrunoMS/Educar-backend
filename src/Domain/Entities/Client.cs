namespace Educar.Backend.Domain.Entities;

public class Client(string name) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string? Description { get; set; }
    public Contract? Contract { get; set; }
}