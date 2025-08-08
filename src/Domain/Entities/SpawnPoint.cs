namespace Educar.Backend.Domain.Entities;

public class SpawnPoint : BaseAuditableEntity
{
    public SpawnPoint(string name, string reference, decimal x, decimal y, decimal z)
    {
        Name = name;
        Reference = reference;
        X = x;
        Y = y;
        Z = z;
    }

    public string Name { get; set; }
    public string Reference { get; set; }
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Z { get; set; }
    public Guid MapId { get; set; }
    public Map Map { get; set; } = null!;
}