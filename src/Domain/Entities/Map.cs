using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Map : BaseAuditableEntity
{
    public Map(string name, string description, MapType type, string reference2D, string reference3D)
    {
        Name = name;
        Description = description;
        Type = type;
        Reference2D = reference2D;
        Reference3D = reference3D;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public MapType Type { get; set; }
    public string Reference2D { get; set; }
    public string Reference3D { get; set; }
    public IList<SpawnPoint> SpawnPoints { get; set; } = new List<SpawnPoint>();
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
}