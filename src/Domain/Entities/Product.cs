namespace Educar.Backend.Domain.Entities;

    public class Product(string name) : BaseAuditableEntity
    {
        public string Name { get; set; } = name;
        public IList<ProductContent> ProductContents { get; private set; }  = new List<ProductContent>();
        public IList<ClientProduct> ClientProducts { get; private set; } = new List<ClientProduct>();
        public IList<ContractProduct> ContractProducts { get; private set; } = new List<ContractProduct>();
        public IList<ClassProduct> ClassProducts { get; private set; } = new List<ClassProduct>();
    }
