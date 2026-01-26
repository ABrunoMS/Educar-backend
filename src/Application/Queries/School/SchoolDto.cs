using Educar.Backend.Application.Queries.Account;
using Educar.Backend.Application.Queries.Address;
using Educar.Backend.Application.Queries.Client;
using Educar.Backend.Application.Queries.Regional;
using Educar.Backend.Domain.Enums;
using System.Linq;

namespace Educar.Backend.Application.Queries.School;

public class SchoolDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public AddressDto? Address { get; set; }
    public ClientCleanDto? Client { get; set; }
    public Guid ClientId { get; set; }
    public Guid RegionalId { get; set; }
    public RegionalDto? Regional { get; set; }
    public List<Guid>? TeacherIds { get; set; }
    public List<Guid>? StudentIds { get; set; }
    public List<Guid>? AccountIds { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.School, SchoolDto>()
                .ForMember(dest => dest.TeacherIds, opt => opt.MapFrom(src => 
                    src.AccountSchools.Where(acs => !acs.IsDeleted && acs.Account.Role == UserRole.Teacher).Select(acs => acs.AccountId).ToList()))
                .ForMember(dest => dest.StudentIds, opt => opt.MapFrom(src => 
                    src.AccountSchools.Where(acs => !acs.IsDeleted && acs.Account.Role == UserRole.Student).Select(acs => acs.AccountId).ToList()))
                .ForMember(dest => dest.AccountIds, opt => opt.MapFrom(src => 
                    src.AccountSchools.Where(acs => !acs.IsDeleted).Select(acs => acs.AccountId).ToList()));
        }
    }
}
