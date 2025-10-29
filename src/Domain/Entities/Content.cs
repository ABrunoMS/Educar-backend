namespace Educar.Backend.Domain.Entities;

public class Content(string name) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public IList<ContractContent> ContractContents { get; private set; } = new List<ContractContent>();
    public IList<ProductContent> ProductContents { get; private set; } = new List<ProductContent>();
    public IList<ClientContent> ClientContents { get; private set; } = new List<ClientContent>();
    public IList<ClassContent> ClassContents { get; private set; } = new List<ClassContent>();
}