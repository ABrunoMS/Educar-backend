namespace Educar.Backend.Application.Queries.SpawnPoint;

public class SpawnPointCleanDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Reference { get; set; }
    public string? X { get; set; }
    public string? Y { get; set; }
    public string? Z { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.SpawnPoint, SpawnPointCleanDto>();
        }
    }
}