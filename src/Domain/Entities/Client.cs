namespace Educar.Backend.Domain.Entities;

public class Client : BaseAuditableEntity
{
    public Client(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; }
    public string Description { get; set; }
}