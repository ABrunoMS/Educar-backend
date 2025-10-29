namespace Educar.Backend.Domain.Entities;


public class ClassProduct
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}