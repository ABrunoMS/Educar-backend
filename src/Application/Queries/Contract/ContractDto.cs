using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Contract;

public class ContractDto
{
    public Guid Id { get; set; }
    public int ContractDurationInYears { get; set; }
    public DateTimeOffset ContractSigningDate { get; set; }
    public DateTimeOffset ImplementationDate { get; set; }
    public int TotalAccounts { get; set; }
    public int? RemainingAccounts { get; set; }
    public string? DeliveryReport { get; set; }
    public ContractStatus Status { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Contract, ContractDto>();
        }
    }
}