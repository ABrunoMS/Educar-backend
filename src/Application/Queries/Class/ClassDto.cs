using Educar.Backend.Application.Queries.School;
using Educar.Backend.Domain.Enums;

namespace Educar.Backend.Application.Queries.Class;

public class ClassDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ClassPurpose Purpose { get; set; }
    public SchoolDto? School { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Class, ClassDto>();
        }
    }
}