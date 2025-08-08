using Educar.Backend.Application.Queries.SpawnPoint;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Map;

public class MapDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public MapType? Type { get; set; }
    public string? Reference2D { get; set; }
    public string? Reference3D { get; set; }
    public List<SpawnPointCleanDto>? SpawnPoints { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Map, MapDto>()
                .ForMember(dest => dest.SpawnPoints, opt => opt.MapFrom(src => src.SpawnPoints));
        }
    }
}