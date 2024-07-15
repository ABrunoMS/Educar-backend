namespace Educar.Backend.Application.Queries.Address;

public class AddressDto
{
    public Guid Id { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Address, AddressDto>();
        }
    }
}