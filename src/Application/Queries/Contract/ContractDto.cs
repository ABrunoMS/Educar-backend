using Educar.Backend.Domain.Enums;
using Educar.Backend.Application.Queries;
using Educar.Backend.Application.Queries.Product;
using Educar.Backend.Application.Queries.Content;

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
    public Guid? ClientId { get; set; }
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    public IList<ContentDto> Contents { get; set; } = new List<ContentDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Contract, ContractDto>()

            .ForMember(dest => dest.Products, opt =>
                    opt.MapFrom(src => src.ContractProducts.Select(cp => cp.Product)))
                
                
            .ForMember(dest => dest.Contents, opt =>
                    opt.MapFrom(src => src.ContractContents.Select(cc => cc.Content)));
        
        }
    }
}