namespace Educar.Backend.Application.Queries.Client;

public class ClientCleanDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Client, ClientCleanDto>();
        }
    }
}