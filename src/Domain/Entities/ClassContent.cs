namespace Educar.Backend.Domain.Entities;

public class ClassContent
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
}