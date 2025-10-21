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
    public Guid SchoolId { get; set; }
    public bool IsActive { get; set; }
    public string? SchoolYear { get; set; }
    public string? SchoolShift { get; set; }
    public List<string>? Content { get; set; }
    public List<Guid>? TeacherIds { get; set; }
    public List<Guid>? StudentIds { get; set; }
    public List<Guid>? AccountIds { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Class, ClassDto>()
                .ForMember(dest => dest.TeacherIds, opt => opt.MapFrom(src => 
                    src.AccountClasses.Where(ac => !ac.IsDeleted && ac.Account.Role == UserRole.Teacher).Select(ac => ac.AccountId).ToList()))
                .ForMember(dest => dest.StudentIds, opt => opt.MapFrom(src => 
                    src.AccountClasses.Where(ac => !ac.IsDeleted && ac.Account.Role == UserRole.Student).Select(ac => ac.AccountId).ToList()))
                .ForMember(dest => dest.AccountIds, opt => opt.MapFrom(src => 
                    src.AccountClasses.Where(ac => !ac.IsDeleted).Select(ac => ac.AccountId).ToList()));
        }
    }
}