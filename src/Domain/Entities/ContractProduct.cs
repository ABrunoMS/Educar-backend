namespace Educar.Backend.Domain.Entities;

// Tabela de ligação: Contrato <-> Produto
public class ContractProduct
{
    public Guid ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}