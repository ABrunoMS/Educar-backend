using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Domain.Entities;

public class Contract(
    int contractDurationInYears,
    DateTimeOffset contractSigningDate,
    DateTimeOffset implementationDate,
    int totalAccounts,
    ContractStatus status)
    : BaseAuditableEntity
{
    public int ContractDurationInYears { get; set; } = contractDurationInYears;
    public DateTimeOffset ContractSigningDate { get; set; } = contractSigningDate;
    public DateTimeOffset ImplementationDate { get; set; } = implementationDate;
    public int TotalAccounts { get; set; } = totalAccounts;
    public int? RemainingAccounts { get; set; }
    public string? DeliveryReport { get; set; }
    public ContractStatus Status { get; set; } = status;
    public Guid? ClientId { get; set; }
    public Client? Client { get; set; } = null!;
    public IList<ContractProduct> ContractProducts { get; private set; } = new List<ContractProduct>();
    public IList<ContractContent> ContractContents { get; private set; } = new List<ContractContent>();
}
