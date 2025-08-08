using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Media;

public class MediaDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public MediaPurpose? Purpose { get; set; }
    public MediaType? Type { get; set; }
    public string? References { get; set; }
    public string? Author { get; set; }
    public bool? Agreement { get; set; }
    public string? Url { get; set; }
    public string? ObjectName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Media, MediaDto>();
        }
    }
}