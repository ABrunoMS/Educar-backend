namespace Educar.Backend.Domain.Entities; 

public class ContractContent
{
    public Guid ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
}