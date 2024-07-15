using Educar.Backend.Application.Queries.Account;
using Educar.Backend.Application.Queries.Address;
using Educar.Backend.Application.Queries.Client;

namespace Educar.Backend.Application.Queries.School;

public class SchoolDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public AddressDto? Address { get; set; }
    public ClientCleanDto? Client { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.School, SchoolDto>();
        }
    }
}