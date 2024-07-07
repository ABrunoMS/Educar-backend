using Educar.Backend.Application.Queries.Contract;

namespace Educar.Backend.Application.Queries.Client;

public class ClientDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ContractDto? Contract { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Client, ClientDto>();
        }
    }
}