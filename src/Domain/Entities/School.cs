namespace Educar.Backend.Domain.Entities;

public class School(string name) : BaseAuditableEntity
{
    public string Name { get; set; } = name;
    public string? Description { get; set; }
    public Address? Address { get; set; }
    public Guid? AddressId { get; set; }
    public Client? Client { get; set; }
    public Guid ClientId { get; set; }
    //public List<Account> Accounts { get; set; } = [];
    public IList<AccountSchool> AccountSchools { get; set; } = new List<AccountSchool>();
}