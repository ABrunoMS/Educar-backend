using Educar.Backend.Application.Queries.Dialogue;
using Educar.Backend.Application.Queries.Game;
using Educar.Backend.Application.Queries.Item;
using Educar.Backend.Application.Queries.Npc;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Map;

public class MapCleanDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public MapType? Type { get; set; }
    public string? Reference2D { get; set; }
    public string? Reference3D { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Map, MapCleanDto>();
        }
    }
}