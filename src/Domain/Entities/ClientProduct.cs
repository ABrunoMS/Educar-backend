namespace Educar.Backend.Domain.Entities;

// Tabela de ligação: Cliente <-> Produto (Quais produtos o cliente comprou)
public class ClientProduct
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}