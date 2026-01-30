using AutoMapper;

namespace Educar.Backend.Application.Queries.Client;

public class ClientCleanDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? MacroRegionId { get; set; }
    public string? MacroRegionName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Client, ClientCleanDto>()
                .ForMember(dest => dest.MacroRegionId, opt => opt.MapFrom(src => src.MacroRegionId))
                .ForMember(dest => dest.MacroRegionName, opt => opt.MapFrom(src => src.MacroRegion != null ? src.MacroRegion.Name : null));
        }
    }
}