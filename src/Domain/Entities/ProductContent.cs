namespace Educar.Backend.Domain.Entities;

public class ProductContent
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
}