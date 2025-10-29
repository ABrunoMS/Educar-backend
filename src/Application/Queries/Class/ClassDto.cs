using Educar.Backend.Application.Queries;
using Educar.Backend.Application.Queries.School;
using Educar.Backend.Application.Queries.Product;
using Educar.Backend.Application.Queries.Content;
using Educar.Backend.Domain.Enums;
using AutoMapper;
using System.Linq; // Adicione este using


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
    public List<ProductDto> Products { get; set; } = new();
    public List<ContentDto> Contents { get; set; } = new();
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
                    src.AccountClasses.Where(ac => !ac.IsDeleted).Select(ac => ac.AccountId).ToList()))     
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                    src.ClassProducts.Select(cp => cp.Product)))
                .ForMember(dest => dest.Contents, opt => opt.MapFrom(src =>
                    src.ClassContents.Select(cc => cc.Content)));
        }
    }
}